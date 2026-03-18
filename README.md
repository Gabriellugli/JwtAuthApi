# JwtAuthApi

API de autenticação desenvolvida em **ASP.NET Core** com **JWT (JSON Web Token)**, como parte do trabalho acadêmico da disciplina de laboratorio de sistemas 1.

---

## 🛠️ Tecnologias Utilizadas

- .NET 10
- ASP.NET Core Web API
- JWT Authentication
- Entity Framework Core
- PostgreSQL (Neon - cloud)
- BCrypt.Net (hash de senhas)
- Swagger (Swashbuckle)

---

## 📁 Estrutura do Projeto

```
JwtAuthApi/
├── Controllers/
│   ├── AuthController.cs       # Endpoints de autenticação
│   └── UsersController.cs      # Endpoints de usuários
├── Data/
│   └── AppDbContext.cs         # Contexto do banco de dados
├── Migrations/                 # Migrações do Entity Framework
├── Models/
│   ├── AuthResponse.cs         # Modelo de resposta da autenticação
│   ├── RefreshRequest.cs       # Modelo de requisição do refresh token
│   ├── User.cs                 # Modelo de usuário
│   └── UserDto.cs              # DTO de usuário (sem senha)
├── Services/
│   ├── TokenService.cs         # Geração e validação de tokens JWT
│   └── UserService.cs          # Gerenciamento de usuários
├── appsettings.json            # Configurações da aplicação
└── Program.cs                  # Configuração e inicialização da aplicação
```

---

## ⚙️ Funcionalidades

### ✅ Login
Autenticação de usuários com geração de tokens JWT.

**Endpoint:** `POST /Auth/login`

**Body:**
```json
{
  "username": "admin",
  "password": "SuaSenha@123"
}
```

**Resposta:**
```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "abc123...",
  "createdAt": "2026-03-17T08:00:00",
  "expiresAt": "2026-03-17T08:15:00"
}
```

---

### ✅ Refresh Token
Renovação do access token sem necessidade de novo login. O refresh token tem validade de **7 dias** e é descartado após o uso (rotação de token).

**Endpoint:** `POST /Auth/refresh`

**Body:**
```json
{
  "refreshToken": "abc123..."
}
```

**Resposta:**
```json
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "xyz789...",
  "createdAt": "2026-03-17T08:05:00",
  "expiresAt": "2026-03-17T08:20:00"
}
```

---

### ✅ Roles
Controle de acesso baseado em perfis de usuário:

- **admin** — acesso total, incluindo exclusão de usuários
- **user** — acesso básico, pode listar usuários

---

### ✅ Autorização por Endpoint
Endpoints protegidos com `[Authorize]` e `[Authorize(Roles = "admin")]`.

| Endpoint | Método | Autenticação | Role |
|---|---|---|---|
| /Auth/login | POST | ❌ | - |
| /Auth/refresh | POST | ❌ | - |
| /Users | GET | ✅ | qualquer |
| /Users | POST | ❌ | - |
| /Users | DELETE | ✅ | admin |

---

## 🔒 Segurança

- Senhas armazenadas com **hash BCrypt**
- Access token com expiração de **15 minutos**
- Refresh token com expiração de **7 dias**
- Validação de senha: mínimo **8 caracteres** e **1 caractere especial**
- Bloqueio de **usuários duplicados**
- Senhas **não retornadas** nas respostas da API (UserDto)
- **CORS** configurado para integração com frontend

---

## 🗄️ Banco de Dados

O projeto utiliza **PostgreSQL** hospedado na plataforma **Neon** (cloud). A conexão é configurada no `appsettings.json`.

O usuário `admin` é criado automaticamente na primeira execução caso não exista no banco.

---

## 🚀 Como Executar

### Pré-requisitos
- Visual Studio 2026
- .NET 10 SDK

### Passos

1. Clone o repositório:
```bash
git clone https://github.com/Gabriellugli/JwtAuthApi.git
```

2. Abra o projeto no Visual Studio

3. Configure a connection string no `appsettings.json` com suas credenciais do Neon

4. Execute o projeto (**F5**)

5. Acesse o Swagger em:
```
http://localhost:5247/swagger
```

---

## 📖 Como Usar o Swagger

1. Acesse `http://localhost:5247/swagger`
2. Faça login em `POST /Auth/login`
3. Copie o `accessToken` da resposta
4. Clique no botão **Authorize 🔒** no topo da página
5. Digite `Bearer SEU_TOKEN` e clique em **Authorize**
6. Agora você pode testar os endpoints protegidos

---

## 👥 Integração com Frontend (React)

O frontend deve fazer requisições HTTP para os endpoints da API. Para rotas protegidas, incluir o token no header:

```javascript
headers: {
  'Authorization': 'Bearer SEU_ACCESS_TOKEN'
}
```

### Exemplo de login em React:
```javascript
const response = await fetch('http://localhost:5247/Auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ username: 'admin', password: 'SuaSenha@123' })
});

const data = await response.json();
// Salvar data.accessToken e data.refreshToken
```

### Exemplo de requisição protegida:
```javascript
const response = await fetch('http://localhost:5247/Users', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${accessToken}`
  }
});
```

> ⚠️ **Importante:** O CORS está configurado para aceitar requisições de qualquer origem em ambiente de desenvolvimento.

---

## 📦 Pacotes NuGet Utilizados

| Pacote | Versão |
|---|---|
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.3 |
| Microsoft.AspNetCore.OpenApi | 10.0.3 |
| Microsoft.EntityFrameworkCore.Tools | 10.0.3 |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.3 |
| BCrypt.Net-Next | 4.0.3 |
| Swashbuckle.AspNetCore | 10.1.5 |
| System.IdentityModel.Tokens.Jwt | 8.16.0 |
| Microsoft.IdentityModel.Tokens | 8.16.0 |
