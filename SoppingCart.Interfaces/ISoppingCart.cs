using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace SoppingCart.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface ISoppingCart : IActor
    {
        Task<CartItem> GetCartItemsAsync();

        Task AddToCartAsync(ShoppingItem shoppingItem);

        Task<string> GetVersionAsync();
    }
}
