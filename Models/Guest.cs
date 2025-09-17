using System;

namespace WeddingMerchantApi.Models
{
    public class Guest
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool? IsGoing { get; set; }
    }
}
