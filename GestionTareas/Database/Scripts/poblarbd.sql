DECLARE @Cantidad INT = 75;

;WITH Numeros AS
(
    SELECT 1 AS Numero

    UNION ALL

    SELECT Numero + 1
    FROM Numeros
    WHERE Numero < @Cantidad
)
INSERT INTO [dbo].[Tareas]
(
    [Titulo],
    [Descripcion],
    [Estado],
    [FechaCreacion],
    [FechaVencimiento]
)
SELECT
    CONCAT(
        N'TEST-',
        RIGHT(N'000' + CONVERT(NVARCHAR(3), Numero), 3),
        N' - ',
        CASE Numero % 5
            WHEN 0 THEN N'Preparar informe mensual'
            WHEN 1 THEN N'Revisar inventario'
            WHEN 2 THEN N'Contactar cliente'
            WHEN 3 THEN N'Actualizar proyecto'
            ELSE N'Comprobar factura'
        END
    ),
    CONCAT(
        N'Dato de prueba para ',
        CASE Numero % 4
            WHEN 0 THEN N'el equipo de ventas'
            WHEN 1 THEN N'la documentación interna'
            WHEN 2 THEN N'el seguimiento administrativo'
            ELSE N'una actividad urgente'
        END,
        N'. Registro número ',
        Numero,
        N'.'
    ),
    CASE Numero % 3
        WHEN 0 THEN N'Pendiente'
        WHEN 1 THEN N'EnCurso'
        ELSE N'Completada'
    END,
    DATEADD(
        MINUTE,
        -Numero,
        SYSUTCDATETIME()
    ),
    CASE
        WHEN Numero % 4 = 0 THEN NULL
        ELSE DATEADD(
            DAY,
            Numero % 15,
            CAST(
                DATEADD(
                    MINUTE,
                    -Numero,
                    SYSUTCDATETIME()
                )
                AS DATE
            )
        )
    END
FROM Numeros
OPTION (MAXRECURSION 1000);