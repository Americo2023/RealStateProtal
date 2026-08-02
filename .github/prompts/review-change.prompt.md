---
description: Revisa un cambio de RealStatePortal buscando errores, regresiones y riesgos de seguridad.
argument-hint: Indica el cambio, archivo o conjunto de archivos que deseas revisar.
agent: agent
---

Revisa el cambio indicado:

${input}

Prioriza los hallazgos en este orden:

1. Errores funcionales y violaciones de reglas de negocio.
2. Fallos de autorización, exposición de datos o validación insuficiente.
3. Dependencias incorrectas entre capas.
4. Problemas de EF Core, migraciones o integridad referencial.
5. Regresiones en contratos API o frontend.
6. Validación ausente o insuficiente.

Comprueba específicamente:

- `Property` conserva sus invariantes y transiciones válidas.
- Solo `Published` aparece en el catálogo público.
- Los favoritos sobreviven a `Sold` y `Draft`.
- Los Controllers no contienen reglas de negocio ni acceden directamente al
  `DbContext`.
- La autorización usa los roles internos y verifica usuarios activos.
- EF Core usa Fluent API y los DTOs no exponen entidades de dominio.

Formato de salida:

- Hallazgos primero, ordenados por severidad.
- Cada hallazgo debe incluir archivo, línea, impacto y corrección propuesta.
- Después incluye preguntas abiertas, pruebas faltantes y un resumen breve.
- Si no encuentras problemas, dilo claramente e indica el riesgo residual.