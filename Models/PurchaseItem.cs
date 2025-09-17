using System;

namespace WeddingMerchantApi.Models
{
    public class PurchaseItem
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public string? Buyer { get; set; }
        public string? Uri { get; set; }
        public bool Deleted { get; set; }
    }
}