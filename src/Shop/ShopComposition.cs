using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shop.Api;
using Shop.Data;
using Shop.Managers;
using Shop.Services;

namespace Shop
{
    /// <summary>
    /// The composition root: the registrations a host makes at startup. Tests boot the same one.
    /// </summary>
    public static class ShopComposition
    {
        /// <summary>
        /// Registers the application services, the managers and the database context.
        /// <paramref name="database"/> picks the database provider.
        /// </summary>
        public static IServiceCollection AddShop(this IServiceCollection services, Action<DbContextOptionsBuilder> database)
        {
            services.AddDbContext<ShopDbContext>(database);

            services.AddScoped<IOrderPricingManager, OrderPricingManager>();

            services.AddScoped<ICatalogAppService, CatalogAppService>();
            services.AddScoped<ICustomerAppService, CustomerAppService>();
            services.AddScoped<IOrderAppService, OrderAppService>();

            return services;
        }

        /// <summary>
        /// Creates the database schema if it does not exist yet.
        /// </summary>
        public static void EnsureShopDatabase(this IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<ShopDbContext>().Database.EnsureCreated();
            }
        }
    }
}