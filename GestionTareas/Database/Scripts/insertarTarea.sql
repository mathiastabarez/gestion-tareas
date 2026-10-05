DECLARE @NumeroAleatorio INT =
    CHECKSUM(NEWID()) & 2147483647;

DECLARE @Estado NVARCHAR(20) =
    CASE @NumeroAleatorio % 3
        WHEN 0 THEN N'Pendiente'
        WHEN 1 THEN N'EnCurso'
        ELSE N'Completada'
    END;

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