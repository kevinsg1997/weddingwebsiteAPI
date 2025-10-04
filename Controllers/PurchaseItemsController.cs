using Microsoft.AspNetCore.Mvc;
using WeddingMerchantApi.Data;
using WeddingMerchantApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WeddingMerchantApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseItemController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public PurchaseItemController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Criar um novo item de compra
        [HttpPost("create")]
        public async Task<IActionResult> CreatePurchaseItem([FromBody] PurchaseItem purchaseItem)
        {
            try
            {
                purchaseItem.Available = true; // Novo item começa como disponível
                _dbContext.PurchaseItem.Add(purchaseItem);
                await _dbContext.SaveChangesAsync();

                return Ok(purchaseItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        //Deletar um item de compra
        [HttpPut("delete")]
        public async Task<IActionResult> DeletePurchaseItem(string id)
        {
            try
            {
                var item = await _dbContext.PurchaseItem.FindAsync(Guid.Parse(id));
                if (item == null)
                {
                    return NotFound(new { error = "Item not found" });
                }

                item.Deleted = true;
                item.UpdatedAt = DateTime.UtcNow;

                _dbContext.PurchaseItem.Update(item);
                await _dbContext.SaveChangesAsync();

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Obter todos os itens de compra
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPurchaseItems()
        {
            try
            {
                var items = await _dbContext.PurchaseItem.ToListAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Obter itens não deletados
        [HttpGet("not-deleted")]
        public async Task<IActionResult> GetNotDeletedPurchaseItems()
        {
            try
            {
                var items = await _dbContext.PurchaseItem.ToListAsync();
                items = items.Where(x => !x.Deleted).ToList();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
