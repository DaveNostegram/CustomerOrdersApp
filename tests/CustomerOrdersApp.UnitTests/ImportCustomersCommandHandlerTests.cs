using CustomerOrdersApp.Application.FileUploads.Commands;
using CustomerOrdersApp.Application.FileUploads.ImportCustomers;
using CustomerOrdersApp.Application.FileUploads.ImportCustomers.Validation;
using CustomerOrdersApp.Application.Interfaces.Repositories;
using CustomerOrdersApp.Contracts;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace CustomerOrdersApp.UnitTests;

public sealed class ImportCustomersCommandHandlerTests
{
    private readonly IValidator<CustomerImportRow> _validator;
    private readonly Mock<ICustomerRepo> _mockRepo;
    public ImportCustomersCommandHandlerTests()
    {
        _validator = new CustomerImportRowValidator();
        _mockRepo = new Mock<ICustomerRepo>();
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenCustomerIdInvalid()
    {
        // Arrange
        var handler = new ImportCustomersCommandHandler(_mockRepo.Object);

        var stream = CsvHelper.CreateCsvStream(
        [
            ["customer_id","first_name","last_name","phone","email","street","city","state","zip_code"],
            ["ABC","John","Doe","","john@test.com","Street","City","NY","12345"]
        ]);

        var command = new ImportCustomersCommand(
            stream,
            "test.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle();

        result.Errors.First().Should().Contain($"Row '2' column 'CustomerId'");
        result.Errors.First().Should().Contain($"customer_id must be a valid integer.");
    }

    [Fact]
    public async Task Handle_ReturnsError_NoFirstName()
    {
        // Arrange
        var handler = new ImportCustomersCommandHandler(_mockRepo.Object);

        var stream = CsvHelper.CreateCsvStream(
        [
            ["customer_id","first_name","last_name","phone","email","street","city","state","zip_code"],
            ["1","","Doe","","john@test.com","Street","City","NY","12345"]
        ]);

        var command = new ImportCustomersCommand(
            stream,
            "test.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle();

        result.Errors.First().Should().Contain("Row '2' column 'FirstName'");
        result.Errors.First().Should().Contain("First Name' must not be empty.");
    }

    [Fact]
    public async Task Handle_ReturnsError_MissingHeader()
    {
        // Arrange
        var handler = new ImportCustomersCommandHandler(_mockRepo.Object);

        var stream = CsvHelper.CreateCsvStream(
        [
            ["customer_id","first_name","last_name","phone","street","city","state","zip_code"],
            ["1","John","Doe","","Street","City","NY","12345"]
        ]);

        var command = new ImportCustomersCommand(
            stream,
            "test.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle();

        result.Errors.First().Should().Contain("Missing required header 'email'.");
    }

    [Fact]
    public async Task Handle_ReturnsError_DuplicateHeader()
    {
        // Arrange
        var handler = new ImportCustomersCommandHandler(_mockRepo.Object);

        var stream = CsvHelper.CreateCsvStream(
        [
            ["customer_id","first_name","last_name","email","email","phone","street","city","state","zip_code"],
            ["1","John","Doe","john@test.com","john@test.com","","Street","City","NY","12345"]
        ]);

        var command = new ImportCustomersCommand(
            stream,
            "test.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle();

        result.Errors.First().Should().Contain("Duplicated header 'email'.");
    }
}