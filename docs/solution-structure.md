# Estructura de la Solución

## Objetivo

RealStatePortal utiliza Clean Architecture para separar el dominio, los casos de
uso, la infraestructura, la API y el frontend. Cada capa tiene una responsabilidad
concreta y las reglas de negocio permanecen independientes de frameworks y bases
de datos.

## Estructura principal

```text
RealStateProtal/
├── RealStatePortal.slnx
├── Directory.Build.props
├── Directory.Packages.props
├── global.json
├── docker-compose.yml
├── README.md
├── docs/
├── src/
│   ├── RealStatePortal.Domain/
│   ├── RealStatePortal.Application/
│   ├── RealStatePortal.Infrastructure/
│   └── RealStatePortal.Api/
└── frontend/
```

## Dependencias entre capas

```text
Domain ← Application ← Infrastructure
                    ↑
                    Api

Frontend → Api
```

- `Domain` no depende de ningún proyecto externo.
- `Application` depende únicamente de `Domain`.
- `Infrastructure` implementa las abstracciones definidas por `Application`.
- `Api` compone la aplicación y expone los endpoints HTTP.
- `Frontend` consume la API mediante HTTP.

## Proyecto Domain

```text
src/RealStatePortal.Domain/
├── Common/
│   ├── AggregateRoot.cs
│   ├── DomainException.cs
│   └── Entity.cs
├── Entities/
│   ├── AuditLog.cs
│   ├── BrokerProfile.cs
│   ├── ContactInquiry.cs
│   ├── Favorite.cs
│   ├── Property.cs
│   ├── PropertyAddress.cs
│   ├── PropertyImage.cs
│   └── User.cs
├── Enums/
│   ├── EnergyClass.cs
│   ├── PropertyStatus.cs
│   ├── PropertyType.cs
│   ├── PropertySortOrder.cs
│   └── UserRole.cs
├── ValueObjects/
│   ├── Coordinates.cs
│   └── Money.cs
└── Events/
```

`Property` es el Aggregate Root. `PropertyAddress` y `PropertyImage` forman parte
del agregado y no deben modificarse ignorando sus reglas.

El ciclo de vida permitido es:

```text
Draft → Published
Published → Draft
Published → Sold
Sold → Deleted
```

Las operaciones `Publish`, `Withdraw`, `MarkAsSold` y `Delete` deben vivir en
`Property`. Las transiciones inválidas deben rechazarse mediante una excepción de
dominio. Una propiedad eliminada no puede restaurarse.

## Proyecto Application

```text
src/RealStatePortal.Application/
├── Abstractions/
│   ├── Authentication/
│   │   ├── ICurrentUserService.cs
│   │   └── IIdentityProvisioningService.cs
│   ├── Email/
│   │   └── IEmailSender.cs
│   ├── Persistence/
│   │   ├── IFavoriteRepository.cs
│   │   ├── IPropertyRepository.cs
│   │   ├── IUserRepository.cs
│   │   └── IUnitOfWork.cs
│   ├── Storage/
│   │   └── IImageStorage.cs
│   └── Time/
│       └── IDateTimeProvider.cs
├── Properties/
│   ├── Commands/
│   │   ├── CreateProperty/
│   │   ├── UpdateProperty/
│   │   ├── PublishProperty/
│   │   ├── WithdrawProperty/
│   │   ├── SellProperty/
│   │   ├── DeleteProperty/
│   │   └── ManagePropertyImages/
│   ├── Dtos/
│   └── Queries/
│       ├── GetPublishedProperties/
│       ├── GetPropertyById/
│       └── SearchProperties/
├── Favorites/
│   ├── AddFavorite/
│   ├── GetUserFavorites/
│   └── RemoveFavorite/
├── ContactInquiries/
│   └── CreateContactInquiry/
├── Users/
├── Brokers/
├── Auditing/
├── Authorization/
├── Common/
│   ├── Pagination.cs
│   ├── Result.cs
│   └── Validation/
└── DependencyInjection.cs
```

Application contiene los Application Services, DTOs, validaciones y la
orquestación de los casos de uso. No se utilizará CQRS/MediatR en V1.

Casos de uso principales:

- Crear, editar, publicar, retirar, vender y eliminar propiedades.
- Transferir una propiedad a otro broker.
- Gestionar imágenes de una propiedad.
- Buscar propiedades con búsqueda rápida, filtros y ordenamiento.
- Agregar, eliminar y listar favoritos.
- Registrar solicitudes de contacto y enviar el correo correspondiente.
- Gestionar usuarios, brokers y permisos.
- Registrar auditoría de operaciones relevantes.

## Proyecto Infrastructure

