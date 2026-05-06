using MediatR;

namespace CustomerOrdersApp.Application.Discounts;

public record CustomerDiscountApplied(
    int CustomerId,
    decimal DiscountAmount,
    string Reason,
    DateTime AppliedAt
) : INotification;

public class CustomerDiscountAppliedHandler
    : INotificationHandler<CustomerDiscountApplied>
{
    public Task Handle(
        CustomerDiscountApplied notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Customer {notification.CustomerId} had {Math.Round(notification.DiscountAmount * 100)}% discount applied because {notification.Reason} at {notification.AppliedAt}");
        return Task.CompletedTask;
    }
}