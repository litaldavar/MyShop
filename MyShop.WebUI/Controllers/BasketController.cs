using MyShop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        BasketService BasketService;

        public BasketController(BasketService basketService)
        {
            this.BasketService = basketService;
        }
        // GET: Basket
        public ActionResult Index()
        {
            var model = BasketService.GetBasketItems(this.HttpContext);
            return View(model);
        }

        public ActionResult AddToBasket(string Id)
        {
            BasketService.AddToBasket(this.HttpContext, Id);

            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromBasket(string Id)
        {
            BasketService.RemoveFromBasket(this.HttpContext, Id);

            return RedirectToAction("Index");
        }

        public PartialViewResult BasketSummary()
        {
            var BasketSummary = BasketService.GetBasketSammary(this.HttpContext);
            return PartialView(BasketSummary);
        }
    }
}