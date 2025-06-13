using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lopushok.Models;

public class ProductSalesDAO
{
    public int id { get; set; }
    
    [ForeignKey("Product")] // Указываем, что это внешний ключ
    public int product_id { get; set; }
    
    public DateOnly sale_date { get; set; }
    
    // Навигационное свойство к продукту
    public virtual ProductDAO Product { get; set; }
}