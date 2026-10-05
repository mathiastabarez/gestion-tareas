DECLARE @NumeroAleatorio INT =
    CHECKSUM(NEWID()) & 2147483647;

DECLARE @Estado NVARCHAR(20) = 'INVALIDA'   

INSERT INTO [dbo].[Tareas]
(
    [Titulo],
    [Descripcion],
    [Estado],
    [FechaCreacion],
    [FechaVencimiento]
)
OUTPUT INSERTED.*
VALUES
(
    CONCAT(N'Tarea de prueba ', @NumeroAleatorio),
    CONCAT(N'Descripción generada para la tarea ', @NumeroAleatorio),
    @Estado,
    SYSUTCDATETIME(),
    DATEADD(
        DAY,
        1 + (@NumeroAleatorio % 30),
        CAST(SYSUTCDATETIME() AS DATE)
    )
);