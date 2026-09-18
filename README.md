# C# WinForms — Cadastro de Clientes (MVP)

Demo **desktop em C#** para portfolio e entrevistas (alinhado a vagas .NET / WinForms / legado).

## O que faz

- Tela WinForms de cadastro de clientes
- CRUD completo (incluir / editar / excluir / listar)
- Providers: **SQLite** (padrão) e **SQL Server** (LocalDB / instância)
- SQL parametrizado + migration idempotente (`sql/001_create_clientes.sql`)
- Seed com 2 clientes demo
- Arquitetura **MVP** (View + Presenter + Repository + Interface)
- Testes unitários com **xUnit**

## Stack

- C# / .NET 8
- Windows Forms
- Microsoft.Data.Sqlite
- Microsoft.Data.SqlClient
- xUnit

## Como rodar (SQLite — padrão)

1. Instale o [.NET 8 SDK](https://dotnet.microsoft.com/download)
2. Duplo clique em `rodar.bat`
3. Ou: `dotnet run --project ClientesDemo`

## Como rodar com SQL Server

1. Tenha LocalDB ou SQL Server disponível
2. Em `ClientesDemo/appsettings.json`, altere:

```json
"Database": { "Provider": "sqlserver" }
```

3. Ajuste a connection string se necessário
4. Ou via ambiente:

```bat
set CLIENTES_DB_PROVIDER=sqlserver
set CLIENTES_SQLSERVER_CONNECTION=Server=(localdb)\MSSQLLocalDB;Database=ClientesDemo;Trusted_Connection=True;TrustServerCertificate=True;
rodar.bat
```

Script SQL idempotente: `sql/001_create_clientes.sql`

## Testes

```bat
dotnet test
```

## Estrutura

```
CSharp-Clientes-Demo/
├── ClientesDemo/
│   ├── Models/
│   ├── Data/          # SQLite + SQL Server + Factory
│   ├── Presentation/  # MVP
│   ├── MainForm.cs
│   └── appsettings.json
├── ClientesDemo.Tests/
├── sql/001_create_clientes.sql
├── rodar.bat
└── README.md
```

## Para entrevista

- OOP: classes, interface, factory de repositório
- WinForms desktop (stack clássica)
- SQL parametrizado (SQLite e SQL Server)
- Migration idempotente
- MVP + testes xUnit (repository e presenter)
- Disposição para evoluir base legada com organização

## LinkedIn

Abra `docs/apresentacao-linkedin.html`
