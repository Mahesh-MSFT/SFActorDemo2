using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEnd.Models
{
    public class IndexModel
    {
        [ReadOnly(true)]
        public string ShoppingCategory { get; set; }

        public Dictionary<string, int> Recommendations { get; set; }

        public string CrossSaleItem { get; set; }
    }
}
