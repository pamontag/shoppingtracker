using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingTracker.Core.Data;
using System.Net.Http;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace ShoppingTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MagicMerchantController : ControllerBase
    {
        // ILogger of MagicMerchantController type
        private readonly ILogger<MagicMerchantController> _logger;

        // GET: api/MagicMerchant
        [HttpGet]
        public IActionResult SearchGames(string gamename)
        {
            // TODO: Implement logic to get MagicMerchant data
            return Ok("Get MagicMerchant data");
        }
 

        // POST: api/MagicMerchant
        [HttpPost]
        public async Task<ShoppingFavorite> AddGameToFavorites([FromBody] ProductPrice game)
        {
            // copy the method AddGameToFavorites from PsStoreController
            // and implement the logic to add the game to the favorites
            var shoppingFavorite = new ShoppingFavorite(ShopTypeEnum.PSStore,game.Id) {
                Name = game.Name,
                Discount = game.Discount,
                DiscountedPrice = game.DiscountedPrice,
                Price = game.Price,
                Store = (int)ShopTypeEnum.PSStore,
                Media = game.Media,
                DiscountDate = String.IsNullOrWhiteSpace(game.Discount) ? null : (DateTime?)DateTime.Now,
                FavoritesDate = DateTime.Now
            }; 
            var table = await ShoppingTrackerDAL.CreateTableAsync(ShoppingTrackerDAL.SHOPPING_FAVORITES_TABLE);
            var favorite = await ShoppingTrackerDAL.InsertOrMergeShoppingFavoriteAsync(table,shoppingFavorite);
            return favorite; 

        }
 
    }
}
