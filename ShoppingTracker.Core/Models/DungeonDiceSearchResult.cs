using System.Collections.Generic;

namespace ShoppingTracker.Core.Models
{
    public class DungeonDiceSearchResult
    {
        public List<Products> products { get; set; }
    }

    public class Products
    {
        // Define properties for the search result here
        public string id_product { get; set; }
        public string price { get; set; }
        public string out_of_stock { get; set; }
        public string avialable_date { get; set; }

        public string name { get; set; }
        public string link { get; set; }

        public List<Images> images { get; set; }
    }

    public class Images
    {
        public BySize bySize { get; set; }
    }

    public class BySize
    {
        public SmallDefault small_default { get; set; }
    }

    public class SmallDefault
    {
        public string url { get; set; }
    }
}