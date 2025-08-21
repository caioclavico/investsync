#!/bin/bash

echo "🚀 Iniciando InvestSync..."
echo "=========================="

# Verificar se o Docker está rodando
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker não está rodando. Por favor, inicie o Docker Desktop."
    exit 1
fi

# Parar containers existentes
echo "🧹 Limpando containers existentes..."
docker-compose down --remove-orphans

# Construir e iniciar os serviços
echo "🔨 Construindo e iniciando os serviços..."
docker-compose up --build -d

# Aguardar os serviços ficarem prontos
echo "⏳ Aguardando os serviços ficarem prontos..."
sleep 30

# Verificar status dos serviços
echo "📊 Status dos serviços:"
docker-compose ps

echo ""
echo "✅ InvestSync está rodando!"
echo "=========================="
echo "🌐 Frontend: http://localhost:3000"
echo "🔧 API: http://localhost:5000"
echo "📡 Kafka: localhost:9092"
echo "🔄 Worker: Rodando em background"
echo "⚙️ Processor: Rodando em background"
echo ""
echo "Para parar a aplicação, execute:"
echo "docker-compose down"
echo ""
echo "Para ver os logs, execute:"
echo "docker-compose logs -f [service-name]"
echo ""
echo "Serviços disponíveis:"
echo "- api, frontend, kafka, worker, processor"
