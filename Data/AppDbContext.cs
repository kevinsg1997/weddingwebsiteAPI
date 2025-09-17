using Microsoft.EntityFrameworkCore;
using WeddingMerchantApi.Models;

namespace WeddingMerchantApi.Data
{
    public class AppDbContext : DbContext
    {
        // DbSet para a tabela PurchaseItems
        public DbSet<PurchaseItem> PurchaseItems { get; set; }

        // DbSet para a tabela Guests
        public DbSet<Guest> Guests { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aqui posso adicionar configurações extras se necessário
            modelBuilder.Entity<PurchaseItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<Guest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            });
        }

        public async Task UpdateItemAsSold(string itemId, string buyerName)
        {
            var purchaseItem = await PurchaseItems
                .FirstOrDefaultAsync(item => item.Id.ToString() == itemId);

            if (purchaseItem != null)
            {
                // Atualiza as informações do comprador e marca o item como não disponível
                purchaseItem.Buyer = buyerName;
                purchaseItem.Available = false;

                await SaveChangesAsync();
            }
        }
    }
}
