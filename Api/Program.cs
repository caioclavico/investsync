
using InvestSync.Api.src.Interfaces;
using InvestSync.Api.src.Repositories;
using InvestSync.Api.src.Extensions;
using InvestSync.Api.src.BO;
using InvestSync.Api.src.Middleware;

// Carrega variáveis do .env
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Lê as variáveis do ambiente
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? "chave-padrao";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "InvestSync";

builder.Services.AddControllers();
builder.Services.AddScoped<TransactionBO>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<ITransactionRepository, InMemoryTransactionRepository>();
builder.Services.AddJwtAuthentication(jwtKey, jwtIssuer);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();

// Configuração do CORS para permitir o frontend React e WebSocket
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Necessário para WebSocket
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar CORS
app.UseCors();

// Configurar WebSockets
app.UseWebSockets(new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromMinutes(2),
});
app.UseMiddleware<StockPriceWebSocketMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();