using Swashbuckle.AspNetCore.SwaggerGen;
using InvestSync.Api.src.Interfaces;
using InvestSync.Api.src.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços Swagger
builder.Services.AddControllers();
builder.Services.AddSingleton<IUserRepositories, InMemoryUserRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

Console.WriteLine($"🚀 Iniciando API no ambiente: {app.Environment.EnvironmentName}");

// Middleware do Swagger (ambiente dev)
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("🌟 Configurando Swagger para ambiente de desenvolvimento...");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
