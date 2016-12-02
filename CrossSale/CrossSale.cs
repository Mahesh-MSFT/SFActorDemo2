using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using CrossSale.Interfaces;

namespace CrossSale
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
    internal class CrossSale : Actor, ICrossSale
    {
        /// <summary>
        /// Initializes a new instance of CrossSale
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public CrossSale(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public Task<int> DoCrossSale(string itemForCrossSale)
        {
            int scn;
            switch (itemForCrossSale)
            {
                case "Electronics":
                    scn = 7;
                    break;
                case "Toys":
                    scn = 0;
                    break;
                case "Clothes":
                    scn = 9;
                    break;
                case "Perfumes":
                    scn = 8;
                    break;
                case "Books":
                    scn = 1;
                    break;
                case "Shoes":
                    scn = 6;
                    break;
                case "Flowers":
                    scn = 5;
                    break;
                case "Furniture":
                    scn = 4;
                    break;
                case "Food":
                    scn = 3;
                    break;
                case "Wines":
                    scn = 2;
                    break;
                default:
                    scn = 1;
                    break;
            }

            return Task.FromResult(scn);
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

            return this.StateManager.TryAddStateAsync("count", 0);
        }

    }
}
