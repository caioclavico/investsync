# InvestSync

Aplicação full-stack voltada para gerenciamento e simulação de investimentos financeiros, com arquitetura moderna baseada em microsserviços e mensageria com Apache Kafka.

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
```

## 🚀 Como executar

### Requisitos

- Node.js (v18+)
- .NET SDK 8
- Docker (para Kafka e bancos)
- Yarn ou npm

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Acesse em: [http://localhost:5173](http://localhost:5173)

### Backend

```bash
cd backend/InvestSync.Api
dotnet run
```

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
