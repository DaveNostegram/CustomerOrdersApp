using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerOrdersApp.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ChangeItemId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_CustomerOrders_Customers_CustomerId",
            schema: "dbo",
            table: "CustomerOrders");

        migrationBuilder.DropForeignKey(
            name: "FK_OrderItems_CustomerOrders_OrderId",
            schema: "dbo",
            table: "OrderItems");

        migrationBuilder.DropPrimaryKey(
            name: "PK_CustomerOrders",
            schema: "dbo",
            table: "CustomerOrders");

        migrationBuilder.RenameTable(
            name: "CustomerOrders",
            schema: "dbo",
            newName: "Orders",
            newSchema: "dbo");

        migrationBuilder.RenameIndex(
            name: "IX_CustomerOrders_PublicId",
            schema: "dbo",
            table: "Orders",
            newName: "IX_Orders_PublicId");

        migrationBuilder.RenameIndex(
            name: "IX_CustomerOrders_CustomerId",
            schema: "dbo",
            table: "Orders",
            newName: "IX_Orders_CustomerId");

        migrationBuilder.AddColumn<int>(
            name: "ItemId",
            schema: "dbo",
            table: "OrderItems",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddPrimaryKey(
            name: "PK_Orders",
            schema: "dbo",
            table: "Orders",
            column: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_OrderItems_Orders_OrderId",
            schema: "dbo",
            table: "OrderItems",
            column: "OrderId",
            principalSchema: "dbo",
            principalTable: "Orders",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Orders_Customers_CustomerId",
            schema: "dbo",
            table: "Orders",
            column: "CustomerId",
            principalSchema: "dbo",
            principalTable: "Customers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_OrderItems_Orders_OrderId",
            schema: "dbo",
            table: "OrderItems");

        migrationBuilder.DropForeignKey(
            name: "FK_Orders_Customers_CustomerId",
            schema: "dbo",
            table: "Orders");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Orders",
            schema: "dbo",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "ItemId",
            schema: "dbo",
            table: "OrderItems");

        migrationBuilder.RenameTable(
            name: "Orders",
            schema: "dbo",
            newName: "CustomerOrders",
            newSchema: "dbo");

        migrationBuilder.RenameIndex(
            name: "IX_Orders_PublicId",
            schema: "dbo",
            table: "CustomerOrders",
            newName: "IX_CustomerOrders_PublicId");

        migrationBuilder.RenameIndex(
            name: "IX_Orders_CustomerId",
            schema: "dbo",
            table: "CustomerOrders",
            newName: "IX_CustomerOrders_CustomerId");

        migrationBuilder.AddPrimaryKey(
            name: "PK_CustomerOrders",
            schema: "dbo",
            table: "CustomerOrders",
            column: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_CustomerOrders_Customers_CustomerId",
            schema: "dbo",
            table: "CustomerOrders",
            column: "CustomerId",
            principalSchema: "dbo",
            principalTable: "Customers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_OrderItems_CustomerOrders_OrderId",
            schema: "dbo",
            table: "OrderItems",
            column: "OrderId",
            principalSchema: "dbo",
            principalTable: "CustomerOrders",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