```text
src/RealStatePortal.Infrastructure/
├── Persistence/
│   ├── RealStatePortalDbContext.cs
│   ├── Configurations/
│   │   ├── AuditLogConfiguration.cs
│   │   ├── BrokerProfileConfiguration.cs
│   │   ├── ContactInquiryConfiguration.cs
│   │   ├── FavoriteConfiguration.cs
│   │   ├── PropertyAddressConfiguration.cs
│   │   ├── PropertyConfiguration.cs
│   │   ├── PropertyImageConfiguration.cs
│   │   └── UserConfiguration.cs
│   ├── Migrations/
│   ├── Repositories/
│   └── Seed/
├── Authentication/
│   ├── Auth0IdentityProvisioningService.cs
│   └── CurrentUserService.cs
├── Storage/
│   ├── AzureBlobImageStorage.cs
│   └── LocalImageStorage.cs
├── Email/
├── Auditing/
└── DependencyInjection.cs
```

Este proyecto implementa persistencia SQL Server con EF Core, repositorios,
autenticación Auth0, envío de correo, auditoría y almacenamiento de imágenes.
Toda configuración de EF Core debe utilizar Fluent API.

Durante el desarrollo las imágenes se almacenan en:

```text
src/RealStatePortal.Api/wwwroot/uploads/
```

En producción se utilizará Azure Blob Storage. La entidad `PropertyImage` solo
almacena las URLs de las imágenes.

## Proyecto Api

```text
src/RealStatePortal.Api/
├── Authorization/
│   ├── Policies.cs
│   └── Requirements/
├── Contracts/
│   ├── Brokers/
│   ├── Favorites/
│   ├── Properties/
│   └── Users/
├── Controllers/
│   ├── BrokersController.cs
│   ├── ContactInquiriesController.cs
│   ├── FavoritesController.cs
│   ├── PropertiesController.cs
│   └── UsersController.cs
├── Extensions/
├── Middleware/
│   ├── CorrelationIdMiddleware.cs
│   └── ExceptionHandlingMiddleware.cs
├── wwwroot/
│   └── uploads/
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

Los Controllers deben recibir solicitudes, aplicar autorización, invocar casos de
uso y devolver respuestas HTTP. No deben contener reglas de negocio.

Endpoints iniciales:

```text
GET    /api/properties
GET    /api/properties/{id}
POST   /api/properties
PUT    /api/properties/{id}
POST   /api/properties/{id}/publish
POST   /api/properties/{id}/withdraw
POST   /api/properties/{id}/sell
DELETE /api/properties/{id}

GET    /api/favorites
POST   /api/favorites/{propertyId}
DELETE /api/favorites/{propertyId}

POST   /api/contact-inquiries

GET    /api/brokers
POST   /api/brokers
PUT    /api/brokers/{id}

GET    /api/users
PUT    /api/users/{id}
```

## Frontend

```text
frontend/
├── package.json
├── vite.config.ts
├── tsconfig.json
├── .env.example
└── src/
    ├── app/
    │   ├── App.tsx
    │   ├── providers.tsx
    │   └── router.tsx
    ├── auth/
    ├── components/
    │   ├── common/
    │   ├── forms/
    │   ├── maps/
    │   └── properties/
    ├── features/
    │   ├── brokers/
    │   ├── favorites/
    │   ├── inquiries/
    │   ├── properties/
    │   └── users/
    ├── layouts/
    ├── maps/
    │   ├── LeafletMapProvider.ts
    │   ├── MapProvider.ts
    │   └── MapView.tsx
    ├── services/
    ├── styles/
    ├── types/
    └── main.tsx
```

El frontend utiliza React, TypeScript, Vite, React Router y React Context. Las
features deben organizar el código por capacidad de negocio. La integración de
mapas debe depender de `MapProvider`, permitiendo sustituir Leaflet por Google
Maps en el futuro.

## Seguridad

Auth0 gestiona la identidad, pero la aplicación mantiene usuarios y permisos
internos en SQL Server.

Roles de aplicación:

- `Visitor`
- `Registered User`
- `Broker`
- `Administrator`

La autorización se basa en claims, roles y policies de ASP.NET Core. La aplicación
debe conservar el modelo interno de permisos aunque Auth0 sea el proveedor de
identidad.

## Persistencia y reglas de visibilidad

- Un broker puede tener múltiples propiedades.
- Cada propiedad pertenece exactamente a un broker.
- Una propiedad tiene una dirección y múltiples imágenes.
- Un usuario puede tener múltiples favoritos.
- Una propiedad puede recibir múltiples solicitudes de contacto.
- Las operaciones relevantes generan registros de auditoría.
- Solo las propiedades `Published` aparecen en el catálogo público.
- Los favoritos se conservan cuando una propiedad pasa a `Sold` o vuelve a `Draft`.
- Una propiedad `Deleted` no puede restaurarse.

## Fases de implementación

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

La implementación debe mantener `main` estable y desarrollar cada fase en su
branch correspondiente según la estrategia definida en la especificación.