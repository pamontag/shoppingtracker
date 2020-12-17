using System;

namespace ShoppingTracker.Core.Models
{
    public class ProductPrice
    {
        public string Id {get; set;}
        public string Platform {get; set;}
        public string Classification {get; set;}        
        public string Name {get; set;}
        public string Price {get;set;}
        public string DiscountedPrice {get; set;}
        public string Discount {get; set;}
        public string Media{get; set;}
        
    }
}
