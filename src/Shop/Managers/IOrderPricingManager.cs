namespace Shop.Managers
{
    /// <summary>
    /// Order pricing. The promo code comes off the subtotal, and the loyalty discount (5% for loyalty members) comes off what remains.
    /// Each discount is rounded to cents, half away from zero.
    /// </summary>
    internal interface IOrderPricingManager
    {
        OrderPrice Calculate(decimal subtotal, int promoPercentOff, bool isLoyaltyMember);
    }

    internal class OrderPrice
    {
        public decimal Subtotal { get; set; }

        public decimal PromoDiscount { get; set; }

        public decimal LoyaltyDiscount { get; set; }

        public decimal Total { get; set; }
    }
}