# RealStatePortal AI Project Specification

## 1. Executive Summary
### 1.1 Purpose
RealStatePortal es una plataforma inmobiliaria web cuyo objetivo es permitir la publicación, búsqueda y gestión de propiedades inmobiliarias.

La plataforma deberá permitir:
- Visualización pública de propiedades.
- Gestión de propiedades por corredores inmobiliarios.
- Comunicación inicial entre potenciales clientes y corredores.
- Gestión de favoritos para usuarios registrados.
- Administración completa del sistema por administradores.

El sistema debe estar diseñado para soportar crecimiento futuro, manteniendo una arquitectura limpia, mantenible y preparada para asistencia mediante herramientas de Inteligencia Artificial

## 2. Product Vision
### 2.1 Visión
Convertirse en una plataforma inmobiliaria moderna que permita:
- Publicación eficiente de propiedades.
- Descubrimiento rápido de propiedades.
- Comunicación efectiva entre compradores y corredores.
- Administración sencilla de propiedades.

## 2.2 Objetivos de negocio
- Facilitar la búsqueda de propiedades.
- Incrementar los contactos entre clientes y corredores.
- Reducir el tiempo de publicación de nuevas propiedades.
- Mantener historial y trazabilidad de cambios.

## 3. Scope
### 3.1 In Scope
**Catálogo de propiedades**
- Listado de propiedades.
- Vista detallada.
- Galería de imágenes.
- Ubicación geográfica.

**Usuarios**
- Registro.
- Login.
- Gestión de favoritos.

**Brokers**
- Crear propiedades.
- Editar propiedades.
- Publicar propiedades.
- Marcar propiedades como vendidas.

**Administradores**
- Gestión de usuarios.
- Gestión de brokers.
- Gestión de propiedades.
- Auditoría.

**Búsquedas**
- Búsqueda rápida.
- Búsqueda avanzada

### 3.2 Out Of Scope
No forman parte de la versión inicial:
Telefonía
- Llamadas desde el portal.

Mensajería
- Chat interno.
- WhatsApp.
- SMS.

Sistema CRM Completo
- Seguimiento comercial avanzado.

La comunicación principal será por correo electrónico. 

## 4. Stakeholders
### 4.1 Visitor
Usuario no autenticado.

### 4.2 Registered User
Usuario autenticado.

### 4.3 Broker
Corredor inmobiliario.

### 4.4 Administrator
Administrador del sistema.

## 5. User Roles
### 5.1 Visitor
Puede:
- Buscar propiedades.
- Ver propiedades.
- Ver imágenes.
- Ver mapas.
- Contactar corredores.

No puede:
- Guardar favoritos.

### 5.2 Registered User
Puede:
- Todo lo anterior.
- Guardar favoritos.
- Eliminar favoritos.

### 5.3 Broker
Puede:
- Crear propiedades.
- Editar propiedades.
- Gestionar imágenes.
- Cambiar estados.
- Gestionar propiedades asignadas

### 5.4 Administrator
Puede:
- Todo lo que hace Broker.
- Crear brokers.
- Eliminar brokers.
- Eliminar propiedades.
- Gestionar usuarios.
- Gestionar permisos.

## 6. Functional Requirements
### FR-001 Property Listing
El sistema debe mostrar propiedades publicadas.
 
### FR-002 Property Details
El sistema debe mostrar:
- Título.
- Descripción.
- Imágenes.
- Dirección.
- Corredor.
- Mapa.

### FR-003 Favorites
Los usuarios autenticados deben poder:
- Agregar favoritos.
- Eliminar favoritos.
 
### FR-004 Contact Inquiry
Los visitantes deben poder contactar al corredor.
 
### FR-005 Property Administration
Los brokers deben poder:
- Crear propiedades.
- Modificar propiedades.
- Gestionar imágenes.
 
### FR-006 Search
La aplicación debe ofrecer:
- Búsqueda rápida.
- Búsqueda avanzada.

## 7. Business Rules
### BR-001
Un Broker puede tener múltiples propiedades. 
 
### BR-002
Una Property pertenece a exactamente un Broker pero puede ser transferida a otro Broker
 
### BR-003
Una propiedad en estado Draft no es visible públicamente. 
 
### BR-004
Una propiedad Published puede ser agregada a favoritos. 
 
