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
            return View(im);
        }

        [HttpPost]
        public IActionResult Index(IndexModel indexModel)
        {
            //ActorId actorId = new ActorId(HttpContext.Connection.RemoteIpAddress.ToString());
            ActorId actorId = new ActorId("Demo");

            ISoppingCart sc = ActorProxy.Create<ISoppingCart>(actorId, "fabric:/SFActorDemoApp");

            ShoppingItem si = new ShoppingItem { ShoppingItemCategory = indexModel.ShoppingCategory };
            sc.AddToCartAsync(si);

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
