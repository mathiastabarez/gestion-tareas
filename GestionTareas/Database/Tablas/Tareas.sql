CREATE TABLE [dbo].[Tareas]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Titulo] NVARCHAR(150) NOT NULL, 
    [Descripcion] NVARCHAR(1000) NOT NULL, 
    [Estado] NVARCHAR(20) NOT NULL, 
    [FechaCreacion] DATETIME2 NOT NULL, 
    [FechaVencimiento] DATE NULL,

    CONSTRAINT [CK_Tareas_Estado] CHECK ([Estado] IN ('Pendiente', 'EnCurso', 'Completada'))
);
