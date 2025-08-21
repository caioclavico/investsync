Write-Host "🚀 Iniciando InvestSync..." -ForegroundColor Green
Write-Host "==========================" -ForegroundColor Green

# Verificar se o Docker está rodando
try {
    docker info | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Docker não está rodando"
    }
} catch {
    Write-Host "❌ Docker não está rodando. Por favor, inicie o Docker Desktop." -ForegroundColor Red
    exit 1
}

# Parar containers existentes
Write-Host "🧹 Limpando containers existentes..." -ForegroundColor Yellow
docker-compose down --remove-orphans

# Construir e iniciar os serviços
Write-Host "🔨 Construindo e iniciando os serviços..." -ForegroundColor Yellow
docker-compose up --build -d

# Aguardar os serviços ficarem prontos
Write-Host "⏳ Aguardando os serviços ficarem prontos..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Verificar status dos serviços
Write-Host "📊 Status dos serviços:" -ForegroundColor Cyan
docker-compose ps

Write-Host ""
Write-Host "✅ InvestSync está rodando!" -ForegroundColor Green
Write-Host "==========================" -ForegroundColor Green
Write-Host "🌐 Frontend: http://localhost:3000" -ForegroundColor Cyan
Write-Host "🔧 API: http://localhost:5000" -ForegroundColor Cyan
Write-Host "📡 Kafka: localhost:9092" -ForegroundColor Cyan
Write-Host "🔄 Worker: Rodando em background" -ForegroundColor Cyan
Write-Host "⚙️ Processor: Rodando em background" -ForegroundColor Cyan
Write-Host ""
Write-Host "Para parar a aplicação, execute:" -ForegroundColor Yellow
Write-Host "docker-compose down" -ForegroundColor White
Write-Host ""
Write-Host "Para ver os logs, execute:" -ForegroundColor Yellow
Write-Host "docker-compose logs -f [service-name]" -ForegroundColor White
Write-Host ""
Write-Host "Serviços disponíveis:" -ForegroundColor Yellow
Write-Host "- api, frontend, kafka, worker, processor" -ForegroundColor White