### BR-005
Si una propiedad pasa a Sold debe seguir apareciendo en favoritos. 
 
### BR-006
Si una propiedad Published vuelve a Draft debe continuar apareciendo en favoritos indicando que fue retirada del catálogo. 
 
### BR-007
Una propiedad vendida no se elimina inmediatamente pero no se muestra a los visitantes del portal. 
 
### BR-008
Cuando finalizan todos los trámites legales la propiedad puede eliminarse definitivamente. 
 
### BR-009
Una propiedad eliminada no puede restaurarse. 
 
## 8. Property Lifecycle
Estados
1. Draft
2. Published
3. Sold
4. Deleted

Transiciones válidas
1. Draft -> Published
2. Published -> Draft
3. Published -> Sold
4. Sold -> Deleted

Transiciones inválidas
1. Deleted -> Sold
2. Deleted -> Draft
3. Deleted -> Published
 
## 9. Domain Model
Aggregate Root

1. Property

Será la principal entidad del negocio.
 
## 10. Entity Catalog
### Property
Campos:
1. Id
2. ReferenceNumber
3. Title
4. Description
5. Status
6. PropertyType
7. Price
8. Bedrooms
9. Bathrooms
10.	Rooms
11.	LivingArea
12.	TotalArea
13.	Floor
14.	NumberOfFloors
15.	ConstructionYear
16.	EnergyClass
17.	PublishedAt
18.	CreatedAt
19.	UpdatedAt
20.	BrokerId
 
### PropertyAddress
1. Id
2. PropertyId
3. Street
4. StreetNumber
5. PostalCode
6. City
7. Region
8. Country
9. Latitude
10.	Longitude
 
### PropertyImage
1. Id
2. PropertyId
3. Url
4. AltText
5. SortOrder
6. IsPrimary
 
### Favorite
1. Id
2. UserId
3. PropertyId
4. CreatedAt
 
### User
1. Id
2. Auth0UserId
3. Email
4. FirstName
5. LastName
6. IsActive
7. CreatedAt
8. UpdatedAt
 
### BrokerProfile
1. Id
2. UserId
3. FullName
4. Email
5. Phone
6. Bio
7. IsActive
 
### ContactInquiry
1. Id
2. PropertyId
3. VisitorName
4. VisitorEmail
5. VisitorPhone
6. Message
7. CreatedAt
 
### AuditLog
1. Id
2. EntityName
3. EntityId
4. Action
5. ChangedByUserId
6. ChangedAt
7. Details
 
## 11. Search Requirements
### Quick Search
Debe buscar simultáneamente en:
1. City
2. PostalCode
3. Street
4. PropertyTitle
5. Description
 
### Advanced Search
Filtros:
1. PropertyType
2. City
3. PriceMin
4. PriceMax
5. BedroomsMin
6. BathroomsMin
7. AreaMin
8. AreaMax
9. Status
 
### Sorting
1. Newest
2. Oldest
3. PriceLowToHigh
4. PriceHighToLow
 
## 12. Communication Model
La comunicación principal entre cliente y corredor será mediante correo electrónico.
El flujo de una solicitud de contacto será:

```text
Usuario
↓
Formulario del portal
↓
Guardar ContactInquiry en la base de datos
↓
Enviar correo al broker
↓
Enviar una copia del mismo mensaje al usuario
```

El portal registrará la solicitud antes de intentar enviar los correos. El correo
se enviará al broker responsable de la propiedad y el visitante recibirá una
copia mediante `Cc`. Si el registro en la base de datos falla, no se enviará
ningún correo. Las credenciales y configuración del proveedor de correo deben
provenir de variables de entorno o configuración externa.

El portal registrará solicitudes de contacto mediante:

1. ContactInquiry

No existirá sistema interno de mensajería. 
 
## 13. Security Model
### Authentication
Proveedor:
         Auth0
 
### Authorization
Roles:
1. Plain Text
2. Visitor
3. Broker
4. Administrator

Autorización basada en:
1. Claims
2. Roles
3. Policies
 
## 14. Architecture
### Arquitectura:
Clean Architecture
 
### Solution Structure
src/
     RealStatePortal.Domain
     RealStatePortal.Application
     RealStatePortal.Infrastructure
     RealStatePortal.Api
frontend
 
