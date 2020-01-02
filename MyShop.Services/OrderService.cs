using MyShop.Core.Contracts;
using MyShop.Core.Modals;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services
{
    public class OrderService : IOrderService
    {
        IRepository<Order> orderContext;

        public OrderService(IRepository<Order> orderContext) 
        {
            this.orderContext = orderContext;
        }

        public void CreateOrder(Order baseOrder, List<BastetItemViewModel> basketItems)
        {
            foreach(var basketItem in basketItems)
            {
                baseOrder.OrderItems.Add(new OrderItem
                {
                    ProductId = basketItem.Id,
                    ProductName = basketItem.ProductName,
                    Price = basketItem.Price,
                    Image = basketItem.Image,
                    Quanity = basketItem.Quanity
                });

                orderContext.Insert(baseOrder);
                orderContext.Commit();
            }
        }
    }
}
