using System;
using System.Threading.Tasks;

namespace Shop.Api
{
    /// <summary>
    /// Products on sale.
    /// </summary>
    public interface ICatalogAppService : IApplicationService
    {
        Task<Guid> AddProduct(string name, decimal price);
    }
}