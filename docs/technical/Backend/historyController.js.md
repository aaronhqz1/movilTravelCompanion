# backend/src/controllers/historyController.js

## Propósito
Gestiona el historial de consultas climáticas de un usuario: guarda una nueva consulta (evitando duplicados de la misma ciudad dentro de 24 horas) y expone dos formas de leer el historial (últimas 3 entradas, o todas).

## Tipo
Controller de Express.

## Dependencias
- Módulo interno: `../config/database` (`db`).
- No usa paquetes npm externos directamente (solo `sqlite3` indirectamente vía `db`).

## Endpoints expuestos (si aplica)
| Método | Ruta | Body/Query | Response | Descripción |
|---|---|---|---|---|
| POST | `/api/history` | `{ userId, city, latitude, longitude, temperature, humidity, wind_speed, weather_code }` | `201 { message, id }` / `409 { error }` / `400 { error }` | Guarda una consulta climática en el historial del usuario, rechazando duplicados de la misma ciudad en las últimas 24 horas. |
| GET | `/api/history/:userId/recent` | — | `200 [ ...hasta 3 filas ]` | Devuelve las 3 consultas más recientes del usuario, ordenadas por `query_time DESC`. |
| GET | `/api/history/:userId` | — | `200 [ ...todas las filas ]` | Devuelve el historial completo del usuario, ordenado por `query_time DESC` (sin límite). |

## Funciones exportadas
| Nombre | Firma/parámetros | Descripción |
|---|---|---|
| `saveHistory` | `(req, res) => void`, lee `req.body.{userId, city, latitude, longitude, temperature, humidity, wind_speed, weather_code}` | Verifica si ya existe una entrada de la misma `city` para ese `userId` en las últimas 24 horas; si existe, responde `409`; si no, inserta la fila nueva. |
| `getRecentHistory` | `(req, res) => void`, lee `req.params.userId` | `SELECT ... LIMIT 3` sobre `weather_history` filtrado por `user_id`. |
| `getAllHistory` | `(req, res) => void`, lee `req.params.userId` | `SELECT` sin límite sobre `weather_history` filtrado por `user_id`. |

## Esquema de datos (si aplica, ej. database.js)
No define esquema; opera sobre la tabla `weather_history` (ver `database.js.md` para columnas).

## Lógica y validaciones relevantes
- **Regla de no-duplicado en 24hs**: antes de insertar, ejecuta `SELECT id FROM weather_history WHERE user_id = ? AND city = ? AND datetime(query_time) > datetime('now', '-24 hours') ORDER BY query_time DESC LIMIT 1`. Si encuentra una fila, responde `409 { error: 'Ya guardaste esta ciudad recientemente. Intenta con otra ciudad.' }` sin insertar. La comparación de `city` es exacta (string igual), no normalizada (p. ej. "Madrid" vs "madrid" se tratarían como ciudades distintas).
- **Validación mínima de entrada**: `saveHistory` exige `userId` y `city`; el resto de los campos climáticos (`latitude`, `longitude`, `temperature`, `humidity`, `wind_speed`, `weather_code`) no se validan y pueden llegar `undefined` (se insertarían como `NULL` en SQLite).
- **Límite de 3 en `getRecentHistory`**: implementado directamente en SQL (`LIMIT 3`), coincide con la regla de negocio de CONTEXT.md ("Solo se muestran las últimas 3 búsquedas del historial en el dashboard").
- `getAllHistory` no tiene límite — existe como endpoint separado pero **no está documentado en el contrato de CONTEXT.md** (ver Notas de diseño).
- Ambos endpoints de lectura devuelven `400` si falta `userId` en la ruta, aunque en la práctica `userId` siempre viene de un parámetro de ruta (`:userId`), por lo que esa rama es difícil de alcanzar salvo con una llamada malformada.

## Relaciones
- Usa `../config/database` para todas las operaciones sobre `weather_history`.
- Sus tres funciones se importan y montan en `backend/src/routes/index.js`.
- Consumido desde el cliente MAUI por `movilTravelCompanion.Core/Services/HistoryService.cs` (vía `IHistoryService`), usado por `DashboardViewModel` para guardar cada nueva búsqueda y mostrar las últimas 3 en un `CollectionView`.

## Notas de diseño
- CONTEXT.md documenta explícitamente en "Endpoints del backend" solo `POST /api/history` y `GET /api/history/:userId/recent`; el endpoint `GET /api/history/:userId` (sin `/recent`, historial completo) existe en el código y está registrado en las rutas, pero no aparece en el contrato documentado de CONTEXT.md. Es una discrepancia menor: el endpoint es funcional, simplemente no está mencionado en la sección de contrato (posiblemente no usado aún por ningún `IHistoryService` del cliente).
- La regla "no duplicar ciudad en 24hs" está explícitamente listada en CONTEXT.md como "Reglas de negocio a replicar en el cliente" — el comentario ahí es "validar en backend, pero reflejar el estado en la UI", consistente con que el backend hace el rechazo autoritativo (409) y el cliente debe interpretarlo, no reimplementar la regla.
