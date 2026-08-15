# backend/src/routes/index.js

## Propósito
Punto único de definición del contrato REST del backend: importa las funciones de todos los controllers y las mapea a rutas HTTP concretas bajo el prefijo `/api` (montado desde `server.js`).

## Tipo
Definición de rutas.

## Dependencias
- `express` (^4.18.2) — `express.Router()`.
- Módulos internos: `../controllers/authController`, `../controllers/weatherController`, `../controllers/historyController`, `../controllers/openaiController`, `../controllers/preferencesController`.

## Endpoints expuestos (si aplica)
| Método | Ruta completa | Handler | Controller |
|---|---|---|---|
| POST | `/api/auth/register` | `register` | `authController.js` |
| POST | `/api/auth/login` | `login` | `authController.js` |
| PUT | `/api/user/:userId/home` | `updateHomeCity` | `authController.js` |
| GET | `/api/user/:userId/preferences` | `getPreferences` | `preferencesController.js` |
| PUT | `/api/user/:userId/preferences` | `updatePreferences` | `preferencesController.js` |
| GET | `/api/weather/random` | `getRandomWeather` | `weatherController.js` |
| GET | `/api/weather/search` | `searchWeather` | `weatherController.js` |
| GET | `/api/weather/coordinates` | `getWeatherByCoords` | `weatherController.js` |
| GET | `/api/weather/cities` | `getCitiesList` | `weatherController.js` |
| GET | `/api/weather/stats` | `getCitiesStats` | `weatherController.js` |
| POST | `/api/history` | `saveHistory` | `historyController.js` |
| GET | `/api/history/:userId/recent` | `getRecentHistory` | `historyController.js` |
| GET | `/api/history/:userId` | `getAllHistory` | `historyController.js` |
| POST | `/api/ai/clothing-recommendation` | `getClothingRecommendation` | `openaiController.js` |

(Ver Body/Query/Response detallados en el documento de cada controller respectivo.)

## Funciones exportadas
| Nombre | Firma/parámetros | Descripción |
|---|---|---|
| `router` (export por defecto de `module.exports`) | instancia de `express.Router` | Router con las 14 rutas listadas arriba, montado en `/api` desde `server.js` (`app.use('/api', routes)`). |

## Esquema de datos (si aplica, ej. database.js)
No aplica.

## Lógica y validaciones relevantes
- Este módulo no contiene lógica de negocio ni validaciones propias — es puramente declarativo (mapeo ruta → handler). Toda validación vive en los controllers respectivos.
- El orden de declaración de rutas agrupa por dominio (autenticación, preferencias, clima, historial, OpenAI), con comentarios separadores, pero Express no depende de ese orden para resolver estas rutas (no hay solapamientos ambiguos entre los patrones definidos).
- Nótese que `GET /api/history/:userId/recent` se declara antes que `GET /api/history/:userId` — orden correcto y necesario en Express, ya que si el patrón genérico `:userId` se declarara primero, capturaría también `/recent` como si fuera un `userId` literal.

## Relaciones
- Importa las cinco funciones de `authController.js`, cinco de `weatherController.js`, tres de `historyController.js`, una de `openaiController.js` y dos de `preferencesController.js`.
- Es importado y montado por `server.js` (`app.use('/api', routes)`).
- Define el contrato consumido en conjunto por todos los servicios de `movilTravelCompanion.Core/Services/`: `AuthService.cs`, `WeatherApiService.cs`, `HistoryService.cs`, `PreferencesService.cs` y `ClothingService.cs`.

## Notas de diseño
- Según CONTEXT.md, las rutas de `preferencesController.js` (`GET`/`PUT /api/user/:userId/preferences`) se agregaron en la sesión 2026-08-09 junto con la tabla `user_preferences` y el resto de la feature de Preferencias — son las únicas rutas de este archivo con fecha documentada de incorporación explícita; el resto proviene del backend heredado de WeatherApp.
- El contrato completo documentado en CONTEXT.md ("Endpoints del backend (contrato ya definido, no cambia)") coincide en su mayoría con este archivo, salvo dos endpoints presentes en el código pero no mencionados en CONTEXT.md: `GET /api/weather/cities`, `GET /api/weather/stats` y `GET /api/history/:userId` (historial completo, sin límite) — ver el detalle de cada uno en `weatherController.js.md` y `historyController.js.md` respectivamente.
