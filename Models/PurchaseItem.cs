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

        public PurchaseItem(string name, string description, decimal price, bool available, bool deleted)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null");
            Description = description ?? throw new ArgumentNullException(nameof(description), "Description cannot be null");
            Price = price;
            Available = available;
            Deleted = deleted;
        }
    }
}