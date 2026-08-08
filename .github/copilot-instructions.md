# Copilot Instructions for RealStatePortal

## Fuente de verdad

- Respetar `docs/RealStatePortal-AI-Project-Specification.md`.
- Usar `docs/solution-structure.md` para la estructura de la solución.
- Mantener los cambios pequeños, enfocados y listos para producción.
- Responder de forma breve y mencionar únicamente los archivos modificados y la
  validación relevante.

## Contexto técnico

- Backend: ASP.NET Core Web API sobre .NET 10.
- Arquitectura: Clean Architecture.
- Persistencia: EF Core y SQL Server mediante Docker.
- Frontend: React, TypeScript, Vite, React Router y React Context.
- Mapas: Leaflet detrás de una abstracción que permita migrar a Google Maps.
- Identidad: Auth0 con sincronización de usuarios y perfiles internos.
- Herramientas gestionadas por mise: .NET, Node y pnpm.

## Capas y dependencias

Mantener estos proyectos y responsabilidades:

- `RealStatePortal.Domain`: entidades, enums, value objects, eventos y reglas de
  negocio. No depende de frameworks, EF Core, Auth0 ni Infrastructure.
- `RealStatePortal.Application`: casos de uso, Application Services, DTOs,
  validaciones y abstracciones para persistencia, identidad, correo, tiempo y
  almacenamiento.
- `RealStatePortal.Infrastructure`: EF Core, SQL Server, repositorios,
  migraciones, Auth0, correo, auditoría y almacenamiento de imágenes.
- `RealStatePortal.Api`: Controllers, contratos HTTP, middleware, DI,
  autenticación y autorización. Los Controllers deben ser delgados.
- `frontend`: interfaz React organizada por features y contratos tipados.

No introducir CQRS/MediatR en V1. No mezclar Infrastructure con Domain ni colocar
reglas de negocio en Controllers.

## Reglas de dominio

- `Property` es el Aggregate Root.
- `PropertyAddress` y `PropertyImage` pertenecen al agregado `Property`.
- Un broker puede tener múltiples propiedades.
- Cada propiedad pertenece exactamente a un broker, pero puede transferirse.
- Estados válidos: `Draft`, `Published`, `Sold`, `Deleted`.
- Transiciones válidas: `Draft -> Published`, `Published -> Draft`,
  `Published -> Sold` y `Sold -> Deleted`.
- `Deleted` es irreversible y no admite restauración.
- Solo `Published` es visible en el catálogo público.
- Una propiedad `Sold` no se elimina inmediatamente.
- Los favoritos se conservan cuando una propiedad pasa a `Sold` o vuelve a
  `Draft`, mostrando su estado retirado cuando corresponda.

Las transiciones deben estar encapsuladas en `Property` y las operaciones
inválidas deben rechazarse desde Domain.

## Application y API

- Implementar funcionalidades mediante Application Services.
- Exponer DTOs; no devolver entidades de dominio directamente.
- Usar interfaces de Application para repositorios y servicios externos.
- Mantener validación de entrada, autorización y manejo uniforme de errores.
- No acceder directamente al `DbContext` desde Controllers.
- Mantener los contratos API compatibles con las funcionalidades de propiedades,
  búsqueda, favoritos, contacto, usuarios, brokers y auditoría.

## Persistencia e imágenes

- Configurar EF Core exclusivamente mediante Fluent API.
- Usar migraciones code-first para cambios de esquema.
- Mantener integridad referencial, índices y restricciones explícitas.
- `PropertyImage` almacena URLs, no archivos binarios.
- En desarrollo, guardar imágenes en `RealStatePortal.Api/wwwroot/uploads`.
- En producción, usar Azure Blob Storage mediante una abstracción de Application.
- Registrar auditoría de operaciones administrativas y cambios relevantes.

## Auth0 y autorización

- Auth0 es el proveedor de identidad, no la fuente única de permisos.
- Todas las credenciales, secretos y parámetros sensibles de Auth0, incluidos
  `client_id`, `client_secret`, dominios privados, tokens y contraseñas, deben
  residir exclusivamente en Backend/API o Infrastructure y provenir de
  configuración externa o variables de entorno.
