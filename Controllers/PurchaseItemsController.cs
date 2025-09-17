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
                _dbContext.PurchaseItems.Add(purchaseItem);
                await _dbContext.SaveChangesAsync();

                return Ok(purchaseItem);
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
                var items = await _dbContext.PurchaseItems.ToListAsync();
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
                var items = await _dbContext.PurchaseItems.Where(x => !x.Deleted).ToListAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
