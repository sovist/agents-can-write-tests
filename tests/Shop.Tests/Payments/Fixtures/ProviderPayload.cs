using System.Text.Json.Nodes;

namespace Shop.Tests.Payments.Fixtures
{
    /// <summary>
    /// Payloads copied from the provider's documentation, one method per documented example.
    /// Only <c>data.reference</c> is replaced. The provider echoes it back from checkout, so it is ours to set.
    /// Every field the provider controls stays exactly as documented.
    /// </summary>
    internal static class ProviderPayload
    {
        /// <summary>
        /// The documented <c>payment.succeeded</c> example: 85.50 EUR, sent as 8550 minor units.
        /// </summary>
        public static string PaymentSucceeded(Guid reference)
        {
            return Load("payment.succeeded.json", reference);
        }

        /// <summary>
        /// The documented <c>payment.failed</c> example: a declined card for 85.50 EUR.
        /// </summary>
        public static string PaymentFailed(Guid reference)
        {
            return Load("payment.failed.json", reference);
        }

        private static string Load(string fixture, Guid reference)
        {
            var payload = JsonNode.Parse(typeof(ProviderPayload).GetResourceFileContent(fixture));

            var data = payload["data"];

            if (data?["reference"] == null)
            {
                throw new InvalidOperationException($"{fixture} has no data.reference to replace.");
            }

            data["reference"] = reference.ToString();

            return payload.ToJsonString();
        }
    }
}