# backend/src/controllers/authController.js

## Propósito
Implementa el registro, inicio de sesión y actualización de ciudad de origen de un usuario: hashea/verifica contraseñas con bcrypt, valida fortaleza de contraseña en el registro, y geocodifica el nombre de ciudad de origen contra la API pública de Open-Meteo al actualizarla.

## Tipo
Controller de Express.

## Dependencias
- `bcrypt` (^5.1.1) — hash y verificación de contraseñas (factor de costo 12, explícitamente mayor al default de 10).
- `node:https` (built-in de Node) — cliente HTTP nativo para llamar a la Geocoding API de Open-Meteo (no se usa una librería como `axios`).
- Módulo interno: `../config/database` (`db`).

## Endpoints expuestos (si aplica)
Este archivo no define rutas directamente (eso lo hace `routes/index.js`), pero sus funciones se montan en:

| Método | Ruta | Body/Query | Response | Descripción |
|---|---|---|---|---|
| POST | `/api/auth/register` | `{ username, password }` | `201 { message, userId, username }` / `400 { error }` | Crea un usuario nuevo con contraseña hasheada. |
| POST | `/api/auth/login` | `{ username, password }` | `200 { message, userId, username, homeCity, homeLatitude, homeLongitude }` / `401 { error }` | Verifica credenciales y devuelve datos de sesión, incluida la ciudad de origen si ya fue configurada. |
| PUT | `/api/user/:userId/home` | `{ homeCity }` (nombre de ciudad en texto libre) | `200 { message, homeCity, homeLatitude, homeLongitude }` / `404 { error }` | Geocodifica `homeCity` contra Open-Meteo y actualiza `home_city`/`home_latitude`/`home_longitude` del usuario. |

## Funciones exportadas
| Nombre | Firma/parámetros | Descripción |
|---|---|---|
| `register` | `(req, res) => Promise<void>`, lee `req.body.{username, password}` | Valida presencia y longitud mínima de `username` (≥3), valida fortaleza de `password`, hashea con bcrypt (costo 12) e inserta en `users`. Maneja colisión de `UNIQUE constraint failed` como 400 "El usuario ya existe". |
| `login` | `(req, res) => void`, lee `req.body.{username, password}` | Busca el usuario por `username`, compara la contraseña con `bcrypt.compare`, y si es válida devuelve los datos de sesión incluyendo ciudad de origen. |
| `updateHomeCity` | `(req, res) => Promise<void>`, lee `req.params.userId` y `req.body.homeCity` | Geocodifica el nombre de ciudad recibido vía `geocodeCity()` y actualiza las tres columnas `home_*` del usuario indicado. |

Funciones internas no exportadas: `validatePasswordStrength(password)` (retorna `{valid, message?}`), `geocodeCity(cityName)` (Promise que resuelve `{city, latitude, longitude, country}` consultando `geocoding-api.open-meteo.com`).

## Esquema de datos (si aplica, ej. database.js)
No define esquema; opera sobre la tabla `users` definida en `config/database.js` (ver `database.js.md`).

## Lógica y validaciones relevantes
- **Validación de fortaleza de contraseña** (`validatePasswordStrength`): mínimo 8 caracteres, al menos una mayúscula, una minúscula, un número y un carácter especial (`!@#$%^&*(),.?":{}|<>`). Se aplica solo en `register`, no en `login`.
- **Username mínimo 3 caracteres**, validado solo en `register`.
- **Hash de contraseña con factor de costo 12** (explícitamente más alto que el default 10 de bcrypt), comentado en el código como decisión de mayor seguridad.
- **Manejo de duplicados**: la restricción `UNIQUE` de SQLite sobre `username` se traduce a un 400 "El usuario ya existe" inspeccionando el mensaje de error (`err.message.includes('UNIQUE constraint failed')`), no un código de error estructurado.
- **Login no revela cuál dato es incorrecto**: tanto usuario inexistente como contraseña incorrecta devuelven el mismo mensaje genérico `401 "Usuario o contraseña incorrectos"`.
- **Geocodificación de ciudad de origen**: `updateHomeCity` no guarda el string tal cual lo mandó el cliente, sino el nombre normalizado que devuelve Open-Meteo (`location.name`), junto con sus coordenadas. Si Open-Meteo no encuentra resultados, `geocodeCity` rechaza con `Error('Ciudad no encontrada')`, que el controller traduce a `404`.
- `register` **no** acepta ni persiste `homeCity` en el body (ver "Notas de diseño" — discrepancia con CONTEXT.md).

## Relaciones
- Usa `../config/database` para todas las operaciones sobre la tabla `users`.
- Sus tres funciones se importan y montan en `backend/src/routes/index.js`.
- Consumido desde el cliente MAUI por `movilTravelCompanion.Core/Services/AuthService.cs` (vía `IAuthService`), que a su vez es usado por `LoginViewModel`, `RegisterViewModel` y (para `updateHomeCity`, si está implementado) la configuración de ciudad de origen.

## Notas de diseño
- CONTEXT.md documenta el contrato de `POST /api/auth/register` como `Body: { username, password, homeCity }`, pero el código real de `register` solo desestructura y usa `{ username, password }` — `homeCity` no se lee ni se persiste en el registro. La ciudad de origen solo puede configurarse después, vía `PUT /api/user/:userId/home` (`updateHomeCity`), consistente con la regla de negocio de CONTEXT.md: "La ciudad de origen es opcional al registrarse; puede configurarse después desde 'Configuración'." Esto es una discrepancia menor entre el contrato documentado y el código real: el campo `homeCity` en el body de registro, si el cliente lo envía, es simplemente ignorado.
- El uso de `node:https` en vez de una librería HTTP (como `axios`, usada en el frontend React original) es consistente en todo el backend (ver también `weatherController.js`), reflejando que el backend evita dependencias externas más allá de las estrictamente necesarias.
