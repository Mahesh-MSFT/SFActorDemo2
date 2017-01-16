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
using System.Text;

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
        private readonly string version;
        /// <summary>
        /// Initializes a new instance of SoppingCart
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public SoppingCart(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
            version = actorService.Context.CodePackageActivationContext.CodePackageVersion;
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
            this.StateManager.TryAddStateAsync<string>("Version", version);
            return this.StateManager.TryAddStateAsync<Recommendation>("State", new Recommendation());

        }

        Task<CartItem> ISoppingCart.GetCartItemsAsync()
        {
            var cartItem = new CartItem();

            var allActors = this.StateManager.GetStateAsync<Recommendation>("State").Result;
            var allrecomm = allActors.RecommendationList.GroupBy(n => n.ShoppingItemCategory).OrderByDescending(g => g.Count()).ToDictionary(g => g.Key, g => g.Count().ToString());
            var recomm = allActors.RecommendationList.Where(x => x.AddedOn > DateTime.UtcNow.AddSeconds(-2)).GroupBy(x => x.IPAddress).Select(t => t.OrderByDescending(c => c.AddedOn).First()).ToDictionary(x => x.IPAddress, x=> x.ShoppingItemCategory);
            

            Random rnd = new Random();
            int scn = rnd.Next(0, 10);

            cartItem.NewCartItem = scn.ToString();
            cartItem.OtherCartItems = recomm;
            cartItem.AllCartItems = allrecomm;

            return Task.FromResult<CartItem>(cartItem);
        }

        Task ISoppingCart.AddToCartAsync(ShoppingItem shoppingItem)
        {
           
                Recommendation rec = this.StateManager.GetStateAsync<Recommendation>("State").Result;

                if (!String.IsNullOrWhiteSpace(shoppingItem.ShoppingItemCategory))

                {
                    rec.RecommendationList.Add(shoppingItem);
                    ////rec.RecommendationList.Add(new ShoppingItem { AddedOn = DateTime.UtcNow, IPAddress = "10.45.278:90", ShoppingItemCategory = "Test" });
                }

                return this.StateManager.SetStateAsync<Recommendation>("State", rec);
        }

        Task<string> ISoppingCart.GetVersionAsync()
        {
            return this.StateManager.GetStateAsync<string>("Version");
        }
    }
}
