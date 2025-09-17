using Microsoft.AspNetCore.Mvc;
using WeddingMerchantApi.Data;
using WeddingMerchantApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WeddingMerchantApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public GuestController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Criar um novo convidado
        [HttpPost("create")]
        public async Task<IActionResult> CreateGuest([FromBody] Guest guest)
        {
            try
            {
                _dbContext.Guests.Add(guest);
                await _dbContext.SaveChangesAsync();

                return Ok(guest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Obter todos os convidados
        [HttpGet("all")]
        public async Task<IActionResult> GetAllGuests()
        {
            try
            {
                var guests = await _dbContext.Guests.ToListAsync();
                return Ok(guests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Obter um convidado por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGuestById(long id)
        {
            try
            {
                var guest = await _dbContext.Guests.FindAsync(id);
                if (guest == null)
                {
                    return NotFound();
                }

                return Ok(guest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Atualizar um convidado
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuest(long id, [FromBody] Guest updatedGuest)
        {
            try
            {
                var guest = await _dbContext.Guests.FindAsync(id);
                if (guest == null)
                {
                    return NotFound();
                }

                guest.Name = updatedGuest.Name;
                guest.Email = updatedGuest.Email;
                guest.IsGoing = updatedGuest.IsGoing;
                guest.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                return Ok(guest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Deletar um convidado
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuest(long id)
        {
            try
            {
                var guest = await _dbContext.Guests.FindAsync(id);
                if (guest == null)
                {
                    return NotFound();
                }

                _dbContext.Guests.Remove(guest);
                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Guest deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
