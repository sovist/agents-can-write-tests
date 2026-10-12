namespace Shop.Api.Dto
{
    public enum WebhookOutcome
    {
        /// <summary>
        /// The event changed the order.
        /// </summary>
        Applied,

        /// <summary>
        /// The event needs no action: another event type, or a redelivery.
        /// </summary>
        Ignored,

        /// <summary>
        /// The event matches no order: an unknown reference, or a different amount or currency.
        /// </summary>
        Rejected
    }
}