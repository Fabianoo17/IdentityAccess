# IdentityAccess
[![CI](https://img.shields.io/github/actions/workflow/status/Fabianoo17/IdentityAccess/.github/workflows/ci.yml?branch=main)](#)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)
![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)

API de autenticação em **.NET 8** com **JWT + Refresh Token (rotação e revogação)**, **SQL Server**, **EF Core**, **Controllers**, **Swagger**, **xUnit** e **CI (GitHub Actions)**.

## ✨ Funcionalidades
- Registro, login, refresh e logout.
- JWT com expiração exata (**ClockSkew = 0**).
- Senhas com **hash + salt** (ASP.NET Core Identity hasher).
- Migrações automáticas no startup (`Database.Migrate()`).

## 📦 Pré‑requisitos
- Docker + Docker Compose
- .NET SDK 8.0
- (CLI de migração) `dotnet tool update -g dotnet-ef --version 8.0.8`

## 🚀 Como rodar

docker compose up -d --build
# API: http://localhost:8080/swagger


