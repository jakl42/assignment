using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Repository.Middleware;
using Serilog;
using Masking.Serilog;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add logger and enable password masking in logs
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Destructure.ByMaskingProperties("Password")
    .CreateLogger();

// Remove logging providers
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container
builder.Services.AddControllers();

// Add Http Context Accessor for logging client infor
builder.Services.AddHttpContextAccessor();

// Add database context and configure database connection
builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed Users database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

// Use API key authentication
app.UseMiddleware<ApiKeyMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();