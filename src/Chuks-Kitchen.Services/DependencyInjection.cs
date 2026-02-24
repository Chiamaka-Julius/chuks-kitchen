using Chuks_Kitchen.Services.BL.Implementations;
using Chuks_Kitchen.Services.BL.Interfaces;
using Chuks_Kitchen.Services.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Chuks_Kitchen.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICqrsDispatcher, CqrsDispatcher>();

        services.AddScoped<IUserBL, UserBL>();
        services.AddScoped<IFoodBL, FoodBL>();
        services.AddScoped<ICartBL, CartBL>();
        services.AddScoped<IOrderBL, OrderBL>();

        return services;
    }
}
