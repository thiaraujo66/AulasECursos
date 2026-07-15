# 🎓 School Manager API (Gestão de Cursos e Matrículas)

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-07405E?style=for-the-badge&logo=sqlite&logoColor=white)
![Swagger](https://img.shields.io/badge/-Swagger-%23Clojure?style=for-the-badge&logo=swagger&logoColor=white)
![Status](https://img.shields.io/badge/Status-Conclu%C3%ADdo-success?style=for-the-badge)

Uma API RESTful robusta, desenvolvida em **.NET 8**, projetada para gerenciar um ecossistema educacional completo. O projeto aplica as melhores práticas de desenvolvimento corporativo, incluindo controle de acesso baseado em papéis (RBAC), segurança via JWT, separação de responsabilidades (DTOs) e padronização de respostas.

## 📑 Índice
- [Funcionalidades](#-funcionalidades)
- [Tecnologias e Arquitetura](#-tecnologias-e-arquitetura)
- [Modelagem e Regras de Negócio](#-modelagem-e-regras-de-negócio)
- [Principais Endpoints](#-principais-endpoints)
- [Como Executar o Projeto](#-como-executar-o-projeto)
- [Como Autenticar e Testar](#-como-autenticar-e-testar)

---

## 🚀 Funcionalidades

### 🔐 Segurança e Autenticação
- Registro de novos usuários e Login com emissão de token **JWT Bearer**.
- Controle de acesso rigoroso por papéis (Roles): `Admin`, `Instructor` e `Student`.
- Tratamento seguro de chaves usando `User Secrets` e Variáveis de Ambiente (nenhuma credencial no repositório).

### 📚 Gestão de Cursos
- **Admin/Instructor:** Criação e atualização de cursos.
- **Admin:** Exclusão de cursos.
- **Público:** Listagem de cursos com paginação e filtros via *query string*, além do detalhamento de cursos específicos.

### 👨‍🎓 Gestão de Estudantes e Matrículas
- **Admin:** Criação de perfil vinculado a um usuário de autenticação, listagem global, desativação e remoção de perfis.
- **Student/Admin:** Atualização e visualização detalhada do próprio perfil.
- **Matrículas:** Aluno pode se matricular em um curso ativo. O sistema bloqueia matrículas duplicadas.
- **Histórico:** Listagem das próprias matrículas (ou visão global para o Admin).

---

## 🛠️ Tecnologias e Arquitetura

- **Framework Core:** .NET 8, ASP.NET Core Web API
- **Banco de Dados:** Entity Framework (EF) Core com SQLite (configurado para dev)
- **Autenticação:** ASP.NET Core Identity + JWT Bearer
- **Design de API:** DTOs (Data Transfer Objects) para isolamento de entidades, Controllers (ou Minimal APIs).
- **Documentação:** Swagger/OpenAPI 
- **Segurança Adicional:** HTTPS habilitado nativamente, políticas restritas de CORS.

---

## 🏗️ Modelagem e Regras de Negócio

O banco de dados foi estruturado com foco em integridade e performance, contendo:
- **Seed Idempotente:** Ao iniciar a aplicação/banco, o sistema garante a criação automática dos papéis (`Admin`, `Instructor`, `Student`) e de um usuário Administrador padrão, caso não existam.
- **Validações de Domínio:** 
  - Títulos de curso exigem no mínimo **3 caracteres**.
  - Validação rigorosa de formato de e-mail.
- **Constraints (EF Core Fluent API):**
  - O e-mail do estudante é **único** (Unique Index).
  - A combinação `EstudanteId + CursoId` na tabela de matrículas é **única**, garantindo integridade e impedindo duplicação direto no banco de dados.
- **Tratamento de Erros:** Exceções capturadas globalmente (Global Exception Handling) gerando respostas padronizadas (ex: *ProblemDetails*) com status HTTP adequados (400, 401, 403, 404, 500).

---

## 📍 Principais Endpoints

Aqui está um resumo de como a API está estruturada (verifique o Swagger para detalhes de Payload e Query Strings):

| Rota | Método | Descrição | Acesso (Role) |
|---|---|---|---|
| `/api/auth/register` | POST | Registra um novo usuário | Público |
| `/api/auth/login` | POST | Autentica e retorna o JWT | Público |
| `/api/courses` | GET | Lista cursos (paginação/filtros) | Público |
| `/api/courses/{id}` | GET | Detalha um curso | Público |
| `/api/courses` | POST | Cria um novo curso | Admin, Instructor |
| `/api/courses/{id}` | PUT | Atualiza um curso | Admin, Instructor |
| `/api/courses/{id}` | DELETE | Remove um curso | Admin |
| `/api/students` | POST | Cria perfil de estudante | Admin |
| `/api/students/{id}` | GET | Detalha estudante | Admin, Student (próprio) |
| `/api/enrollments` | POST | Realiza matrícula em curso | Student (autenticado) |
| `/api/enrollments` | GET | Lista matrículas realizadas | Admin, Student (próprio) |

---

## 💻 Como Executar o Projeto

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- IDE sugerida: Visual Studio 2022, Rider ou VS Code.

### Passo 1: Clone o Repositório
```bash
git clone [https://github.com/seu-usuario/nome-do-repositorio.git](https://github.com/seu-usuario/nome-do-repositorio.git)
cd nome-do-repositorio
```

### Passo 2: Configure os Segredos (User Secrets)
A aplicação requer uma chave secreta para assinar os tokens JWT. Execute os comandos abaixo na pasta do projeto da API:
```bash
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:Secret" "sua-chave-super-secreta-de-pelo-menos-32-caracteres-aqui"
```

### Passo 3: Banco de Dados e Migrations
O projeto utiliza SQLite em ambiente de desenvolvimento. O EF Core criará o arquivo do banco automaticamente na raiz do projeto.
```bash
dotnet ef database update
```
*Nota: Este comando também aplicará o Seed inicial, inserindo os papéis do sistema e o usuário Admin padrão.*

### Passo 4: Iniciar a Aplicação
```bash
dotnet run
```
A API estará acessível em `https://localhost:<porta>`.

---

## 🔒 Como Autenticar e Testar via Swagger

A interface do Swagger já está configurada para suportar a injeção do Token JWT.

1. Navegue até `https://localhost:<porta>/swagger`.
2. **Faça o Login:** Utilize o endpoint `/api/auth/login` com as credenciais do usuário Admin gerado via Seed (ex: `admin@school.com` / `Admin@123`).
3. Copie o valor do token (`accessToken`) retornado no corpo da resposta.
4. No topo da página do Swagger, clique no botão **Authorize** (cadeado).
5. No campo de texto, insira: `Bearer SEU_TOKEN_COPIADO` (não esqueça da palavra "Bearer" e o espaço).
6. Clique em **Authorize**. Agora todas as suas requisições levarão o cabeçalho de segurança.
