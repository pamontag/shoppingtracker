using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ShoppingTracker.Core.Data;
using ShoppingTracker.Core.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;

namespace ShoppingTracker.Function
{
    public static class UpdateFavorites
    {
        [FunctionName("UpdateFavorites")]
        public static async Task Run([TimerTrigger("0 */30 * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var connectionString = config.GetConnectionString("StorageConnectionString");
            log.LogInformation($"Connection string: {connectionString}");
            var table = await ShoppingTrackerDAL.CreateTableAsync(ShoppingTrackerDAL.SHOPPING_FAVORITES_TABLE, connectionString);
            var favorites = await ShoppingTrackerDAL.GetShoppingFavoriteAsync(table);
            log.LogInformation($"Favorites to analyze: {favorites.Count()}");
            List<string> offers = new List<string>();
            foreach (var favorite in favorites)
            {
                Enum.TryParse(favorite.PartitionKey, out ShopTypeEnum shopType);
                log.LogInformation($"Analyze favorite: {favorite.Name} {favorite.PartitionKey}");
                switch (shopType)
                {
                    case ShopTypeEnum.PSStore:
                        var offer = await UpdateFavoritePSStoreAsync(table, favorite, log);
                        if (offer != null)
                            offers.Add(offer);
                        break;

                    default:
                        log.LogError($"shopType {favorite.PartitionKey} not implemented");
                        break;
                }

            }
            log.LogInformation($"Found {offers.Count()} new price change");
            if (offers.Count() > 0)
            {
                if (await SendMailWithOffers(offers, log))
                {
                    log.LogInformation("Sended mail to recipients");
                }
                else
                {
                    log.LogError("Can't send mail to recipients");
                }
            }
        }

        private static async Task<bool> SendMailWithOffers(List<string> offers, ILogger log)
        {
            var sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            var toMail = Environment.GetEnvironmentVariable("ToMail");
            log.LogInformation($"{toMail} - {sendGridApiKey}");
            var client = new SendGridClient(sendGridApiKey);
            var from = new EmailAddress("paolomontagna@gmail.com", "ShoppingTracker");
            var subject = "New Offers found from Shopping Tracker";
            var to = new EmailAddress(toMail, "Offer subscriber");
            var plainTextContent = "Offers from Shopping Tracker" + Environment.NewLine + Environment.NewLine;
            var htmlContent = "<h1>Offers from Shopping Tracker</h1><ul>";
            foreach (var offer in offers)
            {
                htmlContent = htmlContent + $"<li>{offer}</li>";
                plainTextContent = plainTextContent + offer + Environment.NewLine;
            }
            htmlContent = htmlContent + "</ul>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        private static async Task<string> UpdateFavoritePSStoreAsync(Microsoft.Azure.Cosmos.Table.CloudTable table, ShoppingFavorite favorite, ILogger log)
        {
            var baseAddress = "https://web.np.playstation.com/api/graphql/v1/op?operationName=productRetrieveForCtasWithPrice&variables=%7B%22productId%22%3A%22{0}%22%7D&extensions=%7B%22persistedQuery%22%3A%7B%22version%22%3A1%2C%22sha256Hash%22%3A%228532da7eda369efdad054ca8f885394a2d0c22d03c5259a422ae2bb3b98c5c99%22%7D%7D";
            string offer = null;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-psn-store-locale-override", "it-IT");
                HttpResponseMessage response = await client.GetAsync(String.Format(baseAddress, favorite.RowKey));
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var game = JsonConvert.DeserializeObject<PsStoreGame>(jsonResponse);
                    if (game.data.productRetrieve != null)
                    {
                        if (favorite.DiscountedPrice != game.data.productRetrieve.webctas[0].price.discountedPrice)
                        {
                            offer = $"{favorite.Name} - Old Price: {favorite.DiscountedPrice} - New Price: {game.data.productRetrieve.webctas[0].price.discountedPrice} - Discount: {game.data.productRetrieve.webctas[0].price.discountText}";
                            log.LogInformation($"Discount {game.data.productRetrieve.webctas[0].price.discountText} for favorite {favorite.Name} - Old Price: {favorite.DiscountedPrice} - New Price: {game.data.productRetrieve.webctas[0].price.discountedPrice}");
                        }
                        favorite.Discount = String.IsNullOrEmpty(game.data.productRetrieve.webctas[0].price.discountText) ? String.Empty : game.data.productRetrieve.webctas[0].price.discountText;
                        favorite.DiscountedPrice = game.data.productRetrieve.webctas[0].price.discountedPrice;
                        favorite.DiscountDate = DateTime.Now;
                        favorite.Price = game.data.productRetrieve.webctas[0].price.basePrice;
                        await ShoppingTrackerDAL.InsertOrMergeShoppingFavoriteAsync(table, favorite);
                    }
                    else
                    {
                        log.LogError($"Product is no more available in the store: {favorite.Name}");
                    }
                }
                else
                {
                    log.LogError($"Impossible to update favorite: {favorite.Name}");
                }
            }
            return offer;
        }
    }
}
