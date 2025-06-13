using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Lopushok.Models;
using Microsoft.EntityFrameworkCore;

namespace Lopushok
{
    public partial class AddOrEditWindow : Window
    {
        public ProductDAO? _product;
        private int productID;
        public List<MaterialDAO> _allMaterials = new();
        private RemoteDatabaseContext _db = new();
        private string? _imagePath;
        public ObservableCollection<MaterialForList> _materials = new();

        public AddOrEditWindow()
        {
            InitializeComponent();
            MaterialListBox.ItemsSource = _materials;
            LoadTypes();
            LoadMaterials();
            Title = "Добавление продукта";
        }

        public AddOrEditWindow(int productId)
        {
            InitializeComponent();
            MaterialListBox.ItemsSource = _materials;
            LoadTypes();
            LoadMaterials();
            LoadProduct(productId);
            productID = productId;
            Title = "Редактирование продукта";
        }

        public void LoadProduct(int productId)
        {
            _product = _db.Products
                .Include(p => p.ProductType)
                .Include(p => p.ProductMaterials)
                    .ThenInclude(pm => pm.Material)
                .FirstOrDefault(p => p.ProductId == productId);

            if (_product == null) 
                return;

            TitleBox.Text = _product.ProductName;
            ArticleNumberBox.Text = _product.Article;
            MinCostBox.Text = _product.MinAgentCost.ToString();
            WorkshopNumberBox.Text = _product.WorkshopNumber.ToString();
            ProdPersonCountBox.Text = _product.WorkersRequired.ToString();
            ProductTypeBox.SelectedItem = _product.ProductType;

            if (!string.IsNullOrEmpty(_product.ImagePath))
            {
                var path = Path.Combine(AppContext.BaseDirectory, _product.ImagePath);
                if (File.Exists(path))
                {
                    ImagePreview.Source = new Bitmap(path);
                    _imagePath = _product.ImagePath;
                }
            }

            foreach (var material in _product.ProductMaterials)
            {
                _materials.Add(new MaterialForList
                {
                    MaterialId = material.MaterialId,
                    MaterialName = material.Material.MaterialName,
                    MaterialQuantity = material.RequiredQuantity
                });
            }
        }

        public void LoadTypes()
        {
            var productTypes = _db.ProductTypes.ToList();
            ProductTypeBox.ItemsSource = productTypes;
            ProductTypeBox.SelectedIndex = 0;
        }

        public void LoadMaterials()
        {
            _allMaterials = _db.Materials.ToList();
            MaterialBox.ItemsSource = _allMaterials.Select(m => m.MaterialName).ToList();
        }

        private async void SelectImage_Click(object? sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Images", Extensions = { "jpg", "png", "jpeg" } }
                },
                AllowMultiple = false
            };

            var result = await dialog.ShowAsync(this);
            var file = result?.FirstOrDefault();

