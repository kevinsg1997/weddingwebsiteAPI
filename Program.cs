using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? Environment.GetEnvironmentVariable("SUPABASE_CONNECTION");

var mercadoPagoAccessToken = builder.Configuration["MercadoPago:AccessToken"]
                              ?? Environment.GetEnvironmentVariable("MERCADO_PAGO_ACCESS_TOKEN");

MercadoPagoConfig.AccessToken = mercadoPagoAccessToken;

var mercadoPagoTestAccessToken = builder.Configuration["MercadoPagoTest:AccessToken"]
                              ?? Environment.GetEnvironmentVariable("MERCADO_PAGO_TEST_ACCESS_TOKEN");
                              
MercadoPagoTestConfig.AccessToken = mercadoPagoTestAccessToken;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Wedding Merchant API",
        Version = "v1"
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wedding Merchant API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");
app.Run();