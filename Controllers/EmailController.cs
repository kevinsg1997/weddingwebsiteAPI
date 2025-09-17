using Microsoft.AspNetCore.Mvc;
using WeddingMerchantApi.Services;

namespace WeddingMerchantApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RSVPController : ControllerBase
    {
        private readonly EmailService _emailService;

        public RSVPController(IConfiguration configuration)
        {
            _emailService = new EmailService(configuration);
        }

        public class RSVPRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public bool Attending { get; set; }
            public string ItemName { get; set; } = string.Empty;
        }

        [HttpPost("confirmPurchase")]
        public async Task<IActionResult> ConfirmPurchase([FromBody] RSVPRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name))
                    return BadRequest(new { message = "Informe seu nome para identificação." });

                string htmlContent = $"<h1>Ai sim {request.Name}!</h1>" +
                                     $"<p>{request.Name} te deu um presente da loja, aproveite seu {request.ItemName}.<br>Email do convidado: {request.Email}</p>";

                bool sent = await _emailService.SendEmailAsync("", $"{request.Name} realizou uma compra na loja!", htmlContent);

                if (!sent) return StatusCode(500, new { message = "Erro ao enviar o e-mail." });

                return Ok(new { message = "Resposta enviada com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Agora que implementei um banco, não usarei mais esse endpoint.
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmAttendance([FromBody] RSVPRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name))
                    return BadRequest(new { message = "Informe seu nome para identificação." });

                // Para caso eu adicione um bd, posso utilizar.
                    string statusText = request.Attending ? "confirmou presença" : "não poderá comparecer";

                string htmlContent = $"<h1>{request.Name} respondeu ao seu convite!</h1>" +
                                     $"<p>{request.Name} {statusText} no seu casamento.<br>Email do convidado: {request.Email}</p>";

                bool sent = await _emailService.SendEmailAsync("", $"{request.Name} respondeu ao convite do casamento!", htmlContent);

                if (!sent) return StatusCode(500, new { message = "Erro ao enviar o e-mail." });

                return Ok(new { message = "Resposta enviada com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}