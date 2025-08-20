using Microsoft.AspNetCore.Mvc;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;

namespace WeddingMerchantApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        public PaymentController(IConfiguration configuration)
        {
            MercadoPagoConfig.AccessToken = configuration["MercadoPago:AccessToken"];
        }

        public class PurchaseRequest
        {
            public string Title { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
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
                    BackUrls = new PreferenceBackUrlsRequest
                    {
                        Success = "http://localhost:5173/success",
                        Failure = "http://localhost:5173/failure",
                        Pending = "http://localhost:5173/pending"
                    },
                    AutoReturn = "approved"
                };

                Preference preference = await client.CreateAsync(preferenceRequest);

                return Ok(new { init_point = preference.InitPoint });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}