using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingTracker.Core.Models;
using Newtonsoft.Json;
using ShoppingTracker.Core.Data;

namespace ShoppingTracker.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PsStoreController : ControllerBase
    { 
        //https://web.np.playstation.com/api/graphql/v1//op?operationName=getSearchResults&variables=%7B%22countryCode%22%3A%22IT%22%2C%22languageCode%22%3A%22it%22%2C%22pageOffset%22%3A0%2C%22pageSize%22%3A48%2C%22searchTerm%22%3A%22survivalists%22%7D&extensions=%7B%22persistedQuery%22%3A%7B%22version%22%3A1%2C%22sha256Hash%22%3A%22d7d773f7497d9aba365bbab376ed349f15bac9a8cd3c879e5a476587aa397e66%22%7D%7D
        //x-psn-store-locale-override it-IT
        private readonly ILogger<PsStoreController> _logger;
        private readonly string _urlStore;

        

        public PsStoreController(ILogger<PsStoreController> logger)
        {
            _logger = logger;
            _urlStore = "https://web.np.playstation.com/api/graphql/v1//op?operationName=getSearchResults&variables=%7B%22countryCode%22%3A%22IT%22%2C%22languageCode%22%3A%22it%22%2C%22pageOffset%22%3A0%2C%22pageSize%22%3A48%2C%22searchTerm%22%3A%22{0}%22%7D&extensions=%7B%22persistedQuery%22%3A%7B%22version%22%3A1%2C%22sha256Hash%22%3A%22d7d773f7497d9aba365bbab376ed349f15bac9a8cd3c879e5a476587aa397e66%22%7D%7D";
            
        }

        [HttpGet]
        public async Task<IEnumerable<GamePrice>> SearchGames(string gamename)
        {
            var games = new List<GamePrice>();
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-psn-store-locale-override", "it-IT");
            HttpResponseMessage response = await client.GetAsync(String.Format(_urlStore,gamename));
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var searchResult = JsonConvert.DeserializeObject<SearchResult>(jsonResponse);
                foreach(var game in searchResult.data.universalSearch.items) {
                    games.Add(new GamePrice() {
                        Name = game.name,
                        Classification = game.localizedStoreDisplayClassification,
                        Discount = game.price.discountText,
                        DiscountedPrice = game.price.discountedPrice,
                        Id = game.id,
                        Media = game.media.SingleOrDefault(m => m.role == "MASTER") != null ? game.media.SingleOrDefault(m => m.role == "MASTER").url : null,
                        Platform = string.Join(",",game.platforms),
                        Price = game.price.basePrice
                    });
                }
                return games;                
            } 
            return null;
        }

        [HttpPost]
        public async Task<ShoppingFavorite> AddToFavorites(GamePrice game)
        {
            var shoppingFavorite = new ShoppingFavorite(ShopTypeEnum.PSStore,game.Id) {
                Name = game.Name,
                Discount = game.Discount,
                DiscountDate = String.IsNullOrWhiteSpace(game.Discount) ? null : (DateTime?)DateTime.Now,
                DiscountedPrice = game.DiscountedPrice,
                FavoritesDate = DateTime.Now,
                Price = game.Price,
                Media = game.Media
            };
            var table = await ShoppingTrackerDAL.CreateTableAsync(ShoppingTrackerDAL.SHOPPING_FAVORITES_TABLE);
            var favorite = await ShoppingTrackerDAL.InsertOrMergeShoppingFavoriteAsync(table,shoppingFavorite);
            return favorite; 
        }
    }
}
