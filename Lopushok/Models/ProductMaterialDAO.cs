namespace Lopushok.Models;

public class ProductMaterialDAO
{
    public int ProductMaterialId { get; set; }
    public int ProductId { get; set; }
    public int MaterialId { get; set; }
    public int RequiredQuantity { get; set; }

    public ProductDAO Product { get; set; }
    public MaterialDAO Material { get; set; }
}