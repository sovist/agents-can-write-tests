namespace Shop.Managers
{
    /// <summary>
    /// Order pricing. Loyalty members get 5% off the subtotal, rounded to cents, half away from zero.
    /// </summary>
    internal interface IOrderPricingManager
    {
        OrderPrice Calculate(decimal subtotal, bool isLoyaltyMember);
    }

    internal class OrderPrice
    {
        public decimal Subtotal { get; set; }

        public decimal LoyaltyDiscount { get; set; }

        public decimal Total { get; set; }
    }
}