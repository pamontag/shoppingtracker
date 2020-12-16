using System;
using Microsoft.Azure.Cosmos.Table;
namespace ShoppingTracker.Core.Models
{
    public class ShoppingFavorite :  TableEntity
    {
        public ShoppingFavorite()
        {
        }

        public ShoppingFavorite(ShopTypeEnum shopType, string id)
        {
            PartitionKey = shopType.ToString();
            RowKey = id;
        }
        public string Name {get; set;}
        public string Price {get;set;}
        public string DiscountedPrice {get; set;}
        public string Discount {get; set;}
        public DateTime? DiscountDate {get; set;}
        public DateTime FavoritesDate {get; set;}
        public string Media{get; set;}
        
    }

}