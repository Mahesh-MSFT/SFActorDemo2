using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using SoppingCart.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using FrontEnd.Models;
using CrossSale.Interfaces;

namespace FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var im = new IndexModel();
            Random rnd = new Random();
            int scn = rnd.Next(0, 10);

            ShoppingCategoryEnum sce = (ShoppingCategoryEnum)scn;

            im.ShoppingCategory = sce.ToString();

            ActorId actorId = new ActorId(HttpContext.Connection.RemoteIpAddress.ToString());

            ISoppingCart sc = ActorProxy.Create<ISoppingCart>(actorId, "fabric:/SFActorDemoApp");

            ICrossSale cs = ActorProxy.Create<ICrossSale>(actorId, "fabric:/SFActorDemoApp");

            int csi = cs.DoCrossSale(im.ShoppingCategory).Result;

            ShoppingCategoryEnum sce2 = (ShoppingCategoryEnum)csi;

            im.CrossSaleItem = sce2.ToString();

            im.Recommendations = sc.GetCartItemsAsync().Result;

            ViewData["Version"] = sc.GetVersionAsync().Result;

            return View(im);
        }

        [HttpPost]
        public IActionResult Index(IndexModel indexModel)
        {
            ActorId actorId = new ActorId(HttpContext.Connection.RemoteIpAddress.ToString());

            ISoppingCart sc = ActorProxy.Create<ISoppingCart>(actorId, "fabric:/SFActorDemoApp");

            ShoppingItem si = new ShoppingItem { ShoppingItemCategory = indexModel.ShoppingCategory };
            sc.AddToCartAsync(si);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Logout(IndexModel indexModel)
        {
            ActorId actorId = new ActorId(HttpContext.Connection.RemoteIpAddress.ToString());

            IActorService myActorServiceProxy = ActorServiceProxy.Create(new Uri("fabric:/SFActorDemoApp"), actorId);

            myActorServiceProxy.DeleteActorAsync(actorId, new System.Threading.CancellationToken());

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddToCart2(ShoppingItem shoppinItem)
        {
            // Create a randomly distributed actor ID
            ActorId actorId = new ActorId(HttpContext.Connection.RemoteIpAddress.ToString());

            ISoppingCart sc = ActorProxy.Create<ISoppingCart>(actorId, "fabric:/SFActorDemoApp");
            sc.AddToCartAsync(shoppinItem);

            return RedirectToAction("Index");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
