using System.Collections.Generic;

namespace SoppingCart.Interfaces
{
    public class CartItem
    {
        public string NewCartItem { get; set; }

        public Dictionary<string, string> OtherCartItems { get; set; }

        public Dictionary<string, string> AllCartItems { get; set; }
    }
}
