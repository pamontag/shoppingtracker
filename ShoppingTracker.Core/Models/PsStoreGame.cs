    using System;
using System.Collections.Generic;

namespace ShoppingTracker.Core.Models
{
    
    public class Concept    {
        public string __typename { get; set; } 
        public string id { get; set; } 
    }

    public class PSStoreGameSku    {
        public string __typename { get; set; } 
        public string id { get; set; } 
        public string name { get; set; } 
    }

    public class Param    {
        public string __typename { get; set; } 
        public string name { get; set; } 
        public string value { get; set; } 
    }

    public class Action    {
        public string __typename { get; set; } 
        public List<Param> param { get; set; } 
        public string type { get; set; } 
    }

    public class IneligibilityReason    {
        public string __typename { get; set; } 
        public List<object> names { get; set; } 
        public string type { get; set; } 
    }

    public class Meta    {
        public string __typename { get; set; } 
        public bool exclusive { get; set; } 
        public List<IneligibilityReason> ineligibilityReasons { get; set; } 
        public object playabilityDate { get; set; } 
        public string upSellService { get; set; } 
    }

    public class PsStoreGamePrice    {
        public string __typename { get; set; } 
        public string applicability { get; set; } 
        public string basePrice { get; set; } 
        public int basePriceValue { get; set; } 
        public string campaignId { get; set; } 
        public string currencyCode { get; set; } 
        public string discountText { get; set; } 
        public string discountedPrice { get; set; } 
        public int discountedValue { get; set; } 
        public string endTime { get; set; } 
        public bool isExclusive { get; set; } 
        public bool isFree { get; set; } 
        public bool isTiedToSubscription { get; set; } 
        public List<object> qualifications { get; set; } 
        public string rewardId { get; set; } 
        public List<object> serviceBranding { get; set; } 
        public string upsellText { get; set; } 
    }

    public class Webcta    {
        public string __typename { get; set; } 
        public Action action { get; set; } 
        public bool hasLinkedConsole { get; set; } 
        public Meta meta { get; set; } 
        public string type { get; set; } 
        public PsStoreGamePrice price { get; set; } 
    }

    public class ProductRetrieve    {
        public string __typename { get; set; } 
        public Concept concept { get; set; } 
        public string id { get; set; } 
        public string name { get; set; } 
        public List<PSStoreGameSku> skus { get; set; } 
        public List<Webcta> webctas { get; set; } 
    }

    public class DataPsStoreGame    {
        public ProductRetrieve productRetrieve { get; set; } 
    }

    public class PsStoreGame    {
        public DataPsStoreGame data { get; set; } 
    }
}