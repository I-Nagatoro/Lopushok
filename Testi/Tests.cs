using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Lopushok;
using Lopushok.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Lopushok.Tests
{
    [SetUpFixture]
    public class AvaloniaTestInitializer
    {
        [OneTimeSetUp]
        public void InitializeAvalonia()
        {
            AppBuilder.Configure<App>()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions())
                .SetupWithoutStarting();
        }
    }

    [TestFixture]
    public class MainWindowTests
    {
        private MainWindow _mainWindow;
        private Mock<RemoteDatabaseContext> _mockContext;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<RemoteDatabaseContext>();
            _mainWindow = new MainWindow();
            
            // Инициализация UI-элементов
            _mainWindow.SearchBox = new TextBox();
            _mainWindow.ProductListView = new ListBox();
            _mainWindow.Edit_Product = new Button { IsVisible = false };
            _mainWindow.ChangePriceButton = new Button { IsVisible = false };
            _mainWindow.IncreasePriceButton = new Button { IsVisible = false };
        }

        [Test]
        public void LoadData_ShouldPopulateAllProducts()
        {
            // Arrange
            var products = new List<ProductDAO>
            {
                new ProductDAO { ProductId = 1, ProductName = "Test Product" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<ProductDAO>>();
            mockSet.As<IQueryable<ProductDAO>>().Setup(m => m.Provider).Returns(products.Provider);
            mockSet.As<IQueryable<ProductDAO>>().Setup(m => m.Expression).Returns(products.Expression);
            mockSet.As<IQueryable<ProductDAO>>().Setup(m => m.ElementType).Returns(products.ElementType);
            mockSet.As<IQueryable<ProductDAO>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());

            _mockContext.Setup(c => c.Products).Returns(mockSet.Object);

            // Act
            _mainWindow.LoadData();

            // Assert
            Assert.That(_mainWindow._allProducts, Is.Not.Null);
            Assert.That(_mainWindow._allProducts.Count, Is.EqualTo(1));
        }

        // Остальные тесты остаются без изменений
        // ...
    }

    // Остальные тестовые классы (AddOrEditWindowTests, ChangePriceWindowTests и т.д.)
    // ...
}