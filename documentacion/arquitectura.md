# Arquitectura

## Objetivo

La aplicación permite gestionar tareas personales mediante una interfaz web con vistas Razor.

La Web se comunica con una API REST independiente. La API delega los casos de uso en la capa de aplicación, que utiliza el dominio y una abstracción de persistencia. La infraestructura implementa esa abstracción con SQL Server.

## Vista general

```text
Navegador
    ↓
GestionTareas.Web
    ↓ HTTP y JSON
GestionTareas.Api
    ↓
GestionTareas.Application
    ↓ ITareaRepository
GestionTareas.Infrastructure
    ↓ Microsoft.Data.SqlClient
SQL Server
```

`GestionTareas.Database` define el esquema de SQL Server y `GestionTareas.Tests` verifica el dominio y los servicios sin utilizar una base real.

## Solución

```text
GestionTareas.sln
├── GestionTareas.Web
├── GestionTareas.Api
├── GestionTareas.Application
├── GestionTareas.Domain
├── GestionTareas.Infrastructure
├── GestionTareas.Database
└── GestionTareas.Tests
```

## Dependencias

```text
GestionTareas.Web
    ↓ HTTP
GestionTareas.Api
    ├── GestionTareas.Application
    └── GestionTareas.Infrastructure

GestionTareas.Application
    ↓
GestionTareas.Domain

GestionTareas.Infrastructure
    ├── GestionTareas.Application
    └── GestionTareas.Domain

GestionTareas.Tests
    ├── GestionTareas.Api
    ├── GestionTareas.Application
    └── GestionTareas.Domain

GestionTareas.Database
    independiente
```

Reglas de dependencia:

- `Web` se comunica con la API únicamente mediante HTTP.
- `Web` no referencia a la API ni accede a SQL Server.
- `Domain` no depende de ningún otro proyecto de la solución.
- `Application` depende de `Domain` y define las interfaces necesarias.
- `Infrastructure` implementa las interfaces de `Application`.
- `Api` depende de `Application` y referencia `Infrastructure` para registrar sus implementaciones.
- Solo la composición de dependencias en `Program.cs` conoce la infraestructura concreta.
- `Database` es independiente y solo define el esquema SQL.

## Organización por funcionalidad

El código de cada proyecto se agrupará por funcionalidad. La primera funcionalidad será `Tareas`.

```text
Domain/Tareas/
Application/Tareas/
Infrastructure/Tareas/
Api/Tareas/
```

Si se agregan usuarios, proyectos o subtareas, se crearán módulos equivalentes únicamente en los proyectos que correspondan.

No se agregarán anticipadamente entidades, columnas ni abstracciones para funcionalidades futuras.

## GestionTareas.Domain

Biblioteca de clases que contiene el modelo y las reglas principales del negocio.

Estructura:

```text
GestionTareas.Domain/
└── Tareas/
    ├── Tarea.cs
    └── EstadoTarea.cs
```

### Tarea

`Tarea` representa la entidad principal del sistema.

La entidad será responsable de:

- Crear una tarea con estado inicial `Pendiente`.
- Registrar la fecha de creación en UTC.
- Validar y normalizar sus datos.
- Actualizar sus campos editables sin modificar el identificador ni la fecha de creación.

### EstadoTarea

Define los estados permitidos:

- `Pendiente`.
- `EnCurso`.
- `Completada`.

El dominio no conoce HTTP, DTOs, paginación, SQL Server ni `Microsoft.Data.SqlClient`.

## GestionTareas.Application

Biblioteca de clases que contiene los casos de uso, modelos de entrada e interfaces requeridas por la aplicación.

Estructura:

```text
GestionTareas.Application/
├── DependencyInjection.cs
├── Common/
│   └── ResultadoPaginado.cs
└── Tareas/
    ├── Exceptions/
    │   └── TareaNoEncontradaException.cs
    ├── Interfaces/
    │   ├── ITareaService.cs
    │   └── ITareaRepository.cs
    ├── Models/
    │   ├── CrearTareaInput.cs
    │   ├── ActualizarTareaInput.cs
    │   └── BuscarTareasInput.cs
    └── TareaService.cs
```

### Models

