# IdentityAccess
API de autenticação em **.NET 8** com **JWT + Refresh Token (rotação e revogação)**, **SQL Server**, **EF Core**, **Controllers**, **Swagger**, **xUnit** e **CI (GitHub Actions)**.

## Stack
.NET 8 • ASP.NET Core (Controllers) • JWT • EF Core • SQL Server • Swagger • xUnit • Docker

## Como rodar
```bash
docker compose up -d   # sobe SQL Server
dotnet restore
dotnet build
dotnet ef database update --project src/IdentityAccess.Infrastructure --startup-project src/IdentityAccess.Api
dotnet run --project src/IdentityAccess.Api

Acesse http://localhost:5189/swagger.

Segurança

Senhas com hash (ASP.NET Core Identity hasher)

JWT ClockSkew = 0

Refresh token com rotação e revogação

Segredos via User Secrets / variáveis de ambiente

Endpoints

POST /api/auth/register

POST /api/auth/login

POST /api/auth/refresh

POST /api/auth/logout

GET /api/profile (protegido)

Roadmap

 Policies/roles avançadas

 Rate limiting

 Logs estruturados (Serilog)
