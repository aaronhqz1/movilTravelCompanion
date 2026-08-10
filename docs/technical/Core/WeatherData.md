# movilTravelCompanion.Core.Models.WeatherData

## Ubicación
`movilTravelCompanion.Core/Models/WeatherData.cs`

## Propósito
Representa la información de clima de una ciudad devuelta por el backend (clima actual, coordenadas y, opcionalmente, pronóstico horario). Es el modelo central de todos los flujos de clima de la app: ciudad aleatoria (Home), búsqueda por nombre (Home/TravelDestination/Dashboard) y consulta por coordenadas.

## Tipo
Modelo de datos (POCO), usado como DTO de deserialización de respuesta HTTP.

## Responsabilidades
- Mapear la respuesta de los tres endpoints de clima (`/api/weather/random`, `/api/weather/search`, `/api/weather/coordinates`) a un único tipo tipado en C#.
- Contener la lista de pronóstico horario (`HourlyForecast`) cuando el endpoint la incluye.

## Dependencias
- `HourlyForecast` (composición: `List<HourlyForecast>`).
- Usa `System.Text.Json.Serialization.JsonPropertyName` para el mapeo de nombres JSON → propiedades C#.

## Miembros públicos clave

### Propiedades / Campos
| Nombre | Tipo | Descripción |
|---|---|---|
| `City` | `string?` | Nombre de la ciudad. Mapea `"city"`. Anulable (la respuesta de `/api/weather/coordinates` no incluye `city`, según el contrato de `CONTEXT.md`). |
| `Latitude` | `double` | Latitud de la ciudad. Mapea `"latitude"`. |
| `Longitude` | `double` | Longitud de la ciudad. Mapea `"longitude"`. |
| `Temperature` | `double` | Temperatura actual. Mapea `"temperature"`. |
| `Humidity` | `double` | Humedad actual. Mapea `"humidity"`. |
| `WindSpeed` | `double` | Velocidad del viento actual. Mapea `"wind_speed"`. |
| `WeatherCode` | `int` | Código de clima WMO actual. Mapea `"weather_code"`. |
| `HourlyForecast` | `List<HourlyForecast>` | Pronóstico horario, presente solo en las respuestas de `/api/weather/search` y `/api/weather/coordinates` (no en `/api/weather/random`). Mapea `"hourly_forecast"`. Inicializado como lista vacía (`[]`) por defecto. |

### Métodos
Ninguno (POCO sin lógica).

## Flujo y lógica relevante
Se instancia por `System.Text.Json` en los tres métodos de `WeatherApiService` (`GetRandomWeatherAsync`, `SearchWeatherAsync`, `GetWeatherByCoordinatesAsync`), todos pasando por el helper privado `ReadWeatherDataAsync`. Como `/api/weather/random` no devuelve `hourly_forecast` (según `CONTEXT.md`), el default `= []` de la propiedad evita que quede en `null` y fuerza a los consumidores a poder iterar la lista sin chequeo de nulidad extra, aun cuando esté vacía. `City` queda `null` específicamente en la respuesta de `/api/weather/coordinates`, que no lo incluye — cualquier UI que muestre esta propiedad debe tolerar ese caso (por ejemplo, mostrando el nombre de ciudad que ya tenía guardado de otra fuente).

## Relaciones
- **Quién consume esta clase:** `WeatherApiService` (la produce en los tres métodos de clima); en el proyecto MAUI, `HomeViewModel`, `TravelDestinationViewModel` y `DashboardViewModel` (clima del destino, búsqueda de otra ciudad); `HistoryService.AddAsync` la recibe como parámetro para extraer los campos que arma en el cuerpo de `POST /api/history`; `ClothingService`/`DashboardViewModel` toman `Temperature`, `WeatherCode`, `Humidity`, `WindSpeed` de una instancia de este tipo para pedir la recomendación de vestimenta.
- **A quién usa esta clase:** `HourlyForecast` (composición, lista embebida).

## Notas de diseño
`CONTEXT.md` señala un bug corregido durante la sesión 2026-08-09 relacionado a este modelo: `DashboardViewModel.ApplyDestinationWeather` originalmente extraía solo temperatura/humedad/viento a strings sueltos, perdiendo el `WeatherCode` necesario para el endpoint de recomendación de vestimenta; se corrigió reteniendo el objeto `WeatherData` completo en el ViewModel en vez de descomponerlo prematuramente.