## 15. Backend Guidelines
### Tecnología:
1. .NET 10
2. ASP.NET Core
3. EF Core
4. SQL Server

### Enfoque:
Application Services
No se utilizará CQRS/MediatR en V1.
 
## 16. Frontend Guidelines
### Tecnologías:
1. React
2. TypeScript
3. Vite
4. React Router
5. React Context
6. Leaflet

La implementación de mapas debe abstraerse para permitir migrar posteriormente a Google Maps.

## Frontend Coding Standards

### General

All frontend code must be:

- Readable
- Consistent
- Strongly typed
- Production ready
- Easy to maintain

### TypeScript

- Use TypeScript strict mode.
- Avoid the use of `any`.
- Prefer explicit interfaces and types.
- Use typed API contracts.

### React Components

- Use functional components only.
- Do not use class components.
- Keep components focused on a single responsibility.
- Organize components by business feature.

### Functions

Use arrow functions by default.

Examples:

✅ Preferred

```ts
const getPropertyById = async (
  propertyId: number
): Promise<PropertyDto> => {
  return await apiClient.get<PropertyDto>(
    `/properties/${propertyId}`
  );
};
 
## 17. Image Storage Strategy
### Desarrollo
RealStatePortal.Api/wwwroot/uploads
 
### Producción
Azure Blob Storage

La tabla PropertyImage almacenará únicamente URLs.
 
## 18. Development Environment
### Sistema operativo:
macOS

### Herramientas globales:
1. Git
2. VS Code
3. Docker Desktop
4. mise

### Gestionadas por mise:
1. .NET
2. Node
3. pnpm
 
## 19. Git Strategy
1. main
2. architecture
3. backend
4. frontend
5. auth0
6. search
7. favorites
8. deployment

main debe permanecer siempre estable.
 
## 20. Testing Strategy
### Unit Tests
* Domain
* Application

### Integration Tests
* EF Core
* API

### Future
* End-to-End tests
 
## 21. GitHub Copilot Strategy
### Documentos principales:
.github/copilot-instructions.md
.github/chatmodes/
.github/prompts/

Toda generación de código debe respetar este documento como fuente de verdad.
 
## 22. Implementation Roadmap
### Phase 1
Architecture Setup
### Phase 2
Domain Layer
### Phase 3
Application Layer
### Phase 4
Infrastructure Layer
### Phase 5
API Layer
### Phase 6
Auth0 Integration
### Phase 7
Frontend: Crear diseño minimo
+----------------------------------+
| Logo | Menu                      |
+----------------------------------+
|                                  |
| Content Area                     |
|                                  |
+----------------------------------+
### Phase 8
Frontend: Validar Login y Logout de extremo a extremo.
Crear pagina temporal: /auth-test con botones [login] [logout] y mostrar:

- IsAuthenticated
- User Name
- Email
- Roles
- Auth0 User Id

Ademas:
- Test Public Endpoint
- Test Protected Endpoint
### Phase 9 - Navigation & Areas
- Public Area
- Private Area protegida por login, sin funcionalidad por el momento
### Phase 10 - Broker Portal
- Dashboard
- Properties
- Create Property
- Edit Property
- Property Images
- Inquiries
### Phase 11 - Administration
- Manage Brokers
- Manage Users
- Manage Properties
- Audit Logs
### Phase 12 - Public Catalog
- Home
- Property Search
- Property Detail
- Map View
- Contact Broker
### Phase 13 - Favorites
- My Favorites
- Add Favorites
- Remove Favorites
- Sold properties remain visible.
- Draft properties remain visible to previous owners.
### Phase 14 - Search
- Quick Search
- Advanced Search
- Sorting
### Phase 15
Deployment
 
## 23. Future Enhancements
* Google Maps Provider.
* Azure Search.
* Property comparison.
* Saved searches.
* Notifications.
* Analytics.
* Reporting.
 
## 24. AI Development Instructions
Toda IA utilizada en el proyecto debe:

1.	Respetar Clean Architecture.
2.	Mantener Property como Aggregate Root.
3.	Utilizar DTOs.
4.	No colocar reglas de negocio en Controllers.
5.	Utilizar Fluent API para EF Core.
6.	Respetar las reglas de negocio definidas en este documento.
7.	Generar código listo para producción.
8.	Explicar decisiones arquitectónicas importantes.