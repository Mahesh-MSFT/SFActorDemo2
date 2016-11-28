using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using SoppingCart.Interfaces;
using Microsoft.ServiceFabric.Data.Collections;
using CrossSale.Interfaces;

namespace SoppingCart
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class SoppingCart : Actor, ISoppingCart
    {
        /// <summary>
        /// Initializes a new instance of SoppingCart
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public SoppingCart(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            //return this.StateManager.TryAddStateAsync("shoppingcart", new ShoppingItem { ShoppingItemCategory = "testname"});

            return this.StateManager.TryAddStateAsync<Recommendation>("State", new Recommendation());
        }

        Task<Dictionary<string, int>> ISoppingCart.GetCartItemsAsync()
        {
            //return this.StateManager.GetStateAsync<List<ShoppingItem>>("shoppingcart");
            //var allActors = this.StateManager.GetStateNamesAsync().Result;

            var allActors = this.StateManager.GetStateAsync<Recommendation>("State").Result;

            //var recomm = allActors.RecommendationList.GroupBy(n => n.ShoppingItemCategory, (key, values) => new { Category = key, Count = values.Count() });
            var recomm = allActors.RecommendationList.GroupBy(n => n.ShoppingItemCategory).OrderByDescending(g => g.Count()).ToDictionary(g => g.Key, g => g.Count());

            //return Task.FromResult<int>(allActors.RecommendationList.Count());

            return Task.FromResult<Dictionary<string, int>>(recomm);
        }

        Task ISoppingCart.AddToCartAsync(ShoppingItem shoppingItem)
        {
            //var shoppingCartList = this.StateManager.GetOrAddStateAsync<IReliableDictionary<string, ShoppingItem>>("",)

            ////return this.StateManager.GetOrAddStateAsync<IReliableDictionary<string, ShoppingItem>>("shoppingcartList", shoppingItem);

          Recommendation rec = this.StateManager.GetStateAsync<Recommendation>("State").Result;

            rec.RecommendationList.Add(shoppingItem);

            return this.StateManager.SetStateAsync<Recommendation>("State", rec);


            ///return this.StateManager.AddOrUpdateStateAsync(shoppingItem.ShoppingItemCategory, shoppingItem, (k , v) => shoppingItem);

            //return this.StateManager.AddStateAsync<ShoppingItem>(shoppingItem.ShoppingItemName, shoppingItem);

            ////return this.StateManager.AddOrUpdateStateAsync(shoppingItem.ShoppingItemCategory, shoppingItem);

        }
    }
}
