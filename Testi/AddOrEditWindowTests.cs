using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Lopushok;
using Lopushok.Models;
using NUnit.Framework;

namespace Testi;

[TestFixture]
public class AddMaterialTests
{
    private AddOrEditWindow _window;
    private RemoteDatabaseContext _dbContext;
    private MaterialDAO _testMaterial;
    
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
        _window = new AddOrEditWindow();
        
        _window.TextError = new TextBlock();
        _window.MaterialBox = new ComboBox();
        _window.MaterialQuantityBox = new TextBox();
        _window.MaterialListBox = new ListBox();
        _window._materials = new ObservableCollection<MaterialForList>();
        _window.MaterialListBox.ItemsSource = _window._materials;
        
        _testMaterial = _dbContext.Materials.First();
        _window._allMaterials = _dbContext.Materials.ToList();
        _window.MaterialBox.ItemsSource = _window._allMaterials.Select(m => m.MaterialName).ToList();
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Dispose();
    }

    [Test]
    public void AddMaterial_Click_WithValidData_ShouldAddMaterial()
    {
        _window.MaterialBox.SelectedItem = _testMaterial.MaterialName;
        _window.MaterialQuantityBox.Text = "5";

        _window.AddMaterial_Click(null, null);

        Assert.That(_window._materials.Count, Is.EqualTo(1));
        var addedMaterial = _window._materials.First();
        Assert.That(addedMaterial.MaterialId, Is.EqualTo(_testMaterial.MaterialId));
        Assert.That(addedMaterial.MaterialName, Is.EqualTo(_testMaterial.MaterialName));
        Assert.That(addedMaterial.MaterialQuantity, Is.EqualTo(5));
        Assert.That(_window.TextError.Text, Is.Empty);
        Assert.That(_window.MaterialBox.SelectedItem, Is.Null);
        Assert.That(_window.MaterialQuantityBox.Text, Is.Empty);
    }

    [Test]
    public void AddMaterial_Click_WithInvalidQuantity_ShouldShowError()
    {
        _window.MaterialBox.SelectedItem = _testMaterial.MaterialName;
        _window.MaterialQuantityBox.Text = "0"; // Некорректное количество

        _window.AddMaterial_Click(null, null);

        Assert.That(_window._materials.Count, Is.EqualTo(0));
        Assert.That(_window.TextError.Text, Is.EqualTo("Введите корректное значение кол-ва"));
    }

    [Test]
    public void AddMaterial_Click_WithNegativeQuantity_ShouldShowError()
    {
        _window.MaterialBox.SelectedItem = _testMaterial.MaterialName;
        _window.MaterialQuantityBox.Text = "-5";

        _window.AddMaterial_Click(null, null);

        Assert.That(_window._materials.Count, Is.EqualTo(0));
        Assert.That(_window.TextError.Text, Is.EqualTo("Введите корректное значение кол-ва"));
    }

    [Test]
    public void AddMaterial_Click_WithoutSelectedMaterial_ShouldShowError()
    {
        _window.MaterialBox.SelectedItem = null;
        _window.MaterialQuantityBox.Text = "5";

        _window.AddMaterial_Click(null, null);

        Assert.That(_window._materials.Count, Is.EqualTo(0));
        Assert.That(_window.TextError.Text, Is.EqualTo("Выберите материал"));
    }

    [Test]
    public void AddMaterial_Click_WithDuplicateMaterial_ShouldShowError()
    {
        _window.MaterialBox.SelectedItem = _testMaterial.MaterialName;
        _window.MaterialQuantityBox.Text = "5";
        _window.AddMaterial_Click(null, null);

        _window.MaterialBox.SelectedItem = _testMaterial.MaterialName;
        _window.MaterialQuantityBox.Text = "3";

        _window.AddMaterial_Click(null, null);

        Assert.That(_window._materials.Count, Is.EqualTo(1));
        Assert.That(_window.TextError.Text, Is.EqualTo("Этот материал уже добавлен"));
    }

    [Test]
    public void AddMaterial_Click_WithEmptyQuantity_ShouldShowError()
    {
        _window.MaterialBox.SelectedItem = _testMaterial.MaterialName;
        _window.MaterialQuantityBox.Text = "";

        _window.AddMaterial_Click(null, null);

        Assert.That(_window._materials.Count, Is.EqualTo(0));
        Assert.That(_window.TextError.Text, Is.EqualTo("Введите корректное значение кол-ва"));
    }

    [Test]
    public void AddMaterial_Click_WithNonNumericQuantity_ShouldShowError()
    {
        _window.MaterialBox.SelectedItem = _testMaterial.MaterialName;
        _window.MaterialQuantityBox.Text = "abc"; // Нечисловое значение

        _window.AddMaterial_Click(null, null);

        Assert.That(_window._materials.Count, Is.EqualTo(0));
        Assert.That(_window.TextError.Text, Is.EqualTo("Введите корректное значение кол-ва"));
    }
}