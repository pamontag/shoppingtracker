using System;
using System.Collections.Generic;

namespace ShoppingTracker.Core.Models
{

    public class Medium    {
        public string __typename { get; set; } 
        public string role { get; set; } 
        public string type { get; set; } 
        public string url { get; set; } 
    }

    public class Price    {
        public string __typename { get; set; } 
        public string basePrice { get; set; } 
        public string discountText { get; set; } 
        public string discountedPrice { get; set; } 
        public List<object> serviceBranding { get; set; } 
        public string skuId { get; set; } 
        public object upsellServiceBranding { get; set; } 
        public object upsellText { get; set; } 
    }

    public class Sku    {
        public string __typename { get; set; } 
        public string type { get; set; } 
    }

    public class Item    {
        public string __typename { get; set; } 
        public string id { get; set; } 
        public string localizedStoreDisplayClassification { get; set; } 
        public List<Medium> media { get; set; } 
        public string name { get; set; } 
        public List<string> platforms { get; set; } 
        public Price price { get; set; } 
        public List<Sku> skus { get; set; } 
    }

    public class PageInfo    {
        public string __typename { get; set; } 
        public bool isLast { get; set; } 
        public int offset { get; set; } 
        public int size { get; set; } 
        public int totalCount { get; set; } 
    }

    public class UniversalSearch    {
        public string __typename { get; set; } 
        public List<Item> items { get; set; } 
        public string next { get; set; } 
        public PageInfo pageInfo { get; set; } 
        public string searchTerm { get; set; } 
    }

    public class Data    {
        public UniversalSearch universalSearch { get; set; } 
    }

    public class SearchResult    {
        public Data data { get; set; } 
    }
}