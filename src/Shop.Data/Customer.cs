using System;

namespace Shop.Data
{
    public class Customer
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsLoyaltyMember { get; set; }
    }
}