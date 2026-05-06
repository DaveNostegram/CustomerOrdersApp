using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CustomerOrdersApp.Infrastructure.Persistence;
using CustomerOrdersApp.Application.Interfaces.Repositories;
using CustomerOrdersApp.Infrastructure.Repositories;
using CustomerOrdersApp.Application.Discounts;
using CustomerOrdersApp.Application.Interfaces.Services;

namespace CustomerOrdersApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                ));


        services.AddScoped<ICustomerRepo, CustomerRepo>();
        services.AddScoped<IDiscountRepo, DiscountRepo>();

        services.AddScoped<IDiscountService, DiscountService>();

        return services;
    }
}