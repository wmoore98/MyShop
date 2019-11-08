﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.WebUI;
using MyShop.WebUI.Controllers;
using MyShop.WebUI.Tests.Mocks;

namespace MyShop.WebUI.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void IndexPageDoesReturnProducts()
        {
            // Arrange
            IRepository<Product> productContext = new MockContext<Product>();
            IRepository<ProductCategory> productCategoryContext = new MockContext<ProductCategory>();
            productContext.Insert(new Product());
            HomeController controller = new HomeController(productContext, productCategoryContext);

            // Act
            ViewResult result = controller.Index() as ViewResult;
            ProductListViewModel viewModel = (ProductListViewModel)result.ViewData.Model;

            // Assert
            Assert.AreEqual(1, viewModel.Products.Count());

        }

    }
}
