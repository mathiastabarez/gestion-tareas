# Alcance

## Objetivo

Crear una aplicación web sencilla para registrar, consultar y gestionar tareas personales.

## Usuario

La primera versión está destinada a un único usuario y no requiere inicio de sesión.

## Funcionalidades incluidas

- Crear tareas.
- Ver una lista paginada de tareas.
- Buscar tareas por título o descripción.
- Filtrar tareas por estado.
- Ver el detalle de una tarea.
- Editar tareas.
- Eliminar tareas con confirmación.
- Cambiar el estado de una tarea.
- Validar los datos obligatorios.

## Paginación y filtros

- La búsqueda por texto se aplicará al título y a la descripción.
- El filtro por estado será opcional.
- La página inicial será `1`.
- El tamaño predeterminado será de `10` tareas.
- El tamaño máximo será de `50` tareas.
- La lista se ordenará por fecha de creación descendente.
- Al cambiar de página se conservarán la búsqueda y el filtro seleccionados.

## Datos de una tarea

- Título obligatorio.
- Descripción opcional.
- Estado.
- Fecha de creación.
- Fecha de vencimiento opcional.

## Estados

- Pendiente.
- En curso.
- Completada.

Las tareas nuevas se crean con estado `Pendiente`.

## Restricciones técnicas

- La interfaz se desarrollará con ASP.NET Core MVC y vistas Razor.
- La aplicación tendrá una API REST independiente.
- La interfaz consumirá la API mediante HTTP y JSON.
- El dominio y los servicios de aplicación estarán separados de HTTP y del acceso a datos.
- Los datos se almacenarán en SQL Server.
- El esquema se administrará mediante un SQL Database Project.
- El acceso a datos se implementará con `Microsoft.Data.SqlClient` y consultas SQL parametrizadas.
- Se incluirán pruebas unitarias del dominio y de los servicios.

## Fuera del alcance

- Registro e inicio de sesión.
- Múltiples usuarios.
- Proyectos, etiquetas y subtareas.
- Notificaciones.
- Archivos adjuntos.
- Filtros por fechas.
- Ordenamiento configurable por el usuario.
- Pruebas de interfaz y pruebas de integración con SQL Server.
- Despliegue en producción.
