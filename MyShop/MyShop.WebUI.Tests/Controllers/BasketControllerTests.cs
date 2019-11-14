using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.Services;
using MyShop.WebUI.Controllers;
using MyShop.WebUI.Tests.Mocks;

namespace MyShop.WebUI.Tests.Controllers
{
    [TestClass]
    public class BasketControllerTests
    {
        [TestMethod]
        public void CanAddBasketItem()
        {
            // Arrange
            const string productId = "1";
            IRepository<Basket> basketContext = new MockContext<Basket>();
            IRepository<BasketItem> basketItemsContext = new MockContext<BasketItem>();
            IRepository<Product> productContext = new MockContext<Product>();
            MockHttpContext httpContext = new MockHttpContext();

            IBasketService basketService = new BasketService(productContext, basketContext, basketItemsContext);

            // Act
            basketService.AddToBasket(httpContext, productId);

            Basket basket = basketContext.Collection().FirstOrDefault();

            // Assert
            Assert.IsNotNull(basket);
            Assert.AreEqual(1, basket.BasketItems.Count);
            Assert.AreEqual(productId, basket.BasketItems.ToList().FirstOrDefault().ProductId);
        }

        [TestMethod]
        public void CanAddBasketItemController()
        {
            // Arrange
            const string productId = "1";
            IRepository<Basket> basketContext = new MockContext<Basket>();
            IRepository<BasketItem> basketItemsContext = new MockContext<BasketItem>();
            IRepository<Product> productContext = new MockContext<Product>();
            IRepository<Order> orderContext = new MockContext<Order>();
            MockHttpContext httpContext = new MockHttpContext();

            IBasketService basketService = new BasketService(productContext, basketContext, basketItemsContext);
            IOrderService orderService = new OrderService(orderContext);
            BasketController controller = new BasketController(basketService, orderService);
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            // Act
            controller.AddToBasket(productId);

            Basket basket = basketContext.Collection().FirstOrDefault();

            // Assert
            Assert.IsNotNull(basket);
            Assert.AreEqual(1, basket.BasketItems.Count);
            Assert.AreEqual(productId, basket.BasketItems.ToList().FirstOrDefault().ProductId);
        }

        [TestMethod]
        public void CanGetSummaryViewModel()
        {
            // Arrange
            IRepository<Product> productContext = new MockContext<Product>();
            productContext.Insert(new Product() { Id = "1", Price = 10.00m });
            productContext.Insert(new Product() { Id = "2", Price = 5.00m });

            IRepository<Basket> basketContext = new MockContext<Basket>();
            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2 });
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 1 });
            basketContext.Insert(basket);

            IRepository<BasketItem> basketItemsContext = new MockContext<BasketItem>();
            IRepository<Order> orderContext = new MockContext<Order>();

            IBasketService basketService = new BasketService(productContext, basketContext, basketItemsContext);
            IOrderService orderService = new OrderService(orderContext);
            BasketController controller = new BasketController(basketService, orderService);
            MockHttpContext httpContext = new MockHttpContext();
            httpContext.Response.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket") { Value = basket.Id });
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            // Act
            PartialViewResult result = controller.BasketSummary() as PartialViewResult;
            BasketSummaryViewModel basketSummary = (BasketSummaryViewModel)result.ViewData.Model;

            // Assert
            Assert.AreEqual(3, basketSummary.BasketCount);
            Assert.AreEqual(25.00m, basketSummary.BasketTotal);

        }

        [TestMethod]
        public void CanCheckoutAndCreateOrder()
        {
            // Arrange
            IRepository<Product> productContext = new MockContext<Product>();
            productContext.Insert(new Product() { Id = "1", Price = 10.00m, Name = "Product1" });
            productContext.Insert(new Product() { Id = "2", Price = 5.00m });

            IRepository<Basket> basketContext = new MockContext<Basket>();
            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2, BasketId = basket.Id });
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 1, BasketId = basket.Id });
            basketContext.Insert(basket);

            IRepository<BasketItem> basketItemsContext = new MockContext<BasketItem>();
            IRepository<Order> orderContext = new MockContext<Order>();

            IBasketService basketService = new BasketService(productContext, basketContext, basketItemsContext);
            IOrderService orderService = new OrderService(orderContext);
            BasketController controller = new BasketController(basketService, orderService);
            MockHttpContext httpContext = new MockHttpContext();
            httpContext.Response.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket") { Value = basket.Id });
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            // Act
            Order order = new Order();
            controller.Checkout(order);

            // Assert
            Assert.AreEqual(2, order.OrderItems.Count);
            Assert.AreEqual(0, basket.BasketItems.Count);

            Order orderInRep = orderContext.Find(order.Id);
            Assert.AreEqual(10.00m, orderInRep.OrderItems.ToList().FirstOrDefault().Price);
            Assert.AreEqual(2, orderInRep.OrderItems.ToList().FirstOrDefault().Quantity);
            Assert.AreEqual("Product1", orderInRep.OrderItems.ToList().FirstOrDefault().ProductName);

        }
    }
}
