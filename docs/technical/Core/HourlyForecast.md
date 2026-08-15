# movilTravelCompanion.Core.Models.HourlyForecast

## Ubicación
`movilTravelCompanion.Core/Models/HourlyForecast.cs`

## Propósito
Representa un punto del pronóstico horario de clima (hora, temperatura y código de clima) para una ciudad, tal como lo devuelve el backend dentro de la respuesta de búsqueda/coordenadas de clima. Existe para tipar el arreglo `hourly_forecast` embebido en `WeatherData`.

## Tipo
Modelo de datos (POCO), usado como DTO de deserialización de respuesta HTTP, anidado dentro de `WeatherData`.

## Responsabilidades
- Mapear cada entrada horaria del pronóstico (hora, temperatura, código de clima WMO) a propiedades tipadas de C#.

## Dependencias
Ninguna en tiempo de ejecución. Usa `System.Text.Json.Serialization.JsonPropertyName`.

## Miembros públicos clave

### Propiedades / Campos
| Nombre | Tipo | Descripción |
|---|---|---|
| `Time` | `string` | Marca de tiempo de la hora del pronóstico, tal como la envía el backend (string, no `DateTime`). Mapea `"time"`. Default `string.Empty`. |
| `Temperature` | `double` | Temperatura prevista para esa hora. Mapea `"temperature"`. |
| `WeatherCode` | `int` | Código de clima WMO previsto para esa hora. Mapea `"weather_code"`. |

### Métodos
Ninguno (POCO sin lógica).

## Flujo y lógica relevante
Sin lógica propia. Se instancia en lote como parte de la lista `WeatherData.HourlyForecast` cuando `System.Text.Json` deserializa la respuesta de `GET /api/weather/search` o `GET /api/weather/coordinates` (los únicos endpoints de clima que, según `CONTEXT.md`, incluyen `hourly_forecast` en la respuesta; `GET /api/weather/random` no lo incluye). Nótese que `Time` queda como `string` sin parsear a `DateTime`/`DateTimeOffset`: cualquier formateo u ordenamiento por fecha queda a cargo de quien consuma este modelo.

## Relaciones
- **Quién consume esta clase:** indirectamente, quien deserializa `WeatherData` (`WeatherApiService`), y en el proyecto MAUI cualquier ViewModel/vista que muestre pronóstico horario.
- **A quién usa esta clase:** a nadie; no tiene dependencias propias.

## Notas de diseño
Ninguna decisión de arquitectura específica documentada en `CONTEXT.md` para este modelo puntual, más allá del contrato general de los endpoints de clima.