- `CrearTareaInput` contiene los datos necesarios para crear una tarea.
- `ActualizarTareaInput` contiene los datos editables de una tarea.
- `BuscarTareasInput` contiene texto, estado, página y tamaño de página.
- `ResultadoPaginado<T>` contiene elementos, página, tamaño, total de elementos y total de páginas.

Estos modelos no dependen de HTTP ni forman parte del contrato público de la API.

### ITareaService y TareaService

`ITareaService` define los casos de uso disponibles y `TareaService` los implementa.

Responsabilidades de `TareaService`:

- Crear tareas mediante las reglas del dominio.
- Consultar tareas paginadas.
- Consultar una tarea por identificador.
- Actualizar una entidad existente.
- Eliminar una tarea existente.
- Validar la paginación incluso cuando el caso de uso no se invoque desde HTTP.
- Recortar el texto de búsqueda y tratar un valor vacío como ausencia de filtro.
- Utilizar `ITareaRepository` sin conocer su implementación.

### ITareaRepository

Define las operaciones de persistencia necesarias:

- Buscar tareas aplicando texto, estado, orden y paginación, devolviendo `ResultadoPaginado<Tarea>`.
- Obtener una tarea por identificador.
- Crear una tarea y devolver su identificador generado.
- Actualizar una tarea.
- Eliminar una tarea.

Todos sus métodos serán asíncronos y aceptarán un token de cancelación.

No se utilizará un repositorio genérico. Cada funcionalidad tendrá contratos específicos para sus consultas y reglas.

### DependencyInjection

Expondrá un método para registrar los servicios de aplicación:

```csharp
builder.Services.AddApplication();
```

Este registro asociará `ITareaService` con `TareaService`.

## GestionTareas.Infrastructure

Biblioteca de clases que implementa el acceso a datos.

Estructura:

```text
GestionTareas.Infrastructure/
├── DependencyInjection.cs
└── Tareas/
    └── SqlTareaRepository.cs
```

### SqlTareaRepository

Implementa `ITareaRepository` mediante `Microsoft.Data.SqlClient`.

Responsabilidades:

- Abrir y cerrar conexiones con SQL Server.
- Ejecutar consultas SQL parametrizadas y asíncronas.
- Aplicar filtros opcionales por texto y estado.
- Ordenar por fecha de creación descendente y, en caso de empate, por identificador descendente.
- Aplicar paginación mediante `OFFSET` y `FETCH`.
- Obtener el total de resultados mediante `COUNT` y devolverlo junto con la página solicitada.
- Recuperar el identificador generado en las inserciones mediante `OUTPUT INSERTED.Id`.
- Convertir resultados de `SqlDataReader` en objetos `Tarea`.
- Convertir `EstadoTarea` entre enum y texto almacenado en SQL Server.

La consulta de elementos y la consulta del total utilizarán los mismos filtros y la misma conexión. Una página posterior a la última devolverá una colección vacía y conservará el total de elementos.

La búsqueda de texto se realizará con parámetros. La distinción entre mayúsculas y minúsculas seguirá la intercalación configurada en SQL Server.

Cada operación abrirá su propia conexión y la liberará al finalizar. El CRUD actual no requiere transacciones explícitas.

### DependencyInjection

Expondrá un método para registrar la infraestructura:

```csharp
builder.Services.AddInfrastructure(builder.Configuration);
```

Este registro asociará `ITareaRepository` con `SqlTareaRepository`.

Si en el futuro se utiliza Entity Framework, se reemplazará la implementación de infraestructura y su registro. El dominio, los servicios y los controladores permanecerán sin cambios mientras se conserve el contrato de `ITareaRepository`.

## GestionTareas.Api

API REST desarrollada con ASP.NET Core.

Estructura:

```text
GestionTareas.Api/
├── Tareas/
│   ├── TareasController.cs
│   ├── Dtos/
│   │   ├── BuscarTareasRequest.cs
│   │   ├── CrearTareaRequest.cs
│   │   ├── ActualizarTareaRequest.cs
│   │   ├── TareaResponse.cs
│   │   └── ResultadoPaginadoResponse.cs
│   └── TareaMapper.cs
└── GestionTareas.Api.http
```

