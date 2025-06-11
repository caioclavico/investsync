# InvestSync

Aplicativo para gestão de ordens de fundos com atualização em tempo real.

## 🛠 Tecnologias Utilizadas

### Frontend
- [React](https://reactjs.org/) com [TypeScript](https://www.typescriptlang.org/)
- [Syncfusion React UI Components](https://www.syncfusion.com/react-components)
- Vite (em breve)
- Axios

### Backend
- [.NET 8 (C#)](https://dotnet.microsoft.com/)
- Apache Kafka (mensageria)
- SQL Server e Cassandra (em breve)

## 📁 Estrutura do Projeto

```bash
investsync/
├── frontend/             # Aplicação React (TS + Syncfusion)
│   ├── src/
│   │   ├── components/
│   │   ├── pages/
│   │   ├── services/
│   │   └── App.tsx
│   └── package.json
├── backend/              # APIs e serviços em .NET
│   ├── InvestSync.Api/
│   ├── InvestSync.Kafka/
│   └── ...
├── docker-compose.yml    # (em breve)
└── README.md
