USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'MecTecERP')
BEGIN
    CREATE DATABASE MecTecERP;
    PRINT 'Banco de dados MecTecERP criado com sucesso.';
END
ELSE
BEGIN
    PRINT 'Banco de dados MecTecERP já existe.';
END
GO

USE MecTecERP;
GO

-- Habilitar snapshot isolation se necessário para certas operações ou para evitar bloqueios.
-- Por padrão, não é estritamente necessário para Dapper, mas pode ser útil.
-- ALTER DATABASE MecTecERP SET ALLOW_SNAPSHOT_ISOLATION ON;
-- GO
-- ALTER DATABASE MecTecERP SET READ_COMMITTED_SNAPSHOT ON;
-- GO

PRINT 'Contexto alterado para MecTecERP.';
GO