- El frontend no puede contener, leer, compilar ni recibir credenciales,
  `client_id`, `client_secret`, contraseñas, tokens de acceso/refresco ni otros
  secretos. El frontend debe comunicarse con la API mediante contratos HTTP;
  cualquier flujo de autenticación sensible debe estar encapsulado por el
  backend.
- Mantener usuario, perfil, estado activo y roles internos en SQL Server.
- Usar claims, roles y policies de ASP.NET Core.
- Preservar los roles `Visitor`, `Registered User`, `Broker` y `Administrator`.
- Verificar que el usuario interno exista y esté activo antes de operaciones
  protegidas.
- Los brokers gestionan propiedades asignadas; los administradores gestionan
  usuarios, brokers, permisos y propiedades del sistema.
- Nunca confiar en identidad, rol o `BrokerId` enviados por el cliente.
- No guardar secretos, tokens ni credenciales en el repositorio.

## Frontend

## Frontend Coding Standards

### General

Todo el código frontend debe ser:

- Legible.
- Consistente.
- Fuertemente tipado.
- Listo para producción.
- Fácil de mantener.

### TypeScript

- Usar TypeScript en modo estricto.
- Evitar el uso de `any`.
- Preferir interfaces y tipos explícitos.
- Usar contratos tipados para las APIs.

### React Components

- Usar únicamente componentes funcionales.
- No usar componentes de clase.
- Mantener los componentes enfocados en una sola responsabilidad.
- Organizar los componentes por funcionalidad de negocio.

### Functions

Usar arrow functions por defecto.

Ejemplo preferido:

```ts
const getPropertyById = async (
  propertyId: number
): Promise<PropertyDto> => {
  return await apiClient.get<PropertyDto>(
    `/properties/${propertyId}`
  );
};
```

- Organizar el código por features: properties, favorites, brokers, users e
  inquiries.
- Usar props, respuestas API y variables de entorno tipadas.
- Mantener componentes simples y enfocados.
- Proteger rutas y acciones según el rol interno.
- Mantener Leaflet detrás de un proveedor de mapas intercambiable.
- No añadir pruebas frontend salvo solicitud explícita.

## Chat modes disponibles

Usar el mode especializado cuando corresponda:

- `Architecture Designer`
- `Domain Expert`
- `Application Services Developer`
- `EF Core Infrastructure Developer`
- `API Developer`
- `Auth0 Security Specialist`
- `Frontend React Developer`
- `Code Reviewer`
- `Troubleshooting Specialist`

El mode `Full-Stack Developer` puede utilizarse cuando el cambio atraviese varias
capas.

## Testing y validación

- No crear ni mantener proyectos de pruebas backend o archivos de pruebas
  frontend salvo solicitud explícita.
- Antes de finalizar cambios backend, ejecutar:
  `dotnet build src/RealStatePortal.slnx`.
- Si se modifican modelos o migraciones EF Core, ejecutar también el `dotnet ef
  database update` correspondiente.
- Para cambios frontend, ejecutar el script de build o typecheck definido en
  `frontend/package.json` cuando exista.
- Validar primero la comprobación más estrecha que pueda falsar la hipótesis del
  cambio.

## Roadmap y Git

Mantener el alcance de la fase actual:

1. Architecture Setup
2. Domain Layer
3. Application Layer
4. Infrastructure Layer
5. API Layer
6. Auth0 Integration
7. Frontend
8. Search
9. Favorites
10. Deployment

Mantener `main` estable y trabajar en branches de fase (`architecture`,
`backend`, `frontend`, `auth0`, `search`, `favorites` y `deployment`) según la
estrategia del proyecto. No crear commits ni branches automáticamente.

## Estilo de trabajo

- Investigar primero el archivo, símbolo o flujo responsable.
- Formular una hipótesis local y una validación barata antes de editar.
- Preferir el patrón existente sobre nuevas abstracciones.
- No revertir cambios del usuario.
- No dejar endpoints de depuración, código temporal o secretos.
- Documentar decisiones arquitectónicas importantes en `docs/`.