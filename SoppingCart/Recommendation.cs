using SoppingCart.Interfaces;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SoppingCart
{
    [DataContract]
    public class Recommendation
    {
        public Recommendation()
        {
            this.RecommendationList = new List<ShoppingItem>();
        }

        [DataMember]
        public List<ShoppingItem> RecommendationList { get; set; }


    }
}
