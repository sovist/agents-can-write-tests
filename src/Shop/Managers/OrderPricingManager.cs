using System;

namespace Shop.Managers
{
    internal class OrderPricingManager : IOrderPricingManager
    {
        private const decimal LoyaltyRate = 0.05m;

        public OrderPrice Calculate(decimal subtotal, int promoPercentOff, bool isLoyaltyMember)
        {
            var loyaltyDiscount = isLoyaltyMember ? RoundToCents(subtotal * LoyaltyRate) : 0m;

            return new OrderPrice
            {
                Subtotal = subtotal,
                LoyaltyDiscount = loyaltyDiscount,
                Total = subtotal - loyaltyDiscount
            };
        }

        private static decimal RoundToCents(decimal amount)
        {
            return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        }
    }
}