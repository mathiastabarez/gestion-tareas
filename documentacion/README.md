# Documentación

Esta carpeta contiene la documentación necesaria para desarrollar y mantener el proyecto.

La documentación debe ser breve, clara y mantenerse actualizada junto con el código.

## Estructura

```text
documentacion/
├── README.md
├── alcance.md
├── arquitectura.md
├── backlog.md
├── requisitos/
│   └── HU-NNN.md
├── tareas/
│   └── TEC-NNN.md
└── plantillas/
    ├── plantilla-historia-usuario.md
    └── plantilla-tec.md
```

## Tipos de documentos

### Historias de usuario

Las historias de usuario se identifican como `HU-NNN` y se guardan en `requisitos/`.

Describen una funcionalidad desde el punto de vista del usuario y contienen criterios de aceptación comprobables.

Ejemplo:

```text
HU-001 - Crear una tarea
```

### Tareas técnicas

Las tareas técnicas se identifican como `TEC-NNN` y se guardan en `tareas/`.

Incluyen configuración, arquitectura, base de datos, seguridad, pruebas y cualquier trabajo técnico que no represente directamente una funcionalidad para el usuario.

Ejemplo:

```text
TEC-001 - Crear la solución inicial
```

Si una tarea cambia la arquitectura, se debe actualizar también `arquitectura.md`.

## Estados

Las historias y tareas pueden tener los siguientes estados:

- `Pendiente`
- `En curso`
- `Completado`

Solo debe existir un estado vigente por elemento.

## Flujo de trabajo

1. Agregar el elemento a `backlog.md`.
2. Crear su archivo usando la plantilla correspondiente.
3. Cambiar su estado a `En curso` al comenzar.
4. Actualizar sus criterios o tareas durante el desarrollo.
5. Completar el apartado `Resultado`.
6. Cambiar su estado a `Completado` en el archivo y en el backlog.

## Reglas generales

- `backlog.md` muestra el estado actual del trabajo.
- `alcance.md` define qué incluye y qué no incluye el proyecto.
- `arquitectura.md` describe únicamente la arquitectura vigente.
- No guardar contraseñas, cadenas de conexión ni otros secretos.
- Actualizar la documentación en el mismo cambio que modifica el código.
