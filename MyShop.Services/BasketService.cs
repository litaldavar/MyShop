using MyShop.Core.Contracts;
using MyShop.Core.Modals;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class BasketService : IBasketService
    {
        IRepository<Product> productContext;
        IRepository<Basket> basketContext;

        public const string BasketSessionName = "eCommerceBasket";

        public BasketService(IRepository<Product> ProductContext, IRepository<Basket> BasketContext)
        {
            this.productContext = ProductContext;
            this.basketContext = BasketContext;
        }

            private Basket GetBasket(HttpContextBase httpContext , bool createIfNull)
            {
                HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);

                Basket basket = new Basket();

                if(cookie != null)
                {
                    string basketId = cookie.Value;
                    if (!string.IsNullOrEmpty(basketId))
                    {
                        basket = basketContext.Find(basketId);
                    }
                    else
                    {
                        if (createIfNull)
                        {
                            basket = createNewBasket(httpContext);
                        }
                    }
                }
                else
                {
                    if (createIfNull)
                    {
                        basket = createNewBasket(httpContext);
                    }
                }

                return basket;
            }

           private Basket createNewBasket(HttpContextBase httpContext)
        {
            Basket basket = new Basket();
            basketContext.Insert(basket);
            basketContext.Commit();

            HttpCookie cookie = new HttpCookie(BasketSessionName);
            cookie.Value = basket.Id;
            cookie.Expires = DateTime.Now.AddDays(1);

            httpContext.Response.Cookies.Add(cookie);
            return basket;
        }

        public void AddToBasket(HttpContextBase httpContext , string productId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);

            if(item == null)
            {
                item = new BasketItem()
                {
                    BasketId = basket.Id,
                    ProductId = productId,
                    Quanity = 1
                };

                basket.BasketItems.Add(item);
            }
            else
            {
                item.Quanity += 1;
            }

            basketContext.Commit();
        }

        public void RemoveFromBasket(HttpContextBase httpContext , string itemId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if(item != null)
            {
                basket.BasketItems.Remove(item);
                basketContext.Commit();
            }

        }

        public List<BastetItemViewModel> GetBasketItems(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);

            if(basket != null)
            {
                var result = (from b in basket.BasketItems
                              join p in productContext.Collection() on b.ProductId equals p.Id
                              select new BastetItemViewModel()
                              {
                                  Id = b.Id,
                                  Quanity = b.Quanity,
                                  ProductName = p.Name,
                                  Price = p.Price,
                                  Image = p.Image
                              }).ToList();
                return result;
            }
            else
            {
                return new List<BastetItemViewModel>();
            }
        }

        public BasketSummaryViewModel GetBasketSammary(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);
            BasketSummaryViewModel model = new BasketSummaryViewModel(0, 0);

            if (basket != null)
            {
                int? itemCount = (from item in basket.BasketItems
                                  select item.Quanity).Sum();

                decimal? basketTotal = (from b in basket.BasketItems
                                        join p in productContext.Collection() on b.ProductId equals p.Id
                                        select b.Quanity * p.Price).Sum();

                model.BasketCount = itemCount ?? 0;
                model.BasketTotal = basketTotal ?? decimal.Zero;

                return model;
            }
            else
            {
                return model;
            }
        }

        public void ClearBasket(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);

            basket.BasketItems.Clear();
            basketContext.Commit();
        }
    }
       
   
}
