using MyShop.Core.Contracts;
using MyShop.Core.Modals;
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
        IRepository<Customer> customers;
        IBasketService basketService;
        IOrderService orderService;

        public BasketController(IBasketService basketService , IOrderService orderService, IRepository<Customer> customers)
        {
            this.basketService = basketService;
            this.orderService = orderService;
            this.customers = customers;
        }
        // GET: Basket
        public ActionResult Index()
        {
            var model = basketService.GetBasketItems(this.HttpContext);
            return View(model);
        }

        public ActionResult AddToBasket(string Id)
        {
            basketService.AddToBasket(this.HttpContext, Id);

            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromBasket(string Id)
        {
            basketService.RemoveFromBasket(this.HttpContext, Id);

            return RedirectToAction("Index");
        }

        public PartialViewResult BasketSummary()
        {
            var BasketSummary = basketService.GetBasketSammary(this.HttpContext);
            return PartialView(BasketSummary);
        }
        [Authorize]
        public ActionResult Checkout()
        {
            Customer customer = customers.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);

            if(customer != null)
            {
                Order order = new Order(){
                    Email = customer.Email,
                    City = customer.City,
                    FirstName = customer.FirstName,
                    Surname = customer.LastName,
                    Street = customer.Street,
                    State = customer.State,
                    ZipCode = customer.ZipCode
                };
                return View(order);
            }
            else
            {
                return RedirectToAction("Error");
            }
           
        }
        [HttpPost]
        [Authorize]
        public ActionResult Checkout(Order order)
        {
            var basketItems = basketService.GetBasketItems(this.HttpContext);

            order.OrderStatus = "Order Created";
            order.Email = User.Identity.Name;

            //Payment proccess

            order.OrderStatus = "Payment Proccessed";
            orderService.CreateOrder(order, basketItems);
            basketService.ClearBasket(this.HttpContext);

            return RedirectToAction("Thankyou", new { OrderId = order.Id });
        }

        public ActionResult ThankYou(string OrderId)
        {
            ViewBag.OrderId = OrderId;
            return View();
        }
    }
}