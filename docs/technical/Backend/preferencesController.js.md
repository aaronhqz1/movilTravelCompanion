# backend/src/controllers/preferencesController.js

## Propósito
Lee y guarda las preferencias de usuario (estilo de vestimenta por defecto y sensibilidad al frío/calor) usadas para precargar la sección de recomendación de vestimenta del Dashboard, persistidas en la tabla `user_preferences`.

## Tipo
Controller de Express.

## Dependencias
- Módulo interno: `../config/database` (`db`).
- No usa paquetes npm externos directamente.

## Endpoints expuestos (si aplica)
| Método | Ruta | Body/Query | Response | Descripción |
|---|---|---|---|---|
| GET | `/api/user/:userId/preferences` | — | `200 { defaultClothingStyle, coldSensitivity }` | Devuelve las preferencias guardadas del usuario, o los defaults (`casual`/`normal`) sin crear fila si el usuario nunca guardó preferencias. |
| PUT | `/api/user/:userId/preferences` | `{ defaultClothingStyle?, coldSensitivity? }` | `200 { message, defaultClothingStyle, coldSensitivity }` / `400 { error }` | Crea o actualiza (upsert manual) la fila de preferencias del usuario, validando ambos valores. |

## Funciones exportadas
| Nombre | Firma/parámetros | Descripción |
|---|---|---|
| `getPreferences` | `(req, res) => void`, lee `req.params.userId` | `SELECT default_clothing_style, cold_sensitivity FROM user_preferences WHERE user_id = ?`; si no hay fila, devuelve `DEFAULT_PREFERENCES` (`{defaultClothingStyle: 'casual', coldSensitivity: 'normal'}`) sin insertar nada. |
| `updatePreferences` | `(req, res) => void`, lee `req.params.userId` y `req.body.{defaultClothingStyle, coldSensitivity}` | Valida ambos valores (con fallback a los defaults si vienen vacíos/falsy), luego decide `INSERT` o `UPDATE` según si ya existe una fila para ese `user_id`. |

## Esquema de datos (si aplica, ej. database.js)
No define esquema; opera sobre la tabla `user_preferences` (ver `database.js.md`: `user_id` PK, `default_clothing_style`, `cold_sensitivity`).

## Lógica y validaciones relevantes
- **Constantes de validación**: `VALID_CLOTHING_STYLES = ['casual', 'formal', 'deportivo']`, `VALID_COLD_SENSITIVITIES = ['friolento', 'normal', 'caluroso']` — mismos valores que `openaiController.js` (estilos unificados en español).
- **Defaults sin crear fila** (`getPreferences`): si el usuario nunca llamó a `updatePreferences`, no existe fila en `user_preferences` y el endpoint responde con los defaults en memoria (`DEFAULT_PREFERENCES`) en vez de insertar una fila por defecto — evita escrituras innecesarias solo por leer.
- **Upsert manual** (`updatePreferences`): dado que, según el comentario del código, "sqlite3 no soporta INSERT ... ON CONFLICT de forma consistente en todas las versiones", el patrón es: `SELECT` para verificar existencia → si existe, `UPDATE`; si no, `INSERT`. El comentario señala explícitamente que es "el mismo patrón que `saveHistory` en `historyController.js`".
- **Fallback silencioso a defaults en `updatePreferences`**: si `defaultClothingStyle` o `coldSensitivity` vienen vacíos/falsy en el body, se usa el default correspondiente ANTES de validar — es decir, un body vacío `{}` es válido y persiste los defaults explícitamente (a diferencia de `getPreferences`, que no persiste nada).
- **Validación de valores inválidos**: si el valor (ya sea el enviado o el default aplicado) no está en la lista blanca correspondiente, responde `400` con el mensaje de opciones válidas. En la práctica esto solo puede fallar si el cliente manda explícitamente un string fuera de la lista blanca (los defaults siempre son válidos).

## Relaciones
- Usa `../config/database` para todas las operaciones sobre `user_preferences`.
- Sus dos funciones se importan y montan en `backend/src/routes/index.js`.
- Consumido desde el cliente MAUI por `movilTravelCompanion.Core/Services/PreferencesService.cs` (vía `IPreferencesService`), usado por `PreferencesPage`/`PreferencesViewModel` (lectura/guardado directo) y por `DashboardViewModel.LoadAsync` (precarga `SelectedClothingStyle` y sensibilidad al frío como fallback silencioso si falla, sin bloquear el resto del Dashboard).

## Notas de diseño
- Módulo enteramente nuevo de la sesión 2026-08-09 (CONTEXT.md): "Backend: tabla `user_preferences` + `preferencesController.js` (GET/PUT), rutas registradas. Probado con `curl` (defaults sin fila previa, guardar, releer, rechazar valor inválido)." — es decir, el comportamiento de "defaults sin crear fila" fue verificado manualmente antes de integrarse al cliente.
- No existía nada equivalente en la app React original (`WeatherApp`); CONTEXT.md lo marca explícitamente como feature nueva sin contraparte en el frontend original ("Preferencias de usuario (nuevo, no existía en la web original)").
- El comentario sobre el patrón SELECT-then-INSERT/UPDATE remite a `historyController.js` como precedente — refleja una convención consistente en el backend para evitar depender de sintaxis `ON CONFLICT` de SQLite.
