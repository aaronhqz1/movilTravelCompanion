# Backend — Índice de documentación técnica

Documentación técnica del backend Node/Express/SQLite (`backend/src/`), consumido por el cliente .NET MAUI vía HTTP a través de los servicios en `movilTravelCompanion.Core/Services/`. Ver también `CONTEXT.md` en la raíz del repo para el contrato REST completo y el historial de decisiones.

| Módulo | Propósito | Documento |
|---|---|---|
| `server.js` | Punto de entrada: crea la app Express, registra middlewares (CORS, JSON), monta el router en `/api` y arranca el servidor HTTP. | [server.js.md](./server.js.md) |
| `config/database.js` | Abre la conexión SQLite y crea las tablas `users`, `weather_history` y `user_preferences` si no existen. | [database.js.md](./database.js.md) |
| `controllers/authController.js` | Registro, login y actualización de ciudad de origen (con hash bcrypt y geocodificación vía Open-Meteo). | [authController.js.md](./authController.js.md) |
| `controllers/historyController.js` | Guarda y lee el historial de consultas climáticas del usuario, con regla de no-duplicado en 24hs y límite de 3 recientes. | [historyController.js.md](./historyController.js.md) |
| `controllers/openaiController.js` | Genera recomendación de vestimenta con la API de OpenAI a partir del clima, estilo y sensibilidad al frío. | [openaiController.js.md](./openaiController.js.md) |
| `controllers/preferencesController.js` | Lee y guarda las preferencias de usuario (estilo de vestimenta por defecto, sensibilidad al frío) en la tabla `user_preferences`. | [preferencesController.js.md](./preferencesController.js.md) |
| `controllers/weatherController.js` | Consulta clima actual y pronóstico horario en Open-Meteo (por ciudad o coordenadas), y expone una base local de 73 ciudades. | [weatherController.js.md](./weatherController.js.md) |
| `routes/index.js` | Mapeo declarativo de las 14 rutas del contrato REST hacia las funciones de los controllers. | [routes-index.js.md](./routes-index.js.md) |
