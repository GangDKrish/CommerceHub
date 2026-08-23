using CommerceHub.InventoryService;
using CommerceHub.OrderService.Data;
using CommerceHub.OrderService.Messaging;
using CommerceHub.OrderService.Services;
using CommerceHub.ProductService;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString(
            "OrderDatabase")));

builder.Services.AddGrpcClient<Inventory.InventoryClient>(options =>
{
    options.Address = new Uri("https://localhost:7179");
});

builder.Services.AddGrpcClient<Product.ProductClient>(options =>
{
    options.Address = new Uri("https://localhost:7178");
});

builder.Services.AddScoped<IInventoryClient, InventoryGrpcClient>();
builder.Services.AddScoped<IProductClient, ProductGrpcClient>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddSingleton<IOrderMessageQueue, OrderMessageQueue>();
builder.Services.AddHostedService<OrderMessageBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CommerceHub.OrderService v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
