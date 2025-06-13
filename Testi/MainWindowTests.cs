using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Lopushok;
using Lopushok.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Testi
{
    [TestFixture]
    public class MainWindowTests
    {
        private RemoteDatabaseContext _dbContext;
        private MainWindow _mainWindow;
        
        [OneTimeSetUp]
        public void InitAvalonia()
        {
            try
            {
                AppBuilder.Configure<App>()
                    .UsePlatformDetect()
                    .UseHeadless(new AvaloniaHeadlessPlatformOptions())
                    .SetupWithoutStarting();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to initialize Avalonia: {ex}");
            }
        }

        [SetUp]
        public void Setup()
        {
            _dbContext = new RemoteDatabaseContext();
            _mainWindow = new MainWindow();
            
            // Инициализация необходимых свойств
            _mainWindow.SearchBox = new TextBox();
            _mainWindow.TypeFilterBox = new ComboBox();
            _mainWindow.NameSortBox = new ComboBox();
            _mainWindow.NumWorkSortBox = new ComboBox();
            _mainWindow.MinCostBox = new ComboBox();
            _mainWindow.ProductListView = new ListBox();
            _mainWindow.PageButtonsPanel = new StackPanel();
            _mainWindow.Edit_Product = new Button();
            _mainWindow.ChangePriceButton = new Button();
            _mainWindow.IncreasePriceButton = new Button();
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext?.Dispose();
        }

        [Test]
        public void ApplyFilters_ShouldFilterBySearchText()
        {
            // Arrange
            _mainWindow._allProducts = new List<ProductDisplay>
            {
                new() { 
                    ProductName = "Тестовый продукт 1", 
                    Article = "TEST001",
                    ProductType = "Тип 1",
                    WorkshopNumber = 1,
                    MinAgentCost = 100
                },
                new() { 
                    ProductName = "Другой продукт", 
                    Article = "TEST002",
                    ProductType = "Тип 2",
                    WorkshopNumber = 2,
                    MinAgentCost = 200
                }
            };
            _mainWindow.SearchBox.Text = "тестовый";

            // Act
            _mainWindow.ApplyFilters();
            var result = _mainWindow.ProductListView.ItemsSource as IEnumerable<ProductDisplay>;

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().ProductName, Is.EqualTo("Тестовый продукт 1"));
        }

        [Test]
        public void ApplySorting_ShouldSortByNameAscending()
        {
            // Arrange
            var products = new List<ProductDisplay>
            {
                new() { ProductName = "B Продукт", Article = "TEST002" },
                new() { ProductName = "A Продукт", Article = "TEST001" },
                new() { ProductName = "C Продукт", Article = "TEST003" }
            };
            
            _mainWindow.NameSortBox.ItemsSource = new List<ComboBoxItem>
            {
                new() { Content = "Без сортировки", Tag = "None" },
                new() { Content = "Наименование ↑", Tag = "NameAsc" },
                new() { Content = "Наименование ↓", Tag = "NameDesc" }
            };
            _mainWindow.NameSortBox.SelectedItem = _mainWindow.NameSortBox.ItemsSource
                .Cast<ComboBoxItem>()
                .First(x => x.Tag.ToString() == "NameAsc");

            // Act
            var result = _mainWindow.ApplySorting(products).ToList();

            // Assert
            Assert.That(result[0].ProductName, Is.EqualTo("A Продукт"));
            Assert.That(result[1].ProductName, Is.EqualTo("B Продукт"));
            Assert.That(result[2].ProductName, Is.EqualTo("C Продукт"));
        }

        [Test]
        public void ApplySorting_ShouldSortByWorkshopNumberDescending()
        {
            // Arrange
            var products = new List<ProductDisplay>
            {
                new() { ProductName = "Продукт 1", WorkshopNumber = 1 },
                new() { ProductName = "Продукт 2", WorkshopNumber = 3 },
                new() { ProductName = "Продукт 3", WorkshopNumber = 2 }
            };
            
            _mainWindow.NumWorkSortBox.ItemsSource = new List<ComboBoxItem>
            {
                new() { Content = "Без сортировки", Tag = "None" },
                new() { Content = "Цех ↑", Tag = "WorkshopAsc" },
                new() { Content = "Цех ↓", Tag = "WorkshopDesc" }
            };
            _mainWindow.NumWorkSortBox.SelectedItem = _mainWindow.NumWorkSortBox.ItemsSource
                .Cast<ComboBoxItem>()
                .First(x => x.Tag.ToString() == "WorkshopDesc");

            // Act
            var result = _mainWindow.ApplySorting(products).ToList();

            // Assert
            Assert.That(result[0].WorkshopNumber, Is.EqualTo(3));
            Assert.That(result[1].WorkshopNumber, Is.EqualTo(2));
            Assert.That(result[2].WorkshopNumber, Is.EqualTo(1));
        }

        [Test]
        public void ApplySorting_ShouldSortByPriceAscending()
        {
            // Arrange
            var products = new List<ProductDisplay>
            {
                new() { ProductName = "Продукт 1", MinAgentCost = 300 },
                new() { ProductName = "Продукт 2", MinAgentCost = 100 },
                new() { ProductName = "Продукт 3", MinAgentCost = 200 }
            };
            
            _mainWindow.MinCostBox.ItemsSource = new List<ComboBoxItem>
            {
                new() { Content = "Без сортировки", Tag = "None" },
                new() { Content = "Цена ↑", Tag = "PriceAsc" },
                new() { Content = "Цена ↓", Tag = "PriceDesc" }
            };
            _mainWindow.MinCostBox.SelectedItem = _mainWindow.MinCostBox.ItemsSource
                .Cast<ComboBoxItem>()
                .First(x => x.Tag.ToString() == "PriceAsc");

            // Act
            var result = _mainWindow.ApplySorting(products).ToList();

            // Assert
            Assert.That(result[0].MinAgentCost, Is.EqualTo(100));
            Assert.That(result[1].MinAgentCost, Is.EqualTo(200));
            Assert.That(result[2].MinAgentCost, Is.EqualTo(300));
        }

        [Test]
        public void ApplySorting_ShouldApplyMultipleSorts()
        {
            // Arrange
            var products = new List<ProductDisplay>
            {
                new() { ProductName = "Продукт 1", WorkshopNumber = 1, MinAgentCost = 100 },
                new() { ProductName = "Продукт 2", WorkshopNumber = 1, MinAgentCost = 200 },
                new() { ProductName = "Продукт 3", WorkshopNumber = 2, MinAgentCost = 100 }
            };
            
            _mainWindow.NumWorkSortBox.ItemsSource = new List<ComboBoxItem>
            {
                new() { Content = "Без сортировки", Tag = "None" },
                new() { Content = "Цех ↑", Tag = "WorkshopAsc" },
                new() { Content = "Цех ↓", Tag = "WorkshopDesc" }
            };
            _mainWindow.MinCostBox.ItemsSource = new List<ComboBoxItem>
            {
                new() { Content = "Без сортировки", Tag = "None" },
                new() { Content = "Цена ↑", Tag = "PriceAsc" },
                new() { Content = "Цена ↓", Tag = "PriceDesc" }
            };
            
            _mainWindow.NumWorkSortBox.SelectedItem = _mainWindow.NumWorkSortBox.ItemsSource
                .Cast<ComboBoxItem>()
                .First(x => x.Tag.ToString() == "WorkshopAsc");
            _mainWindow.MinCostBox.SelectedItem = _mainWindow.MinCostBox.ItemsSource
                .Cast<ComboBoxItem>()
                .First(x => x.Tag.ToString() == "PriceDesc");

            // Act
            var result = _mainWindow.ApplySorting(products).ToList();

            // Assert
            Assert.That(result[0].WorkshopNumber, Is.EqualTo(1));
            Assert.That(result[0].MinAgentCost, Is.EqualTo(200));
            Assert.That(result[1].WorkshopNumber, Is.EqualTo(1));
            Assert.That(result[1].MinAgentCost, Is.EqualTo(100));
            Assert.That(result[2].WorkshopNumber, Is.EqualTo(2));
        }

        [Test]
        public void RenderPageButtons_ShouldCreateCorrectButtons()
        {
            // Arrange
            _mainWindow._currentPage = 2;
            _mainWindow._totalPages = 3;
            _mainWindow._allProducts = Enumerable.Range(1, 50)
                .Select(i => new ProductDisplay { ProductName = $"Продукт {i}" })
                .ToList();

            // Act
            _mainWindow.RenderPageButtons();

            // Assert
            Assert.That(_mainWindow.PageButtonsPanel.Children.Count, Is.EqualTo(5)); // Назад + 3 страницы + Вперёд
            
            var buttons = _mainWindow.PageButtonsPanel.Children
                .OfType<Button>()
                .ToList();
            
            Assert.That(buttons[0].Content, Is.EqualTo("Назад"));
            Assert.That(buttons[1].Content, Is.EqualTo("1"));
            Assert.That(buttons[2].Content, Is.EqualTo("2"));
            Assert.That(buttons[3].Content, Is.EqualTo("3"));
            Assert.That(buttons[4].Content, Is.EqualTo("Вперёд"));
            
            Assert.That(buttons[2].IsEnabled, Is.False); // Текущая страница должна быть disabled
        }

        [Test]
        public void ProductListView_SelectionChanged_ShouldToggleButtonsVisibility()
        {
            // Arrange
            _mainWindow._allProducts = new List<ProductDisplay>
            {
                new() { ProductName = "Продукт 1" },
                new() { ProductName = "Продукт 2" }
            };
            _mainWindow.ApplyFilters();

            // Act - select one item
            _mainWindow.ProductListView.SelectedItems.Add(_mainWindow.ProductListView.Items[0]);
            _mainWindow.ProductListView_SelectionChanged(null, null);

            // Assert
            Assert.That(_mainWindow.Edit_Product.IsVisible, Is.True);
            Assert.That(_mainWindow.ChangePriceButton.IsVisible, Is.True);
            Assert.That(_mainWindow.IncreasePriceButton.IsVisible, Is.True);

            // Act - clear selection
            _mainWindow.ProductListView.SelectedItems.Clear();
            _mainWindow.ProductListView_SelectionChanged(null, null);

            // Assert
            Assert.That(_mainWindow.Edit_Product.IsVisible, Is.False);
            Assert.That(_mainWindow.ChangePriceButton.IsVisible, Is.False);
            Assert.That(_mainWindow.IncreasePriceButton.IsVisible, Is.False);
        }
    }
}