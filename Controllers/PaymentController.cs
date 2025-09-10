using Microsoft.AspNetCore.Mvc;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using MercadoPago.Client.Payment;
using System.Text.Json;

namespace WeddingMerchantApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
            MercadoPagoConfig.AccessToken = configuration["MercadoPago:AccessToken"];
        }

        public class PurchaseRequest
        {
            public string Title { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public string ItemId { get; set; } = string.Empty; // Id do item no seu banco
        }

        [HttpPost("create-preference")]
        public async Task<IActionResult> CreatePreference([FromBody] PurchaseRequest request)
        {
            try
            {
                var client = new PreferenceClient();
                var preferenceRequest = new PreferenceRequest
                {
                    Items = new List<PreferenceItemRequest>
                    {
                        new PreferenceItemRequest
                        {
                            Title = request.Title,
                            Quantity = request.Quantity,
                            CurrencyId = "BRL",
                            UnitPrice = request.Price
                        }
                    },

                    // importante para identificar o item no webhook
                    ExternalReference = request.ItemId, 

                    BackUrls = new PreferenceBackUrlsRequest
                    {
                        Success = "https://weddingwebsite-chi.vercel.app/merchant/success",
                        Failure = "https://weddingwebsite-chi.vercel.app/merchant/failure",
                        Pending = "https://weddingwebsite-chi.vercel.app/merchant/pending"
                    },
                    AutoReturn = "approved",

                    PaymentMethods = new PreferencePaymentMethodsRequest
                    {
                        DefaultPaymentMethodId = "pix"
                    },

                    // URL do webhook
                    NotificationUrl = "https://suaapi.com/api/payment/webhook"
                };

                Preference preference = await client.CreateAsync(preferenceRequest);

                return Ok(new { init_point = preference.InitPoint });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Novo endpoint para Webhook
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement payload)
        {
            try
            {
                if (!payload.TryGetProperty("type", out var typeProp))
                    return Ok(); // ignora se não tiver o tipo

                string type = typeProp.GetString() ?? "";

                if (type == "payment")
                {
                    string paymentId = payload.GetProperty("data").GetProperty("id").GetString();

                    var paymentClient = new PaymentClient();
                    var payment = await paymentClient.GetAsync(long.Parse(paymentId));

                    if (payment.Status == "approved")
                    {
                        string itemId = payment.ExternalReference;
                        string buyerEmail = payment.Payer.Email;
                        string buyerName = $"{payment.Payer.FirstName} {payment.Payer.LastName}";

                        // 🔽 Aqui você atualiza no banco
                        // Exemplo:
                        // await _db.UpdateItemAsSold(itemId, buyerName, buyerEmail);

                        Console.WriteLine($"✅ Item {itemId} vendido para {buyerName} ({buyerEmail})");
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
