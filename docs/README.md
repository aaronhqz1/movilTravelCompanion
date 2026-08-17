# Documentación — movilTravelCompanion

Índice general de la documentación del proyecto. Para la bitácora de decisiones técnicas y el contrato de API vivo, ver [`../CONTEXT.md`](../CONTEXT.md); para instalación y configuración del entorno, ver [`../README.md`](../README.md).

## Documentos de gestión

| Documento | Descripción |
|---|---|
| [PROJECT_SIGNOFF.md](./PROJECT_SIGNOFF.md) | Estado del proyecto, alcance cumplido, checklist de entregables, evidencia de pruebas, limitaciones conocidas y acta de aprobación |
| [MANUAL_USUARIO.md](./MANUAL_USUARIO.md) | Manual de usuario final — cómo usar la app, pantalla por pantalla, en lenguaje no técnico |

## Documentación técnica (por capa)

Un documento por clase/módulo, con propósito, dependencias, miembros públicos y relaciones con el resto del sistema.

| Capa | Índice | Contenido |
|---|---|---|
| Cliente MAUI | [technical/Client/INDEX.md](./technical/Client/INDEX.md) | Views, ViewModels, Controls, `App`/`AppShell`, `MauiProgram`, helpers de navegación |
| Core (.NET) | [technical/Core/INDEX.md](./technical/Core/INDEX.md) | Modelos de datos, servicios HTTP hacia el backend, configuración |
| Backend (Node/Express) | [technical/Backend/INDEX.md](./technical/Backend/INDEX.md) | Servidor, base de datos SQLite, controllers, rutas |

## Inconsistencias detectadas durante la documentación

Al redactar la documentación técnica se compararon el código real con el contrato descrito en `CONTEXT.md`. Diferencias encontradas (no bloquean el funcionamiento, pero conviene reconciliar `CONTEXT.md` o el código en una futura sesión):

- `POST /api/auth/register` solo usa `{ username, password }`; `homeCity` no se lee en el registro (se configura después vía `PUT /api/user/:userId/home`), aunque `CONTEXT.md` lo lista como parte del body.
- `weatherController.js` tiene **73** ciudades predefinidas, no 50 como indica `CONTEXT.md`.
- Existen tres endpoints en el código no documentados en el contrato de `CONTEXT.md`: `GET /api/weather/cities`, `GET /api/weather/stats`, `GET /api/history/:userId` (historial completo, sin límite de 3).
- `geocodeCity` está duplicada de forma independiente en `authController.js` y `weatherController.js`, con formatos de retorno ligeramente distintos.
- `ClothingRecommendation.cs` (cliente) no mapea `success`, `weather` ni `timestamp` de la respuesta del endpoint de IA — se descartan silenciosamente al deserializar.
- `HistoryService.AddAsync` no expone el `id` que devuelve `POST /api/history`, pese a que el backend sí lo incluye en la respuesta.
- `MainPage.xaml`/`.xaml.cs` es residuo del scaffolding inicial de `dotnet new maui`: no está registrada como `ShellContent` ni como ruta en `AppShell`, es código muerto (documentado igual, marcado como sin uso).

El detalle de cada punto está en el documento técnico correspondiente (ver tabla de arriba).
