using System;
using System.Threading.Tasks;

namespace Shop.Api
{
    /// <summary>
    /// Products and promo codes on sale.
    /// </summary>
    public interface ICatalogAppService : IApplicationService
    {
        Task<Guid> AddProduct(string name, decimal price);

        /// <summary>
        /// Adds a promo code that takes <paramref name="percentOff"/> percent off an order.
        /// </summary>
        Task AddPromoCode(string code, int percentOff);
    }
}