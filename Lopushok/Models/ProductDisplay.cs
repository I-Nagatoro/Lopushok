using System;
using System.IO;
using Avalonia.Media.Imaging;

namespace Lopushok.Models;

public class ProductDisplay
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductType { get; set; }
    public string Article { get; set; }
    public int WorkshopNumber { get; set; }
    public decimal? MinAgentCost { get; set; }
    public string ImagePath { get; set; }
    public DateOnly? LastSaleDate { get; set; }
    public bool IsStale { get; set; }
    public decimal Cost { get; set; }

    public string Price => (MinAgentCost ?? 0).ToString("0.##") + " ₽";
    public string LastSaleDateFormatted => LastSaleDate?.ToString("dd.MM.yyyy") ?? "Нет данных";

    public Bitmap Image
    {
        get
        {
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, ImagePath);
                return File.Exists(path) ? new Bitmap(path) : new Bitmap("Assets/picture.png");
            }
            catch
            {
                return new Bitmap("Assets/picture.png");
            }
        }
    }
}