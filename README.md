# C# WinForms — Cadastro de Clientes (MVP)

Demo **desktop em C#** para portfolio e entrevistas (alinhado a vagas .NET / WinForms / legado).

## O que faz

- Tela WinForms de cadastro de clientes
- CRUD completo (incluir / editar / excluir / listar)
- SQLite local (`data/clientes.db`) com SQL parametrizado
- Seed com 2 clientes demo
- Arquitetura **MVP** (View + Presenter + Repository + Interface)

## Stack

- C# / .NET 8
- Windows Forms
- Microsoft.Data.Sqlite
- Padrão MVP + interface `IClienteRepository`

## Como rodar

1. Instale o [.NET 8 SDK](https://dotnet.microsoft.com/download)
2. Duplo clique em `rodar.bat`
3. Ou: `dotnet run --project ClientesDemo`

## Estrutura

```
CSharp-Clientes-Demo/
├── ClientesDemo/
│   ├── Models/Cliente.cs
│   ├── Data/IClienteRepository.cs
│   ├── Data/SqliteClienteRepository.cs
│   ├── Presentation/IClienteView.cs
│   ├── Presentation/ClientePresenter.cs
│   ├── MainForm.cs
│   └── Program.cs
├── rodar.bat
└── README.md
```

## Para entrevista

- OOP: classes, interface, injeção do repositório no presenter
- WinForms desktop (stack clássica)
- SQL parametrizado (sem concatenar valores)
- Separação View / Presenter / Data (MVP)
- Disposição para evoluir base legada com organização

## LinkedIn

Abra `docs/apresentacao-linkedin.html`
