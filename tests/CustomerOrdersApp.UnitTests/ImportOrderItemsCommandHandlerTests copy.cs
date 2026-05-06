using CustomerOrdersApp.Application.FileUploads.Commands;
using CustomerOrdersApp.Application.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace CustomerOrdersApp.UnitTests;

public sealed class ImportOrderItemsCommandHandlerTests
{
    private readonly Mock<ICustomerRepo> _mockRepo;

    public ImportOrderItemsCommandHandlerTests()
    {
        _mockRepo = new Mock<ICustomerRepo>();
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenOrderDoesNotExist()
    {
        // Arrange
        _mockRepo
            .Setup(x => x.GetOrderIdsByPublicIdsAsync(
                It.IsAny<IReadOnlyCollection<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, int>());

        var handler = new ImportOrderItemsCommandHandler(_mockRepo.Object);

        var stream = CsvHelper.CreateCsvStream(
        [
            ["order_id","item_id","list_price"],
            ["999","1","10.50"]
        ]);

        var command = new ImportOrderItemsCommand(
            stream,
            "order-items.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle();

        result.Errors.First().Should().Contain("Row '2' column 'OrderId'");
        result.Errors.First().Should().Contain("order_Id does not exist.");
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenListPriceIsNotDecimal()
    {
        // Arrange
        _mockRepo
            .Setup(x => x.GetOrderIdsByPublicIdsAsync(
                It.IsAny<IReadOnlyCollection<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, int>
            {
                { 100, 10 }
            });

        var handler = new ImportOrderItemsCommandHandler(_mockRepo.Object);

        var stream = CsvHelper.CreateCsvStream(
        [
            ["order_id","item_id","list_price"],
            ["100","1","abc"]
        ]);

        var command = new ImportOrderItemsCommand(
            stream,
            "order-items.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle();

        result.Errors.First().Should().Contain("Row '2' column 'ListPrice'");
        result.Errors.First().Should().Contain("list_price must be a valid decimal.");
    }
}