using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Lopushok.Migrations
{
    /// <inheritdoc />
    public partial class create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "lopushok");

            migrationBuilder.CreateTable(
                name: "material_types",
                schema: "lopushok",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    material_type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_material_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_types",
                schema: "lopushok",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    product_type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "materials",
                schema: "lopushok",
                columns: table => new
                {
                    material_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    material_name = table.Column<string>(type: "text", nullable: false),
                    material_type = table.Column<int>(type: "integer", nullable: false),
                    package_quantity = table.Column<int>(type: "integer", nullable: false),
                    unit = table.Column<string>(type: "text", nullable: false),
                    stock_quantity = table.Column<int>(type: "integer", nullable: false),
                    min_remaining = table.Column<int>(type: "integer", nullable: false),
                    cost = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_materials", x => x.material_id);
                    table.ForeignKey(
                        name: "FK_materials_material_types_material_type",
                        column: x => x.material_type,
                        principalSchema: "lopushok",
                        principalTable: "material_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "products",
                schema: "lopushok",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    product_name = table.Column<string>(type: "text", nullable: false),
                    article = table.Column<string>(type: "text", nullable: false),
                    min_agent_cost = table.Column<decimal>(type: "numeric", nullable: true),
                    image_path = table.Column<string>(type: "text", nullable: false),
                    product_type = table.Column<int>(type: "integer", nullable: false),
                    workers_required = table.Column<int>(type: "integer", nullable: false),
                    workshop_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_products_product_types_product_type",
                        column: x => x.product_type,
                        principalSchema: "lopushok",
                        principalTable: "product_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_materials",
                schema: "lopushok",
                columns: table => new
                {
                    product_material_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    material_id = table.Column<int>(type: "integer", nullable: false),
                    required_quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_materials", x => x.product_material_id);
                    table.ForeignKey(
                        name: "FK_product_materials_materials_material_id",
                        column: x => x.material_id,
                        principalSchema: "lopushok",
                        principalTable: "materials",
                        principalColumn: "material_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_materials_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "lopushok",
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_materials_material_type",
                schema: "lopushok",
                table: "materials",
                column: "material_type");

            migrationBuilder.CreateIndex(
                name: "IX_product_materials_material_id",
                schema: "lopushok",
                table: "product_materials",
                column: "material_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_materials_product_id",
                schema: "lopushok",
                table: "product_materials",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_product_type",
                schema: "lopushok",
                table: "products",
                column: "product_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_materials",
                schema: "lopushok");

            migrationBuilder.DropTable(
                name: "materials",
                schema: "lopushok");

            migrationBuilder.DropTable(
                name: "products",
                schema: "lopushok");

            migrationBuilder.DropTable(
                name: "material_types",
                schema: "lopushok");

            migrationBuilder.DropTable(
                name: "product_types",
                schema: "lopushok");
        }
    }
}