### Dtos

Los DTOs definen el contrato HTTP:

- `BuscarTareasRequest` recibe texto, estado, página y tamaño de página desde la consulta HTTP.
- `CrearTareaRequest` recibe título, descripción y fecha de vencimiento.
- `ActualizarTareaRequest` recibe título, descripción, estado y fecha de vencimiento.
- `TareaResponse` devuelve todos los datos de una tarea.
- `ResultadoPaginadoResponse<T>` devuelve una página de resultados y sus metadatos.

Los DTOs de entrada validarán campos obligatorios, formatos, longitudes y valores de paginación.

### Mappers

`TareaMapper` convertirá:

- `BuscarTareasRequest` en `BuscarTareasInput`.
- `CrearTareaRequest` en `CrearTareaInput`.
- `ActualizarTareaRequest` en `ActualizarTareaInput`.
- `Tarea` en `TareaResponse`.
- `ResultadoPaginado<Tarea>` en `ResultadoPaginadoResponse<TareaResponse>`.

El mapper no asignará reglas de negocio, estado inicial ni fecha de creación.

El mapeo será manual. No se utilizará AutoMapper.

### Controller

`TareasController` recibirá las solicitudes HTTP, validará los DTOs, utilizará el mapper, llamará a `ITareaService` y devolverá la respuesta correspondiente.

No contendrá reglas de negocio ni consultas SQL.

## Contrato HTTP

```text
GET    /api/tareas?texto=&estado=&pagina=1&tamanioPagina=10
GET    /api/tareas/{id}
POST   /api/tareas
PUT    /api/tareas/{id}
DELETE /api/tareas/{id}
```

Reglas de consulta:

- `texto` es opcional y busca en título y descripción.
- `estado` es opcional.
- Si `pagina` se omite, su valor será `1`.
- `pagina` debe ser mayor o igual a `1`.
- Si `tamanioPagina` se omite, su valor será `10`.
- `tamanioPagina` debe estar entre `1` y `50`.
- Los valores inválidos devolverán `400 Bad Request`; no se corregirán silenciosamente.
- Una página posterior a la última devolverá `200 OK` con una colección vacía.

Comportamiento esperado:

- Listar devuelve `200 OK` con un resultado paginado.
- Consultar una tarea devuelve `200 OK` o `404 Not Found`.
- Crear devuelve `201 Created` con la ubicación del nuevo recurso, o `400 Bad Request`.
- Actualizar devuelve `204 No Content`, `400 Bad Request` o `404 Not Found`.
- Eliminar devuelve `204 No Content` o `404 Not Found`.
- Un error no controlado devuelve `500 Internal Server Error`.

Los errores utilizarán `ProblemDetails`. `TareaNoEncontradaException` se convertirá en `404 Not Found`. Los errores de argumentos o validación se convertirán en `400 Bad Request`. No se expondrán excepciones ni datos internos al cliente.

Al crear una tarea, el repositorio devolverá el identificador generado y el controlador responderá mediante `CreatedAtAction`, apuntando a `GET /api/tareas/{id}`.

La API utilizará OpenAPI y `GestionTareas.Api.http` para comprobar manualmente los endpoints.

## Representación del estado

- El dominio utiliza el enum `EstadoTarea`.
- La API envía y recibe `Pendiente`, `EnCurso` y `Completada` como texto JSON.
- La API configura la serialización de enums como texto.
- La Web tiene su propio enum equivalente dentro de sus DTOs.
- El repositorio almacena esos mismos valores como texto en SQL Server.
- La vista muestra `EnCurso` con la etiqueta visible “En curso”.

## GestionTareas.Web

Aplicación ASP.NET Core MVC con vistas Razor.

Estructura:

```text
GestionTareas.Web/
├── Controllers/
│   └── TareasController.cs
├── Dtos/
├── Mappers/
│   └── TareaViewModelMapper.cs
├── Models/
│   ├── ListadoTareasViewModel.cs
│   ├── TareaViewModel.cs
│   ├── CrearTareaViewModel.cs
│   └── EditarTareaViewModel.cs
├── Services/
│   ├── ITareasApiClient.cs
│   └── TareasApiClient.cs
└── Views/
    └── Tareas/
```

