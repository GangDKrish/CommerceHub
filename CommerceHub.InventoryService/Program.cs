using CommerceHub.InventoryService.Data;
using CommerceHub.InventoryService.GrpcServices;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddGrpc();

// Ensure the gRPC endpoint is always available on a fixed HTTP/2 (TLS)
// port that the OrderService client targets (https://localhost:7179),
// independent of the selected launch profile.
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(7179, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        listenOptions.UseHttps();
    });
    options.ListenLocalhost(5232, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString(
            "InventoryDatabase")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CommerceHub.InventoryService v1");
    });
}

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<InventoryGrpcService>();

app.Run();
