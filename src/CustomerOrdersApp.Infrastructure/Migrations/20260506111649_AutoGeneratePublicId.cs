using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerOrdersApp.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AutoGeneratePublicId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateSequence<int>(
            name: "OrderItemPublicIds",
            schema: "dbo");

        migrationBuilder.AlterColumn<int>(
            name: "PublicId",
            schema: "dbo",
            table: "OrderItems",
            type: "int",
            nullable: false,
            defaultValueSql: "NEXT VALUE FOR OrderItemPublicIds",
            oldClrType: typeof(int),
            oldType: "int");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropSequence(
            name: "OrderItemPublicIds",
            schema: "dbo");

        migrationBuilder.AlterColumn<int>(
            name: "PublicId",
            schema: "dbo",
            table: "OrderItems",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int",
            oldDefaultValueSql: "NEXT VALUE FOR OrderItemPublicIds");
    }
}
