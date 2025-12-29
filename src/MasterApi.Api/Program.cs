using MasterApi.Application;
using MasterApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services from other layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// 2. Add services for the API layer
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Configure the HTTP request pipeline.
// Enable Swagger only for Development and Staging environments
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Staging"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
