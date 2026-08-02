---
description: Diseña y valida un cambio de modelo o migración EF Core para RealStatePortal.
argument-hint: Describe la entidad, relación, índice o restricción que necesitas cambiar.
agent: agent
---

Diseña el cambio de persistencia solicitado:

${input}

Contexto obligatorio:

- Usa SQL Server y EF Core code-first.
- Configura entidades exclusivamente mediante Fluent API.
- Mantén `Property` como Aggregate Root.
- Preserva integridad referencial, índices y restricciones explícitas.
- No mezcles configuraciones de EF Core con Domain.
- `PropertyImage` almacena URLs, no archivos binarios.
- Respeta los estados y la irreversibilidad de `Deleted`.

Procedimiento:

1. Revisa las entidades y configuraciones relacionadas.
2. Identifica el impacto en Application, Infrastructure y API.
3. Implementa el cambio mínimo compatible.
4. Genera o describe la migración necesaria.
5. Verifica que no exista borrado accidental ni pérdida de datos.

Validación esperada:

- `dotnet build src/RealStatePortal.slnx`.
- El comando `dotnet ef database update` correspondiente cuando exista una
  migración y la conexión esté disponible.

Responde con entidades/configuraciones modificadas, migración, riesgos de datos
y validación ejecutada.