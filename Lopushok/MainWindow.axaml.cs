using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Lopushok.Models;
using Microsoft.EntityFrameworkCore;

namespace Lopushok
{
    public partial class MainWindow : Window
    {
        private const int PageSize = 20;
        private int _currentPage = 1;
        private int _totalPages = 1;
        public List<ProductDisplay> _allProducts = new();
        private static readonly RemoteDatabaseContext _db = new();

        public MainWindow()
        {
            InitializeComponent();
            Resources.Add("BoolToRedConverter", new BoolToRedConverter());
            LoadData();
        }

        public void LoadData()
        {
            var oneMonthAgo = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));

            var products = _db.Products
                .Include(p => p.ProductType)
                .Include(p => p.ProductMaterials).ThenInclude(pm => pm.Material)
                .Select(p => new ProductDisplay
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductType = _db.ProductTypes.Where(pt => pt.Id == p.ProductTypeId).Select(x=>x.ProductType).FirstOrDefault(),
                    Article = p.Article,
                    WorkshopNumber = p.WorkshopNumber,
                    MinAgentCost = p.ProductMaterials
                        .Where(pm => pm.Material != null && pm.Material.Cost != null)
                        .Sum(pm => pm.Material.Cost * pm.RequiredQuantity) > 0 
                        ? p.ProductMaterials.Sum(pm => pm.Material.Cost * pm.RequiredQuantity) 
                        : p.MinAgentCost,
                    ImagePath = p.ImagePath,
                    LastSaleDate = _db.ProductSales
                        .Where(s => s.product_id == p.ProductId)
                        .OrderByDescending(s => s.sale_date)
                        .Select(s => (DateOnly?)s.sale_date)
                        .FirstOrDefault(),
                    IsStale = !_db.ProductSales
                        .Any(s => s.product_id == p.ProductId && s.sale_date >= oneMonthAgo),
                    Cost = p.ProductMaterials.Sum(pm => pm.Material.Cost * pm.RequiredQuantity)
                })
                .AsNoTracking()
                .ToList();

            _allProducts = products;
            InitFilters();
            ApplyFilters();
        }

        private void InitFilters()
        {
            // Инициализация сортировок (оставляем как есть)
            NameSortBox.ItemsSource = new List<ComboBoxItem>
            {
                new ComboBoxItem { Content = "Без сортировки", Tag = "None" },
                new ComboBoxItem { Content = "Наименование ↑", Tag = "NameAsc" },
                new ComboBoxItem { Content = "Наименование ↓", Tag = "NameDesc" }
            };
            NumWorkSortBox.ItemsSource = new List<ComboBoxItem>
            {
                new ComboBoxItem { Content = "Без сортировки", Tag = "None" },
                new ComboBoxItem { Content = "Цех ↑", Tag = "WorkshopAsc" },
                new ComboBoxItem { Content = "Цех ↓", Tag = "WorkshopDesc" }
            };
            MinCostBox.ItemsSource = new List<ComboBoxItem>
            {
                new ComboBoxItem { Content = "Без сортировки", Tag = "None" },
                new ComboBoxItem { Content = "Цена ↑", Tag = "PriceAsc" },
                new ComboBoxItem { Content = "Цена ↓", Tag = "PriceDesc" }
            };

            NameSortBox.SelectedIndex = 0;
            NumWorkSortBox.SelectedIndex = 0;
            MinCostBox.SelectedIndex = 0;

            var productTypes = _db.ProductTypes
                .OrderBy(pt => pt.ProductType)
                .Select(pt => pt.ProductType)
                .ToList();

            TypeFilterBox.ItemsSource = new[] { "Все типы" }.Concat(productTypes).ToList();
            TypeFilterBox.SelectedIndex = 0;
        }

        public void ApplyFilters()
        {
            IEnumerable<ProductDisplay> query = _allProducts;

            var search = SearchBox.Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.WorkshopNumber.ToString().Contains(search) ||
                    p.ProductType.Contains(search) ||
                    p.MinAgentCost.ToString().Contains(search) ||
                    p.ProductName.ToLower().Contains(search) ||
                    p.Article.ToLower().Contains(search));
            }

            if (TypeFilterBox.SelectedIndex > 0 && TypeFilterBox.SelectedItem is string selectedType)
            {
                query = query.Where(p => p.ProductType == selectedType);
                NameSortBox.SelectedIndex = 0;
                NumWorkSortBox.SelectedIndex = 0;
                MinCostBox.SelectedIndex = 0;

            }

            query = ApplySorting(query);

            _totalPages = Math.Max(1, (int)Math.Ceiling(query.Count() / (double)PageSize));
            _currentPage = Math.Clamp(_currentPage, 1, _totalPages);

            ProductListView.ItemsSource = query
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            RenderPageButtons();
        }

        public IEnumerable<ProductDisplay> ApplySorting(IEnumerable<ProductDisplay> query)
        {
            string GetTag(ComboBox box) => (box.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "None";

            string workshopTag = GetTag(NumWorkSortBox);
            string priceTag = GetTag(MinCostBox);
            string nameTag = GetTag(NameSortBox);

            IOrderedEnumerable<ProductDisplay>? orderedQuery = null;

            if (workshopTag != "None")
            {
                orderedQuery = orderedQuery switch
                {
                    not null => workshopTag switch
                    {
                        "WorkshopAsc" => orderedQuery.ThenBy(p => p.WorkshopNumber),
                        "WorkshopDesc" => orderedQuery.ThenByDescending(p => p.WorkshopNumber),
                        _ => orderedQuery
                    },
                    null => workshopTag switch
                    {
                        "WorkshopAsc" => query.OrderBy(p => p.WorkshopNumber),
                        "WorkshopDesc" => query.OrderByDescending(p => p.WorkshopNumber),
                        _ => null
                    }
                };
            }

            if (priceTag != "None")
            {
                orderedQuery = orderedQuery switch
                {
                    not null => priceTag switch
                    {
                        "PriceAsc" => orderedQuery.ThenBy(p => p.MinAgentCost),
                        "PriceDesc" => orderedQuery.ThenByDescending(p => p.MinAgentCost),
                        _ => orderedQuery
                    },
                    null => priceTag switch
                    {
                        "PriceAsc" => query.OrderBy(p => p.MinAgentCost),
                        "PriceDesc" => query.OrderByDescending(p => p.MinAgentCost),
                        _ => null
                    }
                };
            }

            if (nameTag != "None")
            {
                orderedQuery = orderedQuery switch
                {
                    not null => nameTag switch
                    {
                        "NameAsc" => orderedQuery.ThenBy(p => p.ProductName),
                        "NameDesc" => orderedQuery.ThenByDescending(p => p.ProductName),
                        _ => orderedQuery
                    },
                    null => nameTag switch
                    {
                        "NameAsc" => query.OrderBy(p => p.ProductName),
                        "NameDesc" => query.OrderByDescending(p => p.ProductName),
                        _ => null
                    }
                };
            }

            return orderedQuery ?? query;
        }

        private void RenderPageButtons()
        {
            PageButtonsPanel.Children.Clear();

            var prevBtn = new Button
            {
                Content = "Назад",
                IsEnabled = _currentPage > 1,
                Margin = new Thickness(5)
            };
            prevBtn.Click += (_, _) =>
            {
                _currentPage--;
                ApplyFilters();
            };
            PageButtonsPanel.Children.Add(prevBtn);

            for (int i = 1; i <= _totalPages; i++)
            {
                var page = i;
                var btn = new Button
                {
                    Content = i.ToString(),
                    Margin = new Thickness(5),
                    IsEnabled = _currentPage != i
                };
                btn.Click += (_, _) =>
                {
                    _currentPage = page;
                    ApplyFilters();
                };
                PageButtonsPanel.Children.Add(btn);
            }

            var nextBtn = new Button
            {
                Content = "Вперёд",
                IsEnabled = _currentPage < _totalPages,
                Margin = new Thickness(5)
            };
            nextBtn.Click += (_, _) =>
            {
                _currentPage++;
                ApplyFilters();
            };
            PageButtonsPanel.Children.Add(nextBtn);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void FilterChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        public void ProductListView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = ProductListView.SelectedItems?.Count > 0;
            Edit_Product.IsVisible = hasSelection;
            ChangePriceButton.IsVisible = hasSelection;
            IncreasePriceButton.IsVisible = hasSelection;
        }

        public async void ChangePriceButton_Click(object? sender, RoutedEventArgs e)
        {
            var selected = ProductListView.SelectedItems.Cast<ProductDisplay>().ToList();
            if (selected.Count == 0) return;

            var avg = selected.Average(p => p.MinAgentCost ?? 0);
            var dialog = new ChangePriceWindow(avg);
            var result = await dialog.ShowDialog<decimal?>(this);
            if (result.HasValue)
            {
                var productIds = selected.Select(p => p.ProductId).ToList();
                var productsToUpdate = _db.Products
                    .Where(p => productIds.Contains(p.ProductId))
                    .ToList();

                foreach (var product in productsToUpdate)
                {
                    product.MinAgentCost = result.Value;
                }

                await _db.SaveChangesAsync();
                LoadData();
            }
        }

        private async void IncreasePriceButton_Click(object? sender, RoutedEventArgs e)
        {
            var selected = ProductListView.SelectedItems.Cast<ProductDisplay>().ToList();
            if (selected.Count == 0) return;

            var dialog = new ChangePriceWindow(0);
            var result = await dialog.ShowDialog<decimal?>(this);
            if (result.HasValue)
            {
                var productIds = selected.Select(p => p.ProductId).ToList();
                var productsToUpdate = _db.Products
                    .Where(p => productIds.Contains(p.ProductId))
                    .ToList();

                foreach (var product in productsToUpdate)
                {
                    product.MinAgentCost = (product.MinAgentCost ?? 0) + result.Value;
                }

                await _db.SaveChangesAsync();
                LoadData();
            }
        }

        private void Add_Product_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = new AddOrEditWindow();
            window.Closed += (s, e) => LoadData();
            window.Show();
        }

        public void Edit_Product_OnClick(object? sender, RoutedEventArgs e)
        {
            if (ProductListView.SelectedItems == null || ProductListView.SelectedItems.Count != 1)
                return;

            if (ProductListView.SelectedItem is not ProductDisplay selectedDisplay)
                return;

            var window = new AddOrEditWindow(selectedDisplay.ProductId);
            window.Closed += (s, e) => LoadData();
            window.Show();
        }
        
        private void ResetFilters_Click(object? sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            NameSortBox.SelectedIndex = 0;
            NumWorkSortBox.SelectedIndex = 0;
            MinCostBox.SelectedIndex = 0;
            TypeFilterBox.SelectedIndex = 0;
            _currentPage = 1;
            ApplyFilters();
        }
    }
}