Responsabilidades:

- Mostrar las páginas de la aplicación.
- Recibir y validar los formularios.
- Consumir la API mediante `HttpClient`.
- Mostrar filtros y controles de paginación.
- Conservar los filtros al cambiar de página.
- Convertir DTOs en modelos para las vistas.
- Mostrar errores comprensibles al usuario.

`ListadoTareasViewModel` contendrá las tareas, los filtros aplicados y los metadatos de paginación.

`TareasApiClient` utilizará `HttpClient`, registrado mediante `IHttpClientFactory`, para serializar JSON, construir la consulta de búsqueda e interpretar las respuestas HTTP.

Se crearán vistas Razor para listar, ver, crear, editar y confirmar la eliminación de tareas.

Los formularios que modifican datos utilizarán protección antifalsificación.

## Reglas de las tareas

- El título es obligatorio, se guarda sin espacios exteriores y admite hasta 150 caracteres.
- La descripción es opcional, admite hasta 1000 caracteres y se guarda como `NULL` cuando está vacía.
- Una tarea nueva comienza en estado `Pendiente`.
- El usuario puede cambiar libremente entre los estados permitidos.
- La fecha de creación se registra en UTC y no puede editarse.
- La fecha de vencimiento es opcional y puede ser anterior a la fecha actual.
- Se permiten títulos repetidos.
- La eliminación es física y requiere confirmación previa en la interfaz Web.

## GestionTareas.Database

Proyecto de base de datos SQL Server con extensión `.sqlproj`.

Estructura:

```text
GestionTareas.Database/
├── GestionTareas.Database.sqlproj
└── dbo/
    └── Tables/
        └── Tareas.sql
```

Responsabilidades:

- Mantener la definición del esquema de la base `GestionTareas`.
- Validar los objetos SQL al compilar.
- Generar un archivo `.dacpac`.
- Publicar el esquema en SQL Server.

La tabla `Tareas` tendrá:

- `Id`: entero, clave primaria e identidad.
- `Titulo`: `nvarchar(150)` obligatorio.
- `Descripcion`: `nvarchar(1000)` opcional.
- `Estado`: `nvarchar(20)` obligatorio.
- `FechaCreacion`: `datetime2` obligatorio.
- `FechaVencimiento`: `date` opcional.

El esquema limitará `Estado` a `Pendiente`, `EnCurso` y `Completada`.

Se crearán índices para los campos utilizados habitualmente en filtros y ordenamiento cuando las mediciones demuestren que son necesarios. No se agregarán índices especulativos en la primera versión.

## Flujos principales

### Consultar tareas

1. El usuario ingresa una búsqueda, selecciona un estado o cambia de página.
2. El controlador MVC recibe los parámetros.
3. `TareasApiClient` realiza un `GET` conservando filtros y paginación.
4. El controlador de la API valida `BuscarTareasRequest`.
5. `TareaMapper` crea `BuscarTareasInput`.
6. `TareaService` valida la paginación, normaliza el texto de búsqueda y consulta el repositorio.
7. `SqlTareaRepository` ejecuta `COUNT` y la consulta paginada con los mismos filtros y orden estable.
8. La API devuelve el resultado paginado.
9. La Web crea `ListadoTareasViewModel` y muestra la página.

### Crear una tarea

1. El usuario completa el formulario Razor.
2. El controlador MVC valida el modelo.
3. El mapper de la Web crea el DTO para la API.
4. `TareasApiClient` envía una solicitud `POST`.
5. El controlador de la API valida `CrearTareaRequest`.
6. `TareaMapper` crea `CrearTareaInput`.
7. `TareaService` crea la entidad mediante las reglas del dominio.
8. `SqlTareaRepository` ejecuta un `INSERT` parametrizado con `OUTPUT INSERTED.Id`.
9. El servicio devuelve el identificador generado.
10. La API responde con `201 Created` y la ubicación de `GET /api/tareas/{id}`.
11. La Web redirige a la lista.

### Actualizar una tarea

1. La Web envía `ActualizarTareaRequest` mediante `PUT`.
2. El mapper de la API crea `ActualizarTareaInput`.
3. `TareaService` obtiene la entidad existente y aplica los cambios.
4. `SqlTareaRepository` ejecuta un `UPDATE` parametrizado.
5. La API devuelve `204 No Content`.

