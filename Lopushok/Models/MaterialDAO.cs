namespace Lopushok.Models;

public class MaterialDAO
{
    public int MaterialId { get; set; }
    public string MaterialName { get; set; }
    public int MaterialTypeId { get; set; }
    public int PackageQuantity { get; set; }
    public string Unit { get; set; }
    public int StockQuantity { get; set; }
    public int MinRemaining { get; set; }
    public decimal Cost { get; set; }
    
    public MaterialTypeDAO MaterialType { get; set; }
}