            if (!string.IsNullOrEmpty(file))
            {
                try
                {
                    var fileName = Path.GetFileName(file);
                    var destPath = Path.Combine(AppContext.BaseDirectory, "Images", fileName);
                    
                    Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                    
                    if (File.Exists(destPath))
                        File.Delete(destPath);
                    
                    File.Copy(file, destPath);
                    
                    _imagePath = Path.Combine("Images", fileName);
                    ImagePreview.Source = new Bitmap(file);
                }
                catch (Exception ex)
                {
                    TextError.Text = $"Ошибка загрузки изображения: {ex.Message}";
                }
            }
        }

        public void AddMaterial_Click(object? sender, RoutedEventArgs routedEventArgs)
        {
            TextError.Text = "";

            if (!int.TryParse(MaterialQuantityBox.Text, out int quantity) || quantity <= 0)
            {
                TextError.Text = "Введите корректное значение кол-ва";
                return;
            }

            if (MaterialBox.SelectedItem == null)
            {
                TextError.Text = "Выберите материал";
                return;
            }

            string materialName = MaterialBox.SelectedItem.ToString()!;
            var material = _allMaterials.FirstOrDefault(m => m.MaterialName == materialName);

            if (material == null)
            {
                TextError.Text = "Выбран несуществующий материал";
                return;
            }

            if (_materials.Any(m => m.MaterialId == material.MaterialId))
            {
                TextError.Text = "Этот материал уже добавлен";
                return;
            }

            _materials.Add(new MaterialForList
            {
                MaterialId = material.MaterialId,
                MaterialName = materialName,
                MaterialQuantity = quantity
            });

            MaterialBox.SelectedItem = null;
            MaterialQuantityBox.Text = "";
        }

        public void DeleteMaterial_Click(object? sender, RoutedEventArgs e)
        {
            if (MaterialListBox.SelectedItem is not MaterialForList selected)
            {
                TextError.Text = "Выберите материал для удаления";
                return;
            }

            _materials.Remove(selected);
            TextError.Text = "";
        }

        public void Save_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                TextError.Text = "Заполните поле наименования продукта корректно";
                return;
            }

            if (string.IsNullOrWhiteSpace(ArticleNumberBox.Text) || !int.TryParse(ArticleNumberBox.Text, out int article))
            {
                TextError.Text = "Укажите корректный артикул";
                return;
            }
            
            if (ProductTypeBox.SelectedItem == null)
            {
                TextError.Text = "Укажите корректный тип продукта";
                return;
            }

            if (!decimal.TryParse(MinCostBox.Text, out decimal minCost) ||
                minCost <= 0)
            {
                TextError.Text = "Укажите корректную минимальную стоимость";
                return;
            }

            if (!int.TryParse(WorkshopNumberBox.Text, out int workshopNumber) || workshopNumber <= 0)
            {
                TextError.Text = "Укажите корректный номер цеха";
                return;
            }
            
            if (!int.TryParse(ProdPersonCountBox.Text, out int workersRequired) || workersRequired <= 0)
            {
                TextError.Text = "Укажите корректное кол-во сотрудников";
                return;
            }
        
            try
            {
                var productType = (ProductTypeDAO)ProductTypeBox.SelectedItem;
                if (_imagePath == null)
                {
                    _imagePath = "products/picture.png";
                }
        
                // Check for duplicate article number
                var duplicateArticle = _db.Products
                    .Any(p => p.Article == ArticleNumberBox.Text && 
                             (_product == null || p.ProductId != _product.ProductId));
                
                if (duplicateArticle)
                {
                    TextError.Text = "Продукт с таким артикулом уже существует";
                    return;
                }
        
                if (_product == null) // Adding new product
                {
                    _product = new ProductDAO
                    {
                        ProductId = _db.Products.Max(p => p.ProductId) + 1,
                        ProductName = TitleBox.Text,
                        Article = ArticleNumberBox.Text,
                        MinAgentCost = Math.Round(minCost, 2),
                        WorkshopNumber = workshopNumber,
                        WorkersRequired = workersRequired,
                        ProductTypeId = productType.Id,
                        ImagePath = _imagePath
                    };
                    
                    _db.Products.Add(_product);
                    _db.SaveChanges(); // Save first to get the ProductId
                    
                    // Now add materials after we have the ID
                    foreach (var material in _materials)
                    {
                        _db.ProductMaterials.Add(new ProductMaterialDAO
                        {
                            ProductMaterialId = _db.ProductMaterials.Max(p => p.ProductMaterialId) + 1,
                            ProductId = _product.ProductId,
                            MaterialId = material.MaterialId,
                            RequiredQuantity = material.MaterialQuantity
                        });
                    }
                }
                else // Editing existing product
                {
                    _product.ProductName = TitleBox.Text;
                    _product.Article = ArticleNumberBox.Text;
                    _product.MinAgentCost = Math.Round(minCost, 2);
                    _product.WorkshopNumber = workshopNumber;
                    _product.WorkersRequired = workersRequired;
                    _product.ProductTypeId = productType.Id;
                    _product.ImagePath = _imagePath ?? _product.ImagePath;
                    
                    // Remove existing materials
                    var existingMaterials = _db.ProductMaterials
                        .Where(pm => pm.ProductId == _product.ProductId)
                        .ToList();
                    _db.ProductMaterials.RemoveRange(existingMaterials);
                    
                    // Add new materials
                    foreach (var material in _materials)
                    {
                        _db.ProductMaterials.Add(new ProductMaterialDAO
                        {
                            ProductId = _product.ProductId,
                            MaterialId = material.MaterialId,
                            RequiredQuantity = material.MaterialQuantity
                        });
                    }
                }
                
                _db.SaveChanges();
                TextError.Text = "Данные успешно сохранены!";
                Close();
            }
            catch (DbUpdateException dbEx)
            {
                TextError.Text = $"Ошибка базы данных: {dbEx.InnerException?.Message ?? dbEx.Message}";
            }
            catch (Exception ex)
            {
                TextError.Text = $"Ошибка при сохранении: {ex.Message}";
            }
        }


        private void Cancel_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        public void MaterialSearchBox_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            string search = MaterialSearchBox.Text?.ToLower() ?? "";

            var filtered = _allMaterials
                .Where(m => m.MaterialName.ToLower().Contains(search))
                .Select(m => m.MaterialName)
                .ToList();

            MaterialBox.ItemsSource = filtered;
            MaterialBox.IsDropDownOpen = true;

            if (filtered.Count == 1)
                MaterialBox.SelectedItem = filtered[0];
        }

        public void Delete_Click(object? sender, RoutedEventArgs e)
        {
            // Находим продукт вместе с его материалами и продажами
            var productForDelete = _db.Products
                .Include(p => p.ProductMaterials)
                .Include(p => p.ProductSales) // Добавляем загрузку связанных продаж
                .FirstOrDefault(p => p.ProductId == _product.ProductId);

            if (productForDelete == null)
                return;

            // Проверяем, есть ли связанные продажи
            if (productForDelete.ProductSales != null && productForDelete.ProductSales.Any())
            {
                // Если есть продажи - показываем сообщение и запрещаем удаление
                TextError.Text = "Нельзя удалить продукт, так как существуют связанные продажи";
                return;
            }

            try
            {
                // Если продаж нет - удаляем материалы и сам продукт
                _db.ProductMaterials.RemoveRange(productForDelete.ProductMaterials);
                _db.Products.Remove(productForDelete);
                _db.SaveChanges();
                Close();
            }
            catch (Exception ex)
            {
                TextError.Text = $"Ошибка при удалении: {ex.Message}";
            }
        }
    }
}