# movilTravelCompanion.Core.Services.WeatherApiService

## Ubicación
- `movilTravelCompanion.Core/Services/WeatherApiService.cs` (implementación)
- `movilTravelCompanion.Core/Services/IWeatherApiService.cs` (interfaz)

## Propósito
Encapsula toda la comunicación HTTP con los endpoints de clima del backend (clima aleatorio, búsqueda por ciudad y consulta por coordenadas), que a su vez el backend resuelve contra las APIs externas de Open-Meteo. Es el único punto del cliente que sabe cómo pedir datos de clima.

## Tipo
Interfaz de servicio (`IWeatherApiService`) + Implementación de servicio (`WeatherApiService`), consumidor de HTTP vía `HttpClient`.

## Responsabilidades
- Obtener el clima de una ciudad aleatoria (`GetRandomWeatherAsync`) para el usuario no autenticado en `HomePage`.
- Buscar el clima de una ciudad por nombre (`SearchWeatherAsync`), usada en Home, TravelDestination y Dashboard.
- Obtener el clima por coordenadas (`GetWeatherByCoordinatesAsync`).
- Escapar correctamente el nombre de ciudad en la query string (`Uri.EscapeDataString`) y formatear latitud/longitud con `CultureInfo.InvariantCulture` para evitar que el separador decimal local (p. ej. coma en vez de punto) rompa la URL.
- Centralizar el manejo de errores HTTP y de cuerpo vacío en un único método privado reutilizado por los tres métodos públicos.

## Dependencias
- `HttpClient`: inyectado por constructor, mismo singleton compartido que el resto de los servicios (`ApiConfig.BaseUrl` como `BaseAddress`).
- `movilTravelCompanion.Core.Models.WeatherData` (tipo de retorno de los tres métodos).
- `System.Globalization.CultureInfo` (para formatear coordenadas de forma invariante).

## Miembros públicos clave

### Propiedades / Campos
Ninguno público.

### Métodos
| Firma | Descripción |
|---|---|
| `Task<WeatherData> GetRandomWeatherAsync()` | `GET /api/weather/random`. Clima de una ciudad aleatoria (de las 50 predefinidas en el backend), sin `hourly_forecast`. |
| `Task<WeatherData> SearchWeatherAsync(string city)` | `GET /api/weather/search?city={cityName}`, con `city` escapado vía `Uri.EscapeDataString`. Incluye `hourly_forecast`. |
| `Task<WeatherData> GetWeatherByCoordinatesAsync(double latitude, double longitude)` | `GET /api/weather/coordinates?lat={lat}&lon={lon}`, con `lat`/`lon` formateados con `CultureInfo.InvariantCulture`. Incluye `hourly_forecast`. |

## Flujo y lógica relevante
Los tres métodos públicos son wrappers delgados que arman la URL y delegan en el helper privado estático `ReadWeatherDataAsync(HttpResponseMessage, string operationDescription)`, que centraliza el patrón repetido en los demás servicios de `Core`: si `!response.IsSuccessStatusCode`, lee el cuerpo como texto y lanza `HttpRequestException` con código HTTP + cuerpo + descripción de la operación que falló (p. ej. "obtener el clima aleatorio", "buscar el clima de 'Madrid'"); si la deserialización a `WeatherData` da `null` pese al éxito HTTP, también lanza excepción explicativa. El uso de `CultureInfo.InvariantCulture` en `GetWeatherByCoordinatesAsync` es intencional: sin él, en un dispositivo configurado con configuración regional que usa coma decimal, `latitude.ToString()` produciría algo como `"40,4168"` en vez de `"40.4168"`, rompiendo la query string que el backend espera con punto decimal.

## Relaciones
- **Quién consume esta clase:** en el proyecto MAUI, `HomeViewModel` (clima aleatorio + búsqueda), `TravelDestinationViewModel` (búsqueda de ciudad de destino) y `DashboardViewModel` (clima del destino al entrar + búsqueda de otra ciudad que se guarda en historial).
- **A quién usa esta clase:** `HttpClient` (inyectado), `WeatherData` (modelo de retorno, que a su vez contiene `HourlyForecast`).

## Notas de diseño
Implementa el contrato de `CONTEXT.md` sección "Endpoints del backend — Clima" y la regla de negocio "usuario no autenticado ve clima de una ciudad aleatoria (de una lista de 50 ciudades predefinidas en el backend)" — esa lista de 50 ciudades vive del lado del servidor, el cliente solo pide `/api/weather/random` sin conocer el conjunto. Los códigos de clima (`weather_code`) que devuelve este servicio siguen el estándar WMO documentado en `CONTEXT.md`, cuya interpretación (íconos, texto descriptivo) queda fuera de este servicio y corresponde a la capa de UI/ViewModel.
