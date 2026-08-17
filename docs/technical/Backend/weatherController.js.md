# backend/src/controllers/weatherController.js

## Propósito
Provee acceso al clima actual y pronóstico horario vía la API pública de Open-Meteo, tanto por coordenadas directas como por nombre de ciudad (geocodificado), además de exponer una base de datos local de ciudades del mundo (para clima aleatorio sin login, listado y estadísticas por continente).

## Tipo
Controller de Express.

## Dependencias
- `node:https` (built-in de Node) — cliente HTTP nativo para llamar a `api.open-meteo.com` (clima) y `geocoding-api.open-meteo.com` (geocodificación).
- No usa `../config/database` (no toca SQLite); las "ciudades" son un array constante en memoria (`worldCities`), no una tabla de base de datos.

## Endpoints expuestos (si aplica)
| Método | Ruta | Body/Query | Response | Descripción |
|---|---|---|---|---|
| GET | `/api/weather/random` | — | `200 { city, latitude, longitude, temperature, humidity, wind_speed, weather_code, hourly_forecast }` | Elige una ciudad al azar de `worldCities` y devuelve su clima actual + pronóstico horario. |
| GET | `/api/weather/search` | Query: `city` (string) | `200 { city, latitude, longitude, temperature, humidity, wind_speed, weather_code, hourly_forecast }` / `404 { error }` / `400 { error }` | Geocodifica el nombre de ciudad recibido y devuelve su clima. |
| GET | `/api/weather/coordinates` | Query: `lat`, `lon` | `200 { temperature, humidity, wind_speed, weather_code, hourly_forecast }` / `400 { error }` | Devuelve clima directamente por coordenadas, sin geocodificar. |
| GET | `/api/weather/cities` | — | `200 { total, cities: [{ name, country, displayName, latitude, longitude }] }` | Lista completa de `worldCities`, ordenada alfabéticamente por nombre. |
| GET | `/api/weather/stats` | — | `200 { totalCities, byContinent: { América, Europa, Asia, África, Oceanía } }` | Cuenta de ciudades por continente, clasificadas heurísticamente por longitud/latitud. |

## Funciones exportadas
| Nombre | Firma/parámetros | Descripción |
|---|---|---|
| `getRandomWeather` | `(req, res) => Promise<void>` | Selecciona ciudad aleatoria de `worldCities` y devuelve su clima vía `getWeatherByCoordinates`. |
| `searchWeather` | `(req, res) => Promise<void>`, lee `req.query.city` | Geocodifica con `geocodeCity` y luego llama `getWeatherByCoordinates`. |
| `getWeatherByCoords` | `(req, res) => Promise<void>`, lee `req.query.{lat, lon}` | Llama `getWeatherByCoordinates` directamente con `parseFloat(lat)`/`parseFloat(lon)`. |
| `getCitiesList` | `(req, res) => void` | Devuelve `worldCities` ordenada alfabéticamente, con forma normalizada para el cliente. |
| `getCitiesStats` | `(req, res) => void` | Clasifica cada ciudad de `worldCities` por continente usando reglas de longitud/latitud, y devuelve el conteo. |

Funciones internas no exportadas: `geocodeCity(cityName)` (Promise que resuelve `{city: "Nombre, País", latitude, longitude}`), `getWeatherByCoordinates(latitude, longitude)` (Promise que resuelve el clima actual + primeras 24 horas de pronóstico horario desde Open-Meteo).

## Esquema de datos (si aplica, ej. database.js)
No aplica (no toca base de datos). `worldCities` es un array constante en memoria con **73 ciudades** (`{name, country, lat, lon}`), agrupadas por comentarios en 5 continentes: América (18), Europa (22), Asia (19), África (8), Oceanía (6).

## Lógica y validaciones relevantes
- **Clima aleatorio sin login**: `getRandomWeather` no requiere autenticación; usado en `HomePage` del cliente MAUI para mostrar clima antes de loguearse, consistente con la regla de negocio de CONTEXT.md ("Usuario no autenticado ve clima de una ciudad aleatoria").
- **`getWeatherByCoordinates`** arma una sola llamada a Open-Meteo pidiendo `current` (temperatura, humedad relativa, viento, código de clima) y `hourly` (temperatura y código por hora), con `timezone=auto` y `forecast_days=1`; el pronóstico horario se recorta a las primeras 24 entradas (`.slice(0, 24)`).
- **`searchWeather`** valida que `city` venga en el query string (`400` si falta); si `geocodeCity` no encuentra resultados, propaga `Error('Ciudad no encontrada')` → `404`; cualquier otro error de red/parseo → `500`.
- **`getWeatherByCoords`** valida que `lat` y `lon` estén presentes (`400` si falta alguno), pero no valida que sean números válidos más allá de `parseFloat` (un valor no numérico produciría `NaN`, que Open-Meteo probablemente rechazaría, resultando en un `500` genérico).
- **Clasificación de continente en `getCitiesStats`** es heurística por rangos de longitud/latitud (no usa el campo `country` real), por lo que puede mal-clasificar casos límite (el comentario del propio código lo describe como "aproximada").
- `geocodeCity` en este archivo devuelve `city` como `"Nombre, País"` (concatenado), mientras que la función homónima en `authController.js` devuelve `city` como `location.name` solo (sin país) — son dos implementaciones independientes y ligeramente distintas, no una función compartida.

## Relaciones
- No depende de `../config/database`.
- Sus cinco funciones se importan y montan en `backend/src/routes/index.js`.
- Consumido desde el cliente MAUI por `movilTravelCompanion.Core/Services/WeatherApiService.cs` (vía `IWeatherApiService`), usado por `HomeViewModel` (clima aleatorio + búsqueda), `TravelDestinationViewModel` (búsqueda de ciudad destino) y `DashboardViewModel` (clima del destino actual + búsqueda de otra ciudad).

## Notas de diseño
- CONTEXT.md describe la lista de ciudades como "una lista de 50 ciudades predefinidas en el backend" (sección "Reglas de negocio a replicar en el cliente"), pero el array `worldCities` real en el código contiene **73 ciudades**, no 50 — discrepancia entre el número documentado en CONTEXT.md y el código real. El comportamiento (selección aleatoria de una lista fija) sí coincide, solo el conteo documentado está desactualizado.
- El contrato de endpoints de CONTEXT.md solo documenta `GET /api/weather/random`, `GET /api/weather/search` y `GET /api/weather/coordinates`; los endpoints `GET /api/weather/cities` y `GET /api/weather/stats` existen en el código y están registrados en las rutas, pero no aparecen en la sección "Endpoints del backend" de CONTEXT.md — posiblemente sin consumidor actual en `IWeatherApiService`.
- Los códigos de clima devueltos (`weather_code`) siguen el estándar WMO documentado como regla de negocio en CONTEXT.md, y son interpretados en texto por `getWeatherDescription` en `openaiController.js` (no en este archivo).
