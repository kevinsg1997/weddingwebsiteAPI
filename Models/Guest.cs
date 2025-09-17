using System;

namespace WeddingMerchantApi.Models
{
    public class Guest
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public bool IsGoing { get; set; }
    
        public Guest(string name, bool isGoing)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null");
            IsGoing = isGoing;
        }
    }
}
