# RealStatePortal

RealStatePortal is a real-estate portal organized with Clean Architecture.

## Architecture

- `RealStatePortal.Domain`: framework-independent business rules.
- `RealStatePortal.Application`: use cases, DTOs, and abstractions.
- `RealStatePortal.Infrastructure`: persistence and external integrations.
- `RealStatePortal.Api`: HTTP composition and delivery.
- `frontend`: React and TypeScript client.

## Local setup

The repository uses `mise` for the toolchain. Run `mise install` before building.

```sh
dotnet build RealStatePortal.slnx
cd frontend && pnpm install && pnpm build
```

To start SQL Server, create a local `.env` with `MSSQL_SA_PASSWORD` and run:

```sh
docker compose up -d
```

Never commit `.env` or credentials.