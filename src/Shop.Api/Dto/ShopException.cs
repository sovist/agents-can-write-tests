using System;

namespace Shop.Api.Dto
{
    /// <summary>
    /// A request the shop refuses. <see cref="Type"/> says why.
    /// </summary>
    public class ShopException : Exception
    {
        public ShopException(ShopExceptionType type)
            : base($"The shop refused the request: {type}.")
        {
            Type = type;
        }

        public ShopExceptionType Type { get; }
    }
}