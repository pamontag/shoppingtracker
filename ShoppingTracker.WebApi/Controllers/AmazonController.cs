﻿using System;
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
using System.Globalization;

namespace ShoppingTracker.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AmazonController : ControllerBase
    { 
        private readonly ILogger<PsStoreController> _logger;
        private readonly string _urlStore;        

        public AmazonController(ILogger<PsStoreController> logger)
        {
            _logger = logger;
            _urlStore = @"https://www.amazon.it/s?k={0}&__mk_it_IT=ÅMÅŽÕÑ&ref=sr_pg_1";
            
        }

        [HttpGet]
        public async Task<IEnumerable<ProductPrice>> SearchProducts(string productName)
        {
            var products = new List<ProductPrice>();
            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(String.Format(_urlStore,productName));
            if (response.IsSuccessStatusCode)
            {
                var htmlResponse = await response.Content.ReadAsStringAsync();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlResponse);
                var nodes = doc.DocumentNode.SelectNodes("//*[@id=\"search\"]//div[@class=\"sg-col-inner\"]//div[@data-asin]");
                foreach(var node in nodes) {
                    var newPrice = node.SelectSingleNode(".//span[@class=\"a-price\"]/span[@class=\"a-offscreen\"]");
                    var oldPrice = node.SelectSingleNode(".//span[@class=\"a-price a-text-price\"]/span[@class=\"a-offscreen\"]");
                    var name = node.SelectSingleNode(".//span[@class=\"a-size-base-plus a-color-base a-text-normal\"]");
                    var media = node.SelectSingleNode(".//span/a/div/img");
                    var platform = node.SelectSingleNode(".//a[@class=\"a-size-base a-link-normal a-text-bold\"]");
                    var prime = node.SelectSingleNode(".//i[@class=\"a-icon a-icon-prime a-icon-medium\"]");
                    string discountedText = null;
                    _logger.LogInformation($"name: {name}");
                    if(oldPrice != null && newPrice != null) {
                        double newNumericPrice;
                        double oldNumericPrice;
                        var oldpriceString = oldPrice.InnerText.Replace("€","").Replace(",",".").Trim();
                        var newpriceString = newPrice.InnerText.Replace("€","").Replace(",",".").Trim();
                        _logger.LogInformation($"newprice: {newpriceString} oldprice: {oldpriceString}");
                        if(double.TryParse(newpriceString, NumberStyles.Any, CultureInfo.InvariantCulture, out newNumericPrice) && double.TryParse(oldpriceString, NumberStyles.Any, CultureInfo.InvariantCulture, out oldNumericPrice)) {
                            discountedText = string.Format("{0:N2}%",(1 - (newNumericPrice / oldNumericPrice)) * 100);
                            _logger.LogInformation($"discount {discountedText}");
                        }
                    }
                    var product = new ProductPrice() {
                        Id = node.Attributes["data-asin"].Value,
                        Name = name != null ? name.InnerText.Trim() : null,
                        Price = oldPrice != null ? oldPrice.InnerText : newPrice != null ? newPrice.InnerText : null,
                        DiscountedPrice = newPrice != null ? newPrice.InnerText : null,
                        Discount = discountedText,
                        Media = media != null && media.Attributes["src"] != null ? media.Attributes["src"].Value : null,
                        Platform = platform != null && platform.InnerText != null ? platform.InnerText.Trim() : null,
                        Classification = prime != null ? "PRIME" : null  
                    };
                    products.Add(product);
                }
                return products;
           
            } 
            return null;
        }

        [HttpPost]
        public async Task<ShoppingFavorite> AddToFavorites(ProductPrice product)
        {
            var shoppingFavorite = new ShoppingFavorite(ShopTypeEnum.Amazon,product.Id) {
                Name = product.Name,
                Discount = product.Discount,
                DiscountDate = String.IsNullOrWhiteSpace(product.Discount) ? null : (DateTime?)DateTime.Now,
                DiscountedPrice = product.DiscountedPrice,
                FavoritesDate = DateTime.Now,
                Price = product.Price,
                Media = product.Media,
                Store = ShopTypeEnum.Amazon
            };
            var table = await ShoppingTrackerDAL.CreateTableAsync(ShoppingTrackerDAL.SHOPPING_FAVORITES_TABLE);
            var favorite = await ShoppingTrackerDAL.InsertOrMergeShoppingFavoriteAsync(table,shoppingFavorite);
            return favorite; 
        }
    }
}
