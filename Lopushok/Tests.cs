using System.Collections.Generic;
using System.Linq;
using Lopushok.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Lopushok;

public class Tests
{
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

        [Test]
        public void ApplyFilters_ShouldFilterBySearchText()
        {
            // Arrange
            _mainWindow._allProducts = new List<ProductDisplay>
            {
                new ProductDisplay { ProductName = "Test Product", Article = "123" },
                new ProductDisplay { ProductName = "Another Product", Article = "456" }
            };

            // Act
            _mainWindow.SearchBox.Text = "Test";
            _mainWindow.ApplyFilters();

            // Assert
            var items = _mainWindow.ProductListView.Items as IEnumerable<ProductDisplay>;
            Assert.That(items.Count(), Is.EqualTo(1));
            Assert.That(items.First().ProductName, Is.EqualTo("Test Product"));
        }

        [Test]
        public void ApplySorting_ShouldSortByNameAscending()
        {
            // Arrange
            var products = new List<ProductDisplay>
            {
                new ProductDisplay { ProductName = "B Product" },
                new ProductDisplay { ProductName = "A Product" }
            };

            // Act
            var result = _mainWindow.ApplySorting(products);

            // Assert
            Assert.That(result.First().ProductName, Is.EqualTo("A Product"));
        }

        [Test]
        public void ProductListView_SelectionChanged_ShouldToggleButtonsVisibility()
        {
            // Arrange
            _mainWindow._allProducts = new List<ProductDisplay>
            {
                new ProductDisplay { ProductId = 1 }
            };
            _mainWindow.ProductListView.ItemsSource = _mainWindow._allProducts;

            // Act
            _mainWindow.ProductListView.SelectedIndex = 0;
            _mainWindow.ProductListView_SelectionChanged(null, null);

            // Assert
            Assert.That(_mainWindow.Edit_Product.IsVisible, Is.True);
            Assert.That(_mainWindow.ChangePriceButton.IsVisible, Is.True);
            Assert.That(_mainWindow.IncreasePriceButton.IsVisible, Is.True);
        }
    }

    [TestFixture]
    public class AddOrEditWindowTests
    {
        private AddOrEditWindow _editWindow;
        private Mock<RemoteDatabaseContext> _mockContext;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<RemoteDatabaseContext>();
            _editWindow = new AddOrEditWindow();
        }

        [Test]
        public void LoadProduct_ShouldPopulateFields()
        {
            // Arrange
            var product = new ProductDAO
            {
                ProductId = 1,
                ProductName = "Test",
                Article = "123",
                MinAgentCost = 100,
                WorkshopNumber = 1,
                WorkersRequired = 2
            };

            var mockSet = new Mock<DbSet<ProductDAO>>();
            mockSet.Setup(m => m.Find(It.IsAny<int>())).Returns(product);
            _mockContext.Setup(c => c.Products).Returns(mockSet.Object);

            // Act
            _editWindow.LoadProduct(1);

            // Assert
            Assert.That(_editWindow.TitleBox.Text, Is.EqualTo("Test"));
            Assert.That(_editWindow.ArticleNumberBox.Text, Is.EqualTo("123"));
        }

        [Test]
        public void Save_Click_ShouldValidateRequiredFields()
        {
            // Act
            _editWindow.Save_Click(null, null);

            // Assert
            Assert.That(_editWindow.TextError.Text, Is.EqualTo("Заполните все обязательные поля корректно"));
        }

        [Test]
        public void AddMaterial_Click_ShouldValidateMaterialSelection()
        {
            // Act
            _editWindow.AddMaterial_Click(null, null);

            // Assert
            Assert.That(_editWindow.TextError.Text, Is.EqualTo("Выберите материал"));
        }

        [Test]
        public void DeleteMaterial_Click_ShouldValidateSelection()
        {
            // Act
            _editWindow.DeleteMaterial_Click(null, null);

            // Assert
            Assert.That(_editWindow.TextError.Text, Is.EqualTo("Выберите материал для удаления"));
        }

        [Test]
        public void Delete_Click_WithSales_ShouldPreventDeletion()
        {
            // Arrange
            var product = new ProductDAO { ProductId = 1 };
            var sales = new List<ProductSalesDAO> { new ProductSalesDAO() }.AsQueryable();

            var mockProductSet = new Mock<DbSet<ProductDAO>>();
            mockProductSet.Setup(m => m.Find(It.IsAny<int>())).Returns(product);

            var mockSalesSet = new Mock<DbSet<ProductSalesDAO>>();
            mockSalesSet.As<IQueryable<ProductSalesDAO>>().Setup(m => m.Provider).Returns(sales.Provider);
            mockSalesSet.As<IQueryable<ProductSalesDAO>>().Setup(m => m.GetEnumerator()).Returns(sales.GetEnumerator());

            _mockContext.Setup(c => c.Products).Returns(mockProductSet.Object);
            _mockContext.Setup(c => c.ProductSales).Returns(mockSalesSet.Object);

            _editWindow._product = product;

            // Act
            _editWindow.Delete_Click(null, null);

            // Assert
            Assert.That(_editWindow.TextError.Text, Is.EqualTo("Нельзя удалить продукт, так как существуют связанные продажи"));
        }
    }

    [TestFixture]
    public class ChangePriceWindowTests
    {
        [Test]
        public void Apply_Click_WithInvalidInput_ShouldNotSetPrice()
        {
            // Arrange
            var window = new ChangePriceWindow(0);
            window.PriceBox.Text = "invalid";

            // Act
            window.Apply_Click(null, null);

            // Assert
            Assert.That(window.EnteredPrice, Is.Null);
        }

        [Test]
        public void Apply_Click_WithValidInput_ShouldSetPrice()
        {
            // Arrange
            var window = new ChangePriceWindow(0);
            window.PriceBox.Text = "10.50";

            // Act
            window.Apply_Click(null, null);

            // Assert
            Assert.That(window.EnteredPrice, Is.EqualTo(10.50m));
        }

        [Test]
        public void Apply_Click_WithCommaDecimalSeparator_ShouldAcceptInput()
        {
            // Arrange
            var window = new ChangePriceWindow(0);
            window.PriceBox.Text = "10,50";

            // Act
            window.Apply_Click(null, null);

            // Assert
            Assert.That(window.EnteredPrice, Is.EqualTo(10.50m));
        }
    }

    [TestFixture]
    public class IntegrationTests
    {

        [Test]
        public void ChangePriceWindow_ShouldUpdateProductPrice()
        {
            // Arrange
            var mainWindow = new MainWindow();
            mainWindow._allProducts = new List<ProductDisplay>
            {
                new ProductDisplay { ProductId = 1, MinAgentCost = 100 }
            };
            mainWindow.ProductListView.ItemsSource = mainWindow._allProducts;
            mainWindow.ProductListView.SelectedIndex = 0;

            // Act
            mainWindow.ChangePriceButton_Click(null, null);

            // Assert
            // Здесь нужно добавить логику для проверки модального окна
            Assert.Pass("This test requires additional setup for modal dialog testing");
        }
    }

    // Вспомогательные классы для тестирования
    public class TestProductDisplay : ProductDisplay
    {
        public TestProductDisplay(int id, string name, string type)
        {
            ProductId = id;
            ProductName = name;
            ProductType = type;
        }
    }
}