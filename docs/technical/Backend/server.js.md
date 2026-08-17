# backend/src/server.js

## Propósito
Punto de entrada de la aplicación backend: crea la instancia de Express, registra los middlewares globales (CORS, parseo de JSON), monta el router principal bajo el prefijo `/api`, define una ruta raíz de diagnóstico y arranca el servidor HTTP en el puerto configurado.

## Tipo
Punto de entrada del servidor.

## Dependencias
- `express` (^4.18.2) — framework HTTP.
- `cors` (^2.8.5) — middleware para habilitar CORS (necesario porque el cliente original era React/Vite en otro origen; el cliente MAUI también hace requests HTTP externas al proceso Node).
- `dotenv` (^16.3.1) — carga variables de entorno desde `.env` (usado para `PORT` y `OPENAI_API_KEY`).
- Módulo interno: `./routes` (`backend/src/routes/index.js`), montado en `/api`.

## Endpoints expuestos (si aplica)
| Método | Ruta | Body/Query | Response | Descripción |
|---|---|---|---|---|
| GET | `/` | — | `{ message, version, api, features: { weather, openai } }` | Ruta de diagnóstico/health-check. Indica si `OPENAI_API_KEY` está configurada mediante `features.openai`. No forma parte del contrato `/api/*` consumido por el cliente MAUI. |
| ALL | `/api/*` | — | (delegado) | Monta el router de `./routes` bajo el prefijo `/api`. Todos los endpoints reales del contrato viven ahí. |

## Funciones exportadas
Ninguna: este módulo no exporta nada (`module.exports` no se usa). Es un script ejecutable (`node src/server.js`, definido como `main` y en los scripts `start`/`dev` de `package.json`).

## Esquema de datos (si aplica, ej. database.js)
No aplica.

## Lógica y validaciones relevantes
- Middleware de manejo de errores genérico al final de la cadena (`app.use((err, req, res, next) => ...)`): captura cualquier error no manejado en un handler síncrono/middleware anterior, lo loguea con `console.error(err.stack)` y responde `500 { error: 'Algo salió mal en el servidor' }`. Nota: como los controllers de este backend son en su mayoría asíncronos con sus propios `try/catch` (o basados en callbacks de `sqlite3`), este handler global rara vez se activa en la práctica — es una red de seguridad, no el mecanismo primario de manejo de errores.
- El puerto se toma de `process.env.PORT`, con fallback a `3000` si no está definido. CONTEXT.md documenta que el cliente MAUI apunta a `http://10.0.2.2:3000` (emulador) o `http://<IP-LAN>:3000` (dispositivo físico), consistente con este default.
- El log de arranque expone si OpenAI está configurado (`OPENAI_API_KEY`), útil para diagnosticar en desarrollo si el endpoint de recomendación de vestimenta va a funcionar.

## Relaciones
- Importa y monta `backend/src/routes/index.js`.
- Es el único módulo que hace `app.listen(...)`; todos los demás módulos del backend (controllers, config) son cargados indirectamente a través de `routes/index.js`.
- No es consumido directamente por ningún servicio del cliente MAUI (es infraestructura de arranque, no un endpoint del contrato), aunque indirectamente todo el tráfico de `movilTravelCompanion.Core/Services/` (`AuthService`, `WeatherApiService`, `HistoryService`, `PreferencesService`, `ClothingService`) pasa por el servidor que este archivo levanta.

## Notas de diseño
- CONTEXT.md aclara que este backend fue copiado físicamente a `backend/` dentro de este repo (antes vivía en el repo separado `WeatherApp`), y que a diferencia de la decisión original **sí se permite editarlo** si hace falta durante la migración a MAUI. `server.js` no fue modificado en la sesión del 2026-08-09 (los cambios de esa sesión fueron en `config/database.js`, `controllers/preferencesController.js`, `controllers/openaiController.js` y `routes/index.js`).
- `node_modules/` del backend está en `.gitignore`; hace falta `npm install` en `backend/` tras clonar el repo.
