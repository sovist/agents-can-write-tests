using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shop.Api.Dto;
using Shop.Data;

namespace Shop.Tests.EntityFactory
{
    /// <summary>
    /// The default and preferred way tests create data. Writes entities straight to the database, like a production code base's entity factory.
    /// A <c>Create...</c> method takes the values a test is about as parameters, and anything else through <c>update</c>.
    /// </summary>
    /// <remarks>
    /// Defaults are what the application itself would produce: <c>ShopFactoryTests</c> compares them with the API.
    /// <c>update</c> sets the fields a test is about, never values the application computes, such as totals.
    /// </remarks>
    public class ShopFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ShopFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<Product> CreateProduct(decimal price, Action<Product> update = null)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = GetRandomName(),
                Price = price
            };

            update?.Invoke(product);

            return await AddEntity(product);
        }

        public async Task<PromoCode> CreatePromoCode(string code, int percentOff, Action<PromoCode> update = null)
        {
            var promoCode = new PromoCode
            {
                Code = code,
                PercentOff = percentOff
            };

            update?.Invoke(promoCode);

            return await AddEntity(promoCode);
        }

        /// <summary>
        /// A customer who is not a loyalty member.
        /// </summary>
        public async Task<Customer> CreateCustomer(Action<Customer> update = null)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = GetRandomName(),
                IsLoyaltyMember = false
            };

            update?.Invoke(customer);

            return await AddEntity(customer);
        }

        /// <summary>
        /// An order awaiting payment, from a new customer who is not a loyalty member and uses no promo code.
        /// No discount applies, so the total equals the subtotal, as the application prices such an order.
        /// </summary>
        public async Task<Order> CreateOrder(decimal subtotal, Action<Order> update = null)
        {
            var customer = await CreateCustomer();

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                Subtotal = subtotal,
                PromoDiscount = 0m,
                LoyaltyDiscount = 0m,
                Total = subtotal,
                Status = OrderStatus.AwaitingPayment
            };

            update?.Invoke(order);

            return await AddEntity(order);
        }

        private async Task<TEntity> AddEntity<TEntity>(TEntity entity)
            where TEntity : class
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();

                dbContext.Set<TEntity>().Add(entity);

                await dbContext.SaveChangesAsync();
            }

            return entity;
        }

        private static string GetRandomName()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 15);
        }
    }
}