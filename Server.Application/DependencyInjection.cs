using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Server.Application.Customers;

namespace Server.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddScoped<ICustomerService, CustomerService>();

        return services;
    }
}
