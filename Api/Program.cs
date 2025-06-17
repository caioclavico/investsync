using Swashbuckle.AspNetCore.SwaggerGen;
using Api.Services;
using Api.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();


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
