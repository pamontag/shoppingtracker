// the purpose of this class is a web api controller that search games  on the dungeondice site and consent to add a game to favorites

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

// create a new controller named DungeonDiceController
namespace ShoppingTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DungeonDiceController : ControllerBase
    {
        // ILogger of DungeonDiceController type
        private readonly ILogger<DungeonDiceController> _logger;

        private readonly string _urlStore;        

        public DungeonDiceController(ILogger<DungeonDiceController> logger)
        {
            _logger = logger;
            _urlStore = "https://www.dungeondice.it/ricerca";
            
        }

        // GET: api/DungeonDice
        [HttpGet]
        public async Task<IEnumerable<ProductPrice>> SearchGames(string gamename)
        {
            var games = new List<ProductPrice>();
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpResponseMessage response = await client.PostAsync(_urlStore,new StringContent($"s={gamename}&resultsPerPage=10"));
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var searchResult = JsonConvert.DeserializeObject<DungeonDiceSearchResult>(jsonResponse);
                foreach(var game in searchResult.products) {
                    games.Add(new ProductPrice() {
                        Name = game.name,
                        Classification = game.out_of_stock == "2" ? "Out of stock" : "In stock",
                        Discount = game.price,
                        DiscountedPrice = game.price,
                        Id = game.id_product,
                        Media = game.images != null && game.images.Count > 0 ? game.images[0].bySize.small_default.url : null,                       
                        Price = game.price 
                    });
                }
                return games;                
            } 
            return null;
        }

        [HttpPost]
        public async Task<ShoppingFavorite> AddToFavorites(ProductPrice game)
        {
            return null;
        }
    }
}