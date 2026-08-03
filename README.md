# RealStatePortal

Arquitectura inicial de la plataforma inmobiliaria definida en la especificación
del proyecto.

## Estructura

- `src/RealStatePortal.Domain`: reglas y modelo de dominio.
- `src/RealStatePortal.Application`: casos de uso, DTOs y abstracciones.
- `src/RealStatePortal.Infrastructure`: persistencia e integraciones externas.
- `src/RealStatePortal.Api`: API HTTP y composición de dependencias.
- `frontend`: React, TypeScript y Vite organizado por features.

La descripción detallada está en [docs/solution-structure.md](docs/solution-structure.md).

## Herramientas

Las versiones de .NET, Node y pnpm se gestionan mediante mise.

## Comandos

```bash
mise exec -- dotnet build RealStatePortal.slnx
cd frontend
pnpm install
pnpm dev
```

Para levantar SQL Server localmente:

```bash
cp .env.example .env
docker compose up -d sqlserver
```

No se almacenan secretos en el repositorio. Sustituye los valores de los archivos
`.env` por credenciales locales antes de ejecutar servicios que las requieran.