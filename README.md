# InvestSync

Aplicativo para gestão de ordens de fundos com atualização em tempo real.

## 🛠️ Tecnologias
- C# (.NET 8) — API REST
- React + TypeScript — Frontend
- Material UI — Interface
- Apache Kafka — Mensageria
- Docker — Deploy local
- GitHub Projects — Planejamento

## 📆 Planejamento
Veja [o Project Board](https://github.com/seu-usuario/investsync/projects) com o planejamento semanal.

## Arquitetura do Projeto

                       +---------------------+
                       |     Front-end       |
                       |  (React + MUI)      |
                       +----------+----------+
                                  |
                                  v
                     REST/GraphQL API (HTTPS)
                                  |
                     +------------+------------+
                     |                         |
          +----------v----------+   +----------v----------+
          |    API Gateway /    |   |     Auth Service     |
          |   Backend (C# ASP)  |   |   (JWT/OAuth2)       |
          +----------+----------+   +----------+-----------+
                     |                         |
                     v                         v
          +---------------+--------------------------------+
          |          Application / Domain Layer            |
          +----------------------+-------------------------+
                                 |
                  +--------------+-------------+
                  |                            |
           +------v------+             +-------v-------+
           |   Kafka     |             | Persistence   |
           | (Producer / |             | (PostgreSQL,  |
           |  Consumer)  |             | Cassandra)    |
           +-------------+             +---------------+

O InvestSync é composto por uma arquitetura modular com front-end em React, API back-end em C#, mensageria com Kafka para eventos financeiros, e persistência de dados em PostgreSQL (relacional) e Cassandra (não relacional para eventos ou histórico).

- **Front-end**: Interface do usuário com React e Material UI.
- **Back-end**: ASP.NET Web API com autenticação JWT.
- **Mensageria**: Apache Kafka para comunicação assíncrona de eventos.
- **Persistência**: PostgreSQL para dados principais, Cassandra para logs/eventos históricos.

## 📁 Estrutura do projeto

/InvestSync
│
├── /frontend                # React app
│   ├── /public
│   ├── /src
│   │   ├── /components
│   │   ├── /pages
│   │   ├── /services        # chamadas à API
│   │   └── /utils
│   └── package.json
│
├── /backend                 # ASP.NET Web API
│   ├── /Controllers
│   ├── /Services
│   ├── /Domain              # Lógica de negócio
│   ├── /Infrastructure      # Kafka, DB, etc.
│   ├── /DTOs
│   ├── /Config
│   └── InvestSync.csproj
│
├── /migrations              # Scripts SQL ou CQL
│
├── /docs                    # Documentações, diagramas
│
├── /.github                 # Workflows CI/CD
│
├── docker-compose.yml       # Ambientes com Kafka, DBs, etc.
├── README.md
└── LICENSE
