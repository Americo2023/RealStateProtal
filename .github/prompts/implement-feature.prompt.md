---
description: Implementa una funcionalidad vertical de RealStatePortal respetando Clean Architecture.
argument-hint: Describe la funcionalidad, el rol que la usa y el criterio de aceptación.
agent: agent
---

Implementa la funcionalidad solicitada en RealStatePortal:

${input}

Antes de editar:

1. Lee `docs/RealStatePortal-AI-Project-Specification.md` y
   `docs/solution-structure.md`.
2. Localiza el flujo y la capa responsable.
3. Formula una hipótesis local y una validación barata.

Implementación:

- Respeta Domain, Application, Infrastructure, API y frontend.
- Mantén `Property` como Aggregate Root y sus reglas en Domain.
- Usa Application Services, DTOs e interfaces de Application.
- Mantén los Controllers delgados.
- No introduzcas CQRS/MediatR ni dependencias innecesarias.
- No crees pruebas salvo que sean solicitadas explícitamente.

Validación:

- Ejecuta primero la comprobación más estrecha disponible.
- Ejecuta `dotnet build src/RealStatePortal.slnx` si el cambio afecta al backend.
- Ejecuta el build o typecheck de `frontend/package.json` si afecta al frontend.

Responde con los archivos modificados, la validación ejecutada y cualquier riesgo
pendiente.