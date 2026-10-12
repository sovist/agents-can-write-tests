using System;
using System.Threading.Tasks;
using Shop.Api;
using Shop.Data;

namespace Shop.Services
{
    internal class CustomerAppService : ICustomerAppService
    {
        private readonly ShopDbContext _dbContext;

        public CustomerAppService(ShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> Register(string name, bool isLoyaltyMember)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = name,
                IsLoyaltyMember = isLoyaltyMember
            };

            _dbContext.Customers.Add(customer);

            await _dbContext.SaveChangesAsync();

            return customer.Id;
        }
    }
}