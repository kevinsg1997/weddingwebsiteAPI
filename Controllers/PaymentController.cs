using Microsoft.AspNetCore.Mvc;
using WeddingMerchantApi.Data;
using WeddingMerchantApi.Models;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _dbContext;

        public PaymentController(IConfiguration configuration, AppDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            // MercadoPagoConfig.AccessToken = configuration["MercadoPago:AccessToken"];
            MercadoPagoConfig.AccessToken = Environment.GetEnvironmentVariable("MERCADO_PAGO_ACCESS_TOKEN");
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

                    NotificationUrl = "https://weddingwebsiteapi-production.up.railway.app/api/payment/paymentWebhook"
                };

                Preference preference = await client.CreateAsync(preferenceRequest);

                return Ok(new { init_point = preference.InitPoint });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("paymentWebhook")]
        public async Task<IActionResult> PaymentWebhook([FromBody] JsonElement payload)
        {
            try
            {
                if (!payload.TryGetProperty("type", out var typeProp))
                    return Ok();

                string type = typeProp.GetString() ?? "";

                if (type == "payment")
                {
                    string paymentId = payload.GetProperty("data").GetProperty("id").GetString() ?? "";

                    Console.WriteLine($"🔔 Webhook recebido para o pagamento ID: {paymentId}");

                    string buyerName = "";

                    var paymentClient = new PaymentClient();
                    var payment = await paymentClient.GetAsync(long.Parse(paymentId));

                    if (payment.Status == "approved")
                    {
                        string itemId = payment.ExternalReference;

                        await _dbContext.UpdateItemAsSold(itemId, buyerName);

                        Console.WriteLine($"✅ Item {itemId} comprado.");

                        await NotifyClients(itemId, buyerName);
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Pagamento {paymentId} não aprovado. Status: {payment.Status}");
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        
        private static readonly List<HttpContext> _clients = new List<HttpContext>();
        private static readonly object _clientsLock = new();

        [HttpGet("events")]
        public async Task Events()
        {
            Response.ContentType = "text/event-stream";
            Response.StatusCode = 200;

            lock (_clientsLock)
            {
                _clients.Add(HttpContext);
            }

            try
            {
                while (!HttpContext.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                }
            }
            finally
            {
                lock (_clientsLock)
                {
                    _clients.Remove(HttpContext);
                }
            }
        }

        private async Task NotifyClients(string itemId, string buyerName)
        {
            var data = new
            {
                id = itemId,
                available = false,
                buyer = buyerName
            };

            var json = JsonSerializer.Serialize(data);

            foreach (var client in _clients.ToList())
            {
                try
                {
                    await client.Response.WriteAsync($"data: {json}\n\n");
                    await client.Response.Body.FlushAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending event: " + ex.Message);
                    _clients.Remove(client);
                }
            }
        }
    }
}
