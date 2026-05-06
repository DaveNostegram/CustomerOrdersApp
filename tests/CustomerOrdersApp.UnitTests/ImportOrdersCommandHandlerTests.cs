using CustomerOrdersApp.Application.FileUploads;
using CustomerOrdersApp.Application.FileUploads.Commands;
using CustomerOrdersApp.Application.Interfaces.Repositories;
using CustomerOrdersApp.Contracts;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace CustomerOrdersApp.UnitTests;

public sealed class ImportOrdersCommandHandlerTests
{
    private readonly Mock<ICustomerRepo> _mockRepo;

    public ImportOrdersCommandHandlerTests()
    {
        _mockRepo = new Mock<ICustomerRepo>();
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenOrderDateInvalid()
    {
        // Arrange
        _mockRepo
            .Setup(x => x.GetCustomerIdsByPublicIdsAsync(
                It.IsAny<IReadOnlyCollection<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, int>
            {
                { 1, 10 }
            });

        var handler = new ImportOrdersCommandHandler(_mockRepo.Object);

        var stream = CsvHelper.CreateCsvStream(
        [
            ["order_id","customer_id","order_status","order_date","required_date","shipped_date"],
            ["100","1","1","2016-02-16","17/02/2016","18/02/2016"]
        ]);

        var command = new ImportOrdersCommand(
            stream,
            "orders.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle();

        result.Errors.First().Should().Contain("Row '2' column 'OrderDate'");
        result.Errors.First().Should().Contain("order_date must be in format dd/MM/yyyy.");
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenCustomerDoesNotExist()
    {
        // Arrange
        _mockRepo
            .Setup(x => x.GetCustomerIdsByPublicIdsAsync(
                It.IsAny<IReadOnlyCollection<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, int>());

        var handler = new ImportOrdersCommandHandler(_mockRepo.Object);

        var stream = CsvHelper.CreateCsvStream(
        [
            ["order_id","customer_id","order_status","order_date","required_date","shipped_date"],
            ["100","999","1","16/02/2016","17/02/2016",""]
        ]);

        var command = new ImportOrdersCommand(
            stream,
            "orders.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle();

        result.Errors.First().Should().Contain("Row '2' column 'CustomerId'");
        result.Errors.First().Should().Contain("customer_id does not exist.");
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenOrderStatusInvalid()
    {
        // Arrange
        _mockRepo
            .Setup(x => x.GetCustomerIdsByPublicIdsAsync(
                It.IsAny<IReadOnlyCollection<int>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, int>
            {
                { 1, 10 }
            });

        var handler = new ImportOrdersCommandHandler(_mockRepo.Object);

        var stream = CsvHelper.CreateCsvStream(
        [
            ["order_id","customer_id","order_status","order_date","required_date","shipped_date"],
            ["100","1","999","16/02/2016","17/02/2016",""]
        ]);

        var command = new ImportOrdersCommand(
            stream,
            "orders.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle();

        result.Errors.First().Should().Contain("Row '2' column 'OrderStatus'");
        result.Errors.First().Should().Contain("order_status is not valid.");
    }
}