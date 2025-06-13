using Lopushok.Models;
using Microsoft.EntityFrameworkCore;

namespace Lopushok;

public class RemoteDatabaseContext : DbContext
{
    public DbSet<MaterialTypeDAO> MaterialTypes { get; set; }
    public DbSet<MaterialDAO> Materials { get; set; }
    public DbSet<ProductTypeDAO> ProductTypes { get; set; }
    public DbSet<ProductDAO> Products { get; set; }
    public DbSet<ProductMaterialDAO> ProductMaterials { get; set; }
    
    public DbSet<ProductSalesDAO> ProductSales { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=45.67.56.214;Port=5421;Database=user7;Username=user7;Password=a8yLONBC;Include Error Detail=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaterialTypeDAO>(e =>
        {
            e.ToTable("material_types", "lopushok");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            e.Property(x => x.MaterialType).HasColumnName("material_type");
        });

        modelBuilder.Entity<ProductSalesDAO>(e =>
        {
            e.ToTable("product_sales", "lopushok");
            e.HasKey(x => x.id);
            e.Property(x => x.id).HasColumnName("id").ValueGeneratedOnAdd();
            e.Property(x => x.product_id).HasColumnName("product_id");
            e.Property(x => x.sale_date).HasColumnName("sale_date").ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<MaterialDAO>(e =>
        {
            e.ToTable("materials", "lopushok");
            e.HasKey(x => x.MaterialId);
            e.Property(x => x.MaterialId).HasColumnName("material_id").ValueGeneratedOnAdd();
            e.Property(x => x.MaterialName).HasColumnName("material_name");
            e.Property(x => x.MaterialTypeId).HasColumnName("material_type");
            e.Property(x => x.PackageQuantity).HasColumnName("package_quantity");
            e.Property(x => x.Unit).HasColumnName("unit");
            e.Property(x => x.StockQuantity).HasColumnName("stock_quantity");
            e.Property(x => x.MinRemaining).HasColumnName("min_remaining");
            e.Property(x => x.Cost).HasColumnName("cost");
        });

        modelBuilder.Entity<ProductTypeDAO>(e =>
        {
            e.ToTable("product_types", "lopushok");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            e.Property(x => x.ProductType).HasColumnName("product_type");
        });

        modelBuilder.Entity<ProductDAO>(e =>
        {
            e.ToTable("products", "lopushok");
            e.HasKey(x => x.ProductId);
            e.Property(x => x.ProductId).HasColumnName("product_id").ValueGeneratedOnAdd();
            e.Property(x => x.ProductName).HasColumnName("product_name");
            e.Property(x => x.Article).HasColumnName("article");
            e.Property(x => x.MinAgentCost).HasColumnName("min_agent_cost");
            e.Property(x => x.ImagePath).HasColumnName("image_path");
            e.Property(x => x.ProductTypeId).HasColumnName("product_type");
            e.Property(x => x.WorkersRequired).HasColumnName("workers_required");
            e.Property(x => x.WorkshopNumber).HasColumnName("workshop_number");
        });

        modelBuilder.Entity<ProductMaterialDAO>(e =>
        {
            e.ToTable("product_materials", "lopushok");
            e.HasKey(x => x.ProductMaterialId);
            e.Property(x => x.ProductMaterialId).HasColumnName("product_material_id").ValueGeneratedOnAdd();
            e.Property(x => x.ProductId).HasColumnName("product_id");
            e.Property(x => x.MaterialId).HasColumnName("material_id");
            e.Property(x => x.RequiredQuantity).HasColumnName("required_quantity");
        });
    }
}
