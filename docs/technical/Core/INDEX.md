# Índice técnico — movilTravelCompanion.Core

Documentación técnica de cada clase/interfaz de la librería `movilTravelCompanion.Core` (lógica de negocio y servicios HTTP que consumen el backend Express). Ver también `CONTEXT.md` en la raíz del repo para el contrato completo de la API REST y las reglas de negocio.

| Clase / Interfaz | Propósito | Documento |
|---|---|---|
| `ApiConfig` | Centraliza la URL base del backend en una única constante configurable según entorno (emulador/dispositivo físico). | [ApiConfig.md](./ApiConfig.md) |
| `ClothingRecommendation` | Modelo de la respuesta del endpoint de recomendación de vestimenta generada por OpenAI. | [ClothingRecommendation.md](./ClothingRecommendation.md) |
| `HistoryEntry` | Modelo de una entrada del historial de consultas de clima de un usuario. | [HistoryEntry.md](./HistoryEntry.md) |
| `HourlyForecast` | Modelo de un punto del pronóstico horario de clima, embebido en `WeatherData`. | [HourlyForecast.md](./HourlyForecast.md) |
| `User` | Modelo del usuario autenticado: identidad, ciudad de origen y destino de viaje actual; objeto central de la sesión. | [User.md](./User.md) |
| `UserPreferences` | Modelo de las preferencias de vestimenta del usuario (estilo por defecto y sensibilidad al frío). | [UserPreferences.md](./UserPreferences.md) |
| `WeatherData` | Modelo del clima de una ciudad (actual + pronóstico horario opcional); usado por todos los flujos de clima. | [WeatherData.md](./WeatherData.md) |
| `AuthService` (+ `IAuthService`) | Servicio HTTP de registro y login de usuarios. | [AuthService.md](./AuthService.md) |
| `WeatherApiService` (+ `IWeatherApiService`) | Servicio HTTP de clima: ciudad aleatoria, búsqueda por nombre y consulta por coordenadas. | [WeatherApiService.md](./WeatherApiService.md) |
| `HistoryService` (+ `IHistoryService`) | Servicio HTTP para guardar y recuperar el historial de consultas de clima de un usuario. | [HistoryService.md](./HistoryService.md) |
| `PreferencesService` (+ `IPreferencesService`) | Servicio HTTP para leer y guardar las preferencias de vestimenta del usuario. | [PreferencesService.md](./PreferencesService.md) |
| `ClothingService` (+ `IClothingService`) | Servicio HTTP para pedir una recomendación de vestimenta generada por OpenAI. | [ClothingService.md](./ClothingService.md) |
| `ISessionStore` | Interfaz de abstracción sobre el almacenamiento de la sesión del usuario (implementada en el proyecto MAUI). | [ISessionStore.md](./ISessionStore.md) |
