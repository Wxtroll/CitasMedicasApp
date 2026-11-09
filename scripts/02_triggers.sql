USE CitasMedicas;
GO

IF OBJECT_ID('dbo.AuditoriaCitas', 'U') IS NULL
CREATE TABLE AuditoriaCitas (
    AuditoriaId INT IDENTITY(1,1) PRIMARY KEY,
    CitaId INT NOT NULL,
    EstadoAnterior NVARCHAR(15) NOT NULL,
    EstadoNuevo NVARCHAR(15) NOT NULL,
    FechaCambio DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    Usuario NVARCHAR(128) NOT NULL DEFAULT SYSTEM_USER
);
GO

CREATE OR ALTER TRIGGER trg_Citas_Auditoria ON Citas
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(Estado)
        INSERT INTO AuditoriaCitas (CitaId, EstadoAnterior, EstadoNuevo)
        SELECT i.CitaId, d.Estado, i.Estado
        FROM inserted i
        JOIN deleted d ON d.CitaId = i.CitaId
        WHERE i.Estado <> d.Estado;
END;
GO

CREATE OR ALTER TRIGGER trg_Consultas_ValidarCita ON Consultas
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted i JOIN Citas c ON c.CitaId = i.CitaId WHERE c.Estado = 'Cancelada')
        THROW 50001, 'No se puede registrar una consulta para una cita cancelada.', 1;
END;
GO
