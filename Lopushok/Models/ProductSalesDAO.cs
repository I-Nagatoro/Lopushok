using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lopushok.Models;

public class ProductSalesDAO
{
    public int id { get; set; }
    
    [ForeignKey("Product")]
    public int product_id { get; set; }
    public DateOnly sale_date { get; set; }
    public virtual ProductDAO Product { get; set; }
}