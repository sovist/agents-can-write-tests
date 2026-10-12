using System;
using System.Threading.Tasks;
using Shop.Api;
using Shop.Data;

namespace Shop.Services
{
    internal class CatalogAppService : ICatalogAppService
    {
        private readonly ShopDbContext _dbContext;

        public CatalogAppService(ShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> AddProduct(string name, decimal price)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Price = price
            };

            _dbContext.Products.Add(product);

            await _dbContext.SaveChangesAsync();

            return product.Id;
        }

        public async Task AddPromoCode(string code, int percentOff)
        {
            _dbContext.PromoCodes.Add(new PromoCode
            {
                Code = code,
                PercentOff = percentOff
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}