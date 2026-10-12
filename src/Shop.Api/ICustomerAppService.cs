using System;
using System.Threading.Tasks;

namespace Shop.Api
{
    public interface ICustomerAppService : IApplicationService
    {
        Task<Guid> Register(string name, bool isLoyaltyMember);
    }
}