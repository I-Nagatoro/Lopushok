using System.Collections.Generic;

namespace Lopushok.Models;

public class ProductDAO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string Article { get; set; }
    public decimal? MinAgentCost { get; set; }
    public string ImagePath { get; set; }
    public int ProductTypeId { get; set; }
    public int WorkersRequired { get; set; }
    public int WorkshopNumber { get; set; }
    public ProductTypeDAO ProductType { get; set; }
    public ICollection<ProductMaterialDAO> ProductMaterials { get; set; }
    public virtual ICollection<ProductSalesDAO> ProductSales { get; set; } = new List<ProductSalesDAO>();
}
