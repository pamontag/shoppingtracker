using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingTracker.Core.Models;
using Newtonsoft.Json;
using ShoppingTracker.Core.Data;

namespace ShoppingTracker.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FavoritesController : ControllerBase
    { 
        [HttpGet]
        public async Task<IEnumerable<ShoppingFavorite>> GetFavorites()
        {
            var table = await ShoppingTrackerDAL.CreateTableAsync(ShoppingTrackerDAL.SHOPPING_FAVORITES_TABLE);
            return await ShoppingTrackerDAL.GetShoppingFavoriteAsync(table);
        }

        [HttpDelete]
        public async Task<double?> DeleteFavorite(string id, string partition) {
            var table = await ShoppingTrackerDAL.CreateTableAsync(ShoppingTrackerDAL.SHOPPING_FAVORITES_TABLE);
            return await ShoppingTrackerDAL.DeleteFavoriteAsync(table, partition, id);
        }

    }
}