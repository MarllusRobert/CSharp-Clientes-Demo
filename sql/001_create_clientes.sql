-- Migration idempotente: cria tabela clientes se não existir (SQL Server).
-- Pode rodar várias vezes sem erro.

IF OBJECT_ID(N'dbo.clientes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.clientes (
        id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        nome NVARCHAR(200) NOT NULL,
        documento NVARCHAR(50) NULL,
        cidade NVARCHAR(120) NULL,
        telefone NVARCHAR(40) NULL,
        email NVARCHAR(200) NULL,
        created_at DATETIME2 NOT NULL CONSTRAINT DF_clientes_created_at DEFAULT (SYSUTCDATETIME())
    );
END
GO
