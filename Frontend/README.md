# 🔐 Sistema de Autenticação - InvestSync Frontend

## 📱 Telas Implementadas

### Tela de Login (`/login`)

- **Campos:** E-mail e Senha
- **Validações:** E-mail obrigatório, senha obrigatória
- **Funcionalidades:**
  - ✅ Formulário responsivo e moderno
  - ✅ Estados de loading
  - ✅ Tratamento de erros
  - ✅ Botão para ir para cadastro
  - ✅ Link "Esqueceu sua senha?" (placeholder)
  - ✅ Animações suaves

### Tela de Cadastro (`/register`)

- **Campos:** Nome, E-mail, Senha, Confirmar Senha
- **Validações:**
  - ✅ Nome obrigatório
  - ✅ E-mail válido e obrigatório
  - ✅ Senha mínima de 6 caracteres
  - ✅ Confirmação de senha deve coincidir
- **Funcionalidades:**
  - ✅ Formulário responsivo
  - ✅ Validação em tempo real
  - ✅ Estados de loading
  - ✅ Botão para voltar ao login
  - ✅ Design moderno com gradientes

## 🚀 Como Usar

### Rotas Disponíveis

```
/ → Redireciona para /login
/login → Tela de login
/register → Tela de cadastro
/home → Página inicial (placeholder)
* → Redireciona para /login
```

### Iniciar o Projeto

```bash
cd Frontend
npm start
```

### Navegação

- **Login → Cadastro:** Clique em "Cadastre-se aqui"
- **Cadastro → Login:** Clique em "Faça login aqui"
- **Após login/cadastro:** Será direcionado para `/home` (desenvolvimento)

## 🎨 Design e UX

### Características Visuais

- **Login:** Gradiente azul-roxo (`#667eea` → `#764ba2`)
- **Cadastro:** Gradiente roxo-azul (`#764ba2` → `#667eea`)
- **Cards:** Bordas arredondadas, sombras suaves
- **Animações:** Entrada suave (`slideUp`)
- **Responsivo:** Adapta-se a mobile e desktop

### Estados Interativos

- **Hover:** Efeitos de elevação nos botões
- **Focus:** Bordas coloridas nos inputs
- **Loading:** Botões desabilitados com texto dinâmico
- **Erro:** Mensagens de erro destacadas em vermelho

## 🔧 Estrutura de Arquivos

```
Frontend/src/
├── components/
│   ├── Login.tsx          # Componente de login
│   ├── Login.css          # Estilos do login
│   ├── Register.tsx       # Componente de cadastro
│   └── Register.css       # Estilos do cadastro
├── assets/logos/          # Logos do projeto
└── App.tsx               # Roteamento principal
```

## 📋 Próximos Passos

### Funcionalidades Pendentes

- [ ] Integração com API de autenticação
- [ ] Persistência de token JWT
- [ ] Proteção de rotas privadas
- [ ] Recuperação de senha
- [ ] Validação de e-mail
- [ ] Dashboard principal

### Melhorias de UX

- [ ] Feedback visual de sucesso
- [ ] Campos de senha com toggle de visibilidade
- [ ] Validação de força de senha
- [ ] Lembrar login (checkbox)
- [ ] Loading skeletons

## 🧪 Como Testar

### Cenários de Teste

#### Login

1. **Sucesso:** Preencha e-mail e senha → Clique "Entrar"
2. **Erro:** Deixe campos vazios → Veja validação HTML5
3. **Navegação:** Clique "Cadastre-se aqui" → Vai para `/register`

#### Cadastro

1. **Sucesso:** Preencha todos os campos válidos → Clique "Criar conta"
2. **Senha diferente:** Digite senhas diferentes → Veja erro
3. **Senha curta:** Digite menos de 6 caracteres → Veja erro
4. **Navegação:** Clique "Faça login aqui" → Vai para `/login`

### URLs de Teste

- http://localhost:3000/login
- http://localhost:3000/register
- http://localhost:3000/ (redireciona para login)

## 🔒 Segurança

### Implementações Atuais

- ✅ Validação client-side
- ✅ Sanitização de inputs
- ✅ Prevenção de submit duplo (loading states)

### Pendentes

- [ ] Validação server-side
- [ ] Hash de senhas (backend)
- [ ] Rate limiting
- [ ] CAPTCHA para múltiplas tentativas
- [ ] Criptografia de dados sensíveis