### Eliminar una tarea

1. La vista solicita confirmación al usuario.
2. La Web envía `DELETE` a la API.
3. `TareaService` comprueba que la tarea exista.
4. `SqlTareaRepository` ejecuta un `DELETE` parametrizado.
5. La API devuelve `204 No Content`.

## Manejo de errores y registro

- La API tendrá un manejador global de excepciones.
- Los errores de validación devolverán `400 Bad Request`.
- `TareaNoEncontradaException` devolverá `404 Not Found`.
- Los errores de argumentos o paginación devolverán `400 Bad Request`.
- Los errores inesperados se registrarán mediante `ILogger` y devolverán `500 Internal Server Error`.
- La Web mostrará un mensaje comprensible cuando la API no esté disponible.
- No se registrarán cadenas de conexión ni datos sensibles.

## Configuración

La API configurará:

- La cadena de conexión `ConnectionStrings:GestionTareas`.
- Los servicios de `Application` mediante `AddApplication`.
- La infraestructura mediante `AddInfrastructure`.
- La serialización de enums como texto.
- El manejo global de errores.
- OpenAPI.

La Web configurará:

- La dirección base de la API en `Api:BaseUrl`.
- `TareasApiClient` mediante `IHttpClientFactory`.

Las credenciales se almacenarán mediante secretos de usuario o variables de entorno y no se guardarán en Git.

El navegador envía formularios a la Web y el servidor Web llama a la API. Por ese motivo no será necesario configurar CORS.

La arquitectura no presupone una instancia local, rutas del equipo ni credenciales concretas. Los requisitos, valores de configuración y pasos para ejecutar o desplegar el sistema se documentarán en una guía separada.

## GestionTareas.Tests

Proyecto de pruebas unitarias.

Estructura:

```text
GestionTareas.Tests/
├── Domain/
├── Application/
├── Api/
└── Fakes/
    └── FakeTareaRepository.cs
```

`FakeTareaRepository` implementará `ITareaRepository` y permitirá probar los servicios sin depender de `Infrastructure` ni de SQL Server.

Se probarán como mínimo:

- Las reglas y normalizaciones de `Tarea`.
- La creación con estado inicial `Pendiente`.
- Las validaciones de `TareaService`.
- Los valores predeterminados, límites y páginas sin resultados.
- La normalización del texto de búsqueda.
- La conservación de filtros en las consultas al repositorio falso.
- La actualización y eliminación de tareas inexistentes.
- Las conversiones de `TareaMapper`.

Las pruebas de interfaz, del repositorio SQL, de integración y extremo a extremo quedan fuera de la primera versión.

Las consultas SQL se comprobarán manualmente ejecutando el flujo CRUD, los filtros y la paginación mediante OpenAPI o `GestionTareas.Api.http` después de publicar la base local.

## Extensibilidad

Para agregar una funcionalidad nueva:

1. Incorporar entidades y reglas en `Domain`, si son necesarias.
2. Incorporar casos de uso e interfaces en `Application`.
3. Implementar persistencia o servicios externos en `Infrastructure`.
4. Exponer contratos HTTP en `Api`.
5. Agregar interfaz de usuario en `Web`.
6. Actualizar el esquema en `Database`.
7. Agregar pruebas en `Tests`.

No todas las funcionalidades requieren cambios en todos los proyectos.

La extensibilidad se obtiene mediante dependencias claras y contratos específicos. No se crearán repositorios genéricos, entidades base ni campos reservados para necesidades desconocidas.

## Decisiones para la primera versión

- Las operaciones HTTP y SQL serán asíncronas.
- Se propagarán tokens de cancelación hasta el repositorio.
- El mapeo será manual.
- La API será la única entrada para las operaciones de la aplicación.
- El proyecto SQL será la fuente del esquema de la base de datos.
- No se implementarán autenticación, notificaciones ni procesos en segundo plano.
- No se utilizarán CQRS, MediatR, AutoMapper ni Unit of Work.
- No se agregarán caché, versionado de API ni comprobaciones de salud.