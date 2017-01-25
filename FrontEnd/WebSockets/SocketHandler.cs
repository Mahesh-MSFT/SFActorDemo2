using FrontEnd.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Newtonsoft.Json;
using SoppingCart.Interfaces;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrontEnd.WebSockets
{
    public class SocketHandler
    {
        public const int BufferSize = 4096;

        WebSocket socket;
        string IPAddress;

        SocketHandler(WebSocket socket, string ipAddress)
        {
            this.socket = socket;
            this.IPAddress = ipAddress;
        }

        async Task EchoLoop()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);

            while (this.socket.State == WebSocketState.Open)
            {
                var incoming = await this.socket.ReceiveAsync(seg, CancellationToken.None);
                ////var incomingdecoded = new ArraySegment<byte>(buffer, 0, incoming.Count);
                var incomingdecoded = Encoding.Default.GetString(buffer, 0, incoming.Count);

                ActorId actorId = new ActorId("Demo");
                ISoppingCart sc = ActorProxy.Create<ISoppingCart>(actorId, "fabric:/SFActorDemoApp");

                ShoppingItem si = new ShoppingItem { ShoppingItemCategory = incomingdecoded, IPAddress = this.IPAddress, AddedOn = DateTime.UtcNow };
                await sc.AddToCartAsync(si);

                ActorId actorId2 = new ActorId("Demo");
                ISoppingCart sc2 = ActorProxy.Create<ISoppingCart>(actorId2, "fabric:/SFActorDemoApp");

                var ret = sc2.GetCartItemsAsync().Result;

                ret.NewCartItem =((ShoppingCategoryEnum)Convert.ToInt16(ret.NewCartItem)).ToString();
                //ret.NewCartItem = ((FaultyShoppingCategoryEnum)Convert.ToInt16(ret.NewCartItem)).ToString();

                var data = JsonConvert.SerializeObject(ret);
                var encoded = Encoding.UTF8.GetBytes(data);
                var buffer2 = new ArraySegment<Byte>(encoded, 0, encoded.Length);

                await this.socket.SendAsync(buffer2, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        static async Task Acceptor(HttpContext hc, Func<Task> n)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;

            var socket = await hc.WebSockets.AcceptWebSocketAsync();
            var h = new SocketHandler(socket, hc.Connection.RemoteIpAddress.ToString());

            await h.EchoLoop();

        }

        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(SocketHandler.Acceptor);
        }
    }
}
