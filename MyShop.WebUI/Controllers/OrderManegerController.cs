using MyShop.Core.Contracts;
using MyShop.Core.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class OrderManegerController : Controller
    {
        IOrderService orderService;

        public OrderManegerController(IOrderService OrderService)
        {
            this.orderService = OrderService;
        }
        // GET: OrderManeger
        public ActionResult Index()
        {
            List<Order> orders = orderService.GetOrderList();
            return View(orders);
        }

        public ActionResult UpdateOrder(string Id)
        {
            ViewBag.StatusList = new List<string>()
           {
               "Order Created",
               "Payment Proccessed",
               "Order Shipped",
               "OrderComplete"
           };

           Order order = orderService.GetOrder(Id);
            return View(order);            
        }

        [HttpPost]
        public ActionResult UpdateOrder(Order UpdatedOrder , string Id)
        {
            Order order = orderService.GetOrder(Id);

            order.OrderStatus = UpdatedOrder.OrderStatus;
            orderService.UpdateOrder(order);

            return RedirectToAction("Index");
        }

    }
}