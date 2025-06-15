using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços Swagger
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

app.MapGet("/hello", () => "Hello World!");

app.Run();
