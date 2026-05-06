using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerOrdersApp.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "dbo");

        migrationBuilder.CreateTable(
            name: "Customers",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                State = table.Column<int>(type: "int", nullable: false),
                ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                PublicId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Customers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CustomerOrders",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                RequiredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                ShippedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                PublicId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CustomerOrders", x => x.Id);
                table.ForeignKey(
                    name: "FK_CustomerOrders_Customers_CustomerId",
                    column: x => x.CustomerId,
                    principalSchema: "dbo",
                    principalTable: "Customers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "OrderItems",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                OrderId = table.Column<int>(type: "int", nullable: false),
                ListPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                FinalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                PublicId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_OrderItems_CustomerOrders_OrderId",
                    column: x => x.OrderId,
                    principalSchema: "dbo",
                    principalTable: "CustomerOrders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CustomerOrders_CustomerId",
            schema: "dbo",
            table: "CustomerOrders",
            column: "CustomerId");

        migrationBuilder.CreateIndex(
            name: "IX_CustomerOrders_PublicId",
            schema: "dbo",
            table: "CustomerOrders",
            column: "PublicId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Customers_PublicId",
            schema: "dbo",
            table: "Customers",
            column: "PublicId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_OrderItems_OrderId",
            schema: "dbo",
            table: "OrderItems",
            column: "OrderId");

        migrationBuilder.CreateIndex(
            name: "IX_OrderItems_PublicId",
            schema: "dbo",
            table: "OrderItems",
            column: "PublicId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OrderItems",
            schema: "dbo");

        migrationBuilder.DropTable(
            name: "CustomerOrders",
            schema: "dbo");

        migrationBuilder.DropTable(
            name: "Customers",
            schema: "dbo");
    }
}
