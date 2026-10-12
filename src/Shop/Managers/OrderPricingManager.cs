using System;

namespace Shop.Managers
{
    internal class OrderPricingManager : IOrderPricingManager
    {
        private const decimal LoyaltyRate = 0.05m;

        public OrderPrice Calculate(decimal subtotal, int promoPercentOff, bool isLoyaltyMember)
        {
            var promoDiscount = RoundToCents(subtotal * promoPercentOff / 100);

            var loyaltyDiscount = isLoyaltyMember ? RoundToCents((subtotal - promoDiscount) * LoyaltyRate) : 0m;

            return new OrderPrice
            {
                Subtotal = subtotal,
                PromoDiscount = promoDiscount,
                LoyaltyDiscount = loyaltyDiscount,
                Total = subtotal - promoDiscount - loyaltyDiscount
            };
        }

        private static decimal RoundToCents(decimal amount)
        {
            return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        }
    }
}