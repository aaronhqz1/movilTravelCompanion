# movilTravelCompanion.Core.Models.HistoryEntry

## Ubicación
`movilTravelCompanion.Core/Models/HistoryEntry.cs`

## Propósito
Representa una fila del historial de consultas de clima de un usuario, tal como la devuelve el backend (`weather_history`). Existe para deserializar tipadamente las respuestas de los endpoints de historial y mostrarlas en la UI (por ejemplo, en el `CollectionView` de últimas búsquedas del Dashboard).

## Tipo
Modelo de datos (POCO), usado como DTO de deserialización de respuesta HTTP.

## Responsabilidades
- Mapear cada campo de una entrada de historial devuelta por el backend (id, usuario, ciudad, coordenadas, datos de clima, momento de la consulta) a propiedades tipadas de C#.
- Servir como elemento de lista (`List<HistoryEntry>`) para la colección de últimas búsquedas.

## Dependencias
Ninguna en tiempo de ejecución. Usa `System.Text.Json.Serialization.JsonPropertyName` para el mapeo de nombres JSON → propiedades C# (varios campos del backend usan `snake_case`, p. ej. `wind_speed`, `weather_code`).

## Miembros públicos clave

### Propiedades / Campos
| Nombre | Tipo | Descripción |
|---|---|---|
| `Id` | `int` | Identificador de la fila en `weather_history`. Mapea `"id"`. |
| `UserId` | `int` | Id del usuario dueño de la entrada. Mapea `"userId"`. |
| `City` | `string` | Nombre de la ciudad consultada. Mapea `"city"`. Default `string.Empty`. |
| `Latitude` | `double` | Latitud de la ciudad. Mapea `"latitude"`. |
| `Longitude` | `double` | Longitud de la ciudad. Mapea `"longitude"`. |
| `Temperature` | `double` | Temperatura registrada en el momento de la consulta. Mapea `"temperature"`. |
| `Humidity` | `double` | Humedad registrada. Mapea `"humidity"`. |
| `WindSpeed` | `double` | Velocidad del viento registrada. Mapea `"wind_speed"`. |
| `WeatherCode` | `int` | Código de clima WMO registrado. Mapea `"weather_code"`. |
| `QueryTime` | `DateTime` | Momento en que se guardó la consulta. Mapea `"queryTime"`. |

### Métodos
Ninguno (POCO sin lógica).

## Flujo y lógica relevante
No contiene lógica propia: es poblado por `System.Text.Json` al deserializar la respuesta de `GET /api/history/:userId/recent` en `HistoryService.GetRecentAsync`. Las convenciones de nombre mixtas (`camelCase` para `userId`/`queryTime`, `snake_case` para `wind_speed`/`weather_code`) reflejan tal cual el contrato JSON del backend documentado en `CONTEXT.md`, y cada `JsonPropertyName` existe justamente para tender ese puente sin renombrar los campos del lado del servidor.

## Relaciones
- **Quién consume esta clase:** `HistoryService.GetRecentAsync` (la deserializa desde la respuesta HTTP) y, en el proyecto MAUI, `DashboardViewModel`, que según `CONTEXT.md` muestra las últimas 3 búsquedas en un `CollectionView`.
- **A quién usa esta clase:** a nadie; no tiene dependencias propias.

## Notas de diseño
Implementa la regla de negocio de `CONTEXT.md` "Solo se muestran las últimas 3 búsquedas del historial en el dashboard" — el modelo en sí no aplica ese límite (lo aplica el backend en `GET /api/history/:userId/recent`, que ya devuelve como máximo 3 filas), pero es el tipo que transporta esos datos hasta la UI.
