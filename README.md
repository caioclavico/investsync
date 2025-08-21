# InvestSync

Aplicação full-stack voltada para gerenciamento e simulação de investimentos financeiros, com arquitetura moderna baseada em microsserviços e mensageria com Apache Kafka.

## 🛠 Tecnologias Utilizadas

### Frontend

- [React](https://reactjs.org/) com [TypeScript](https://www.typescriptlang.org/)
- React Router DOM para navegação
- React Toastify para notificações
- Axios para requisições HTTP

### Backend

- [.NET 9 (C#)](https://dotnet.microsoft.com/)
- Apache Kafka (mensageria)
- JWT Authentication
- In-Memory Repository (desenvolvimento)

## 📁 Estrutura do Projeto

```bash
investsync/
├── Frontend/             # Aplicação React com TypeScript
│   ├── src/
│   │   ├── Pages/       # Componentes de páginas
│   │   ├── services/    # Serviços de API
│   │   ├── assets/      # Recursos estáticos
│   │   └── App.tsx
│   ├── Dockerfile       # Docker para frontend
│   └── package.json
├── Api/                 # API principal em .NET
│   ├── src/
│   │   ├── Controllers/ # Controladores da API
│   │   ├── Models/      # Modelos de dados
│   │   ├── Services/    # Serviços de negócio
│   │   └── DTOs/        # Objetos de transferência
│   └── Dockerfile       # Docker para API
├── Backend/             # Serviços de backend
│   ├── Worker/          # Worker para Kafka (FinnhubWorker)
│   ├── Processor/       # Processador de eventos
│   ├── Shared/          # Projeto compartilhado
│   └── docker-compose.yml # Kafka setup
├── docker-compose.yml   # Configuração completa
├── start.sh             # Script de início (Linux/Mac)
├── start.ps1            # Script de início (Windows)
└── README.md
```

## 🚀 Como executar

### Opção 1: Docker (Recomendado)

#### Requisitos

- Docker Desktop
- Docker Compose

#### Execução rápida

**Windows (PowerShell):**

```powershell
.\start.ps1
```

**Linux/Mac:**

```bash
./start.sh
```

#### Execução manual

```bash
# Construir e iniciar todos os serviços
docker-compose up --build -d

# Verificar status
docker-compose ps

# Ver logs
docker-compose logs -f

# Parar aplicação
docker-compose down
```

### Opção 2: Desenvolvimento local

#### Requisitos

- Node.js (v18+)
- .NET SDK 9
- Docker (apenas para Kafka)

#### Backend

```bash
# Iniciar Kafka
cd Backend
docker-compose up -d

# Iniciar API
cd ../Api
dotnet run

# Iniciar Worker (em outro terminal)
cd ../Backend/Worker
dotnet run

# Iniciar Processor (em outro terminal)
cd ../Backend/Processor
dotnet run
```

#### Frontend

```bash
cd Frontend
npm install
npm start
```

## 🌐 Acessos

Após executar a aplicação, você pode acessar:

- **Frontend**: http://localhost:3000
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Kafka**: localhost:9092

## 🔧 Funcionalidades

### ✅ Implementadas

- **Autenticação JWT**: Login e registro de usuários
- **Dashboard**: Interface principal com informações do usuário
- **Transações**: Depósito e saque de valores
- **Compra/Venda de Ações**: Sistema completo de investimentos
- **Kafka Integration**: Monitoramento de preços e eventos de transações
- **Cálculo de P&L**: Lucro/prejuízo automático nas vendas (FIFO)
- **Notificações**: Toast notifications para feedback do usuário

### 🚧 Em Desenvolvimento

- **Carteira de Investimentos**: Visualização detalhada das posições
- **Relatórios**: Análise de performance e histórico
- **Banco de Dados**: Migração para SQL Server/PostgreSQL
- **Testes**: Testes unitários e de integração

## 📊 Arquitetura

### Frontend (React + TypeScript)

- **Páginas**: Login, Registro, Dashboard
- **Serviços**: authService, transactionService
- **Roteamento**: React Router com rotas protegidas
- **Estado**: React Hooks para gerenciamento local

### Backend (.NET 9)

- **API**: RESTful com JWT Authentication
- **Kafka**: Produção e consumo de eventos
- **Worker**: FinnhubWorker para dados de mercado
- **Processor**: Processamento de eventos de transações
- **Repositórios**: In-Memory (desenvolvimento)
- **Serviços**: StockPriceSubscription, TransactionProducer

### Infraestrutura

- **Kafka**: Mensageria para eventos de preços e transações
- **Docker**: Containerização completa
- **Health Checks**: Monitoramento de saúde dos serviços

## 🛠 Comandos Úteis

```bash
# Ver logs de um serviço específico
docker-compose logs -f api
docker-compose logs -f frontend
docker-compose logs -f kafka
docker-compose logs -f worker
docker-compose logs -f processor

# Reconstruir um serviço específico
docker-compose up --build api

# Parar apenas um serviço
docker-compose stop frontend

# Remover todos os containers e volumes
docker-compose down -v

# Acessar container em execução
docker exec -it investsync-api bash
```

## 🐛 Troubleshooting

### Kafka não inicia

```bash
# Limpar volumes do Kafka
docker-compose down -v
docker-compose up kafka -d
```

### Erro de build no Frontend

```bash
# Limpar cache do npm
cd Frontend
npm cache clean --force
npm install
```

### API não conecta no Kafka

- Verificar se o Kafka está rodando: `docker-compose ps`
- Verificar logs do Kafka: `docker-compose logs kafka`

## 📝 Variáveis de Ambiente

### API

- `JWT_KEY`: Chave secreta para JWT
- `JWT_ISSUER`: Emissor do JWT
- `ASPNETCORE_ENVIRONMENT`: Ambiente de execução

### Frontend

- `REACT_APP_API_URL`: URL da API backend

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.
npm run dev

````

Acesse em: [http://localhost:5173](http://localhost:5173)

### Backend

```bash
cd backend/InvestSync.Api
dotnet run
````

Acesse a API em: [https://localhost:5001](https://localhost:5001)

---

## 📌 Objetivos do Projeto

- ✅ Interface moderna e responsiva com Syncfusion
- ✅ Backend com APIs RESTful e integração Kafka
- 🚧 Persistência com SQL Server e Cassandra
- 🚧 Dashboard para análise de investimentos
- 🚧 Integração com corretoras via FIX (futuramente)

---

## 🧠 Conceitos Implementados

- Clean Architecture no backend
- Separação de domínios: comandos, eventos e mensagens
- Frontend desacoplado com tipagens fortes (TS)
- Integração assíncrona via Kafka
- Componentização no React com Syncfusion

---

## 📌 Issues em aberto

Veja as [issues](https://github.com/caioclavico/investsync/issues) para acompanhar o progresso ou contribuir com sugestões!

---

## 🤝 Contribuição

Contribuições são bem-vindas! Para contribuir:

1. Faça um fork do repositório
2. Crie uma branch com sua feature: `git checkout -b minha-feature`
3. Commit suas mudanças: `git commit -m 'feat: nova funcionalidade'`
4. Push para sua branch: `git push origin minha-feature`
5. Abra um Pull Request

---

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.
