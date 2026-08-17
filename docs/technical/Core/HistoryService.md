# movilTravelCompanion.Core.Services.HistoryService

## Ubicación
- `movilTravelCompanion.Core/Services/HistoryService.cs` (implementación)
- `movilTravelCompanion.Core/Services/IHistoryService.cs` (interfaz)

## Propósito
Encapsula la comunicación HTTP con los endpoints de historial de consultas de clima (`/api/history`), permitiendo guardar una nueva consulta y recuperar las últimas búsquedas de un usuario.

## Tipo
Interfaz de servicio (`IHistoryService`) + Implementación de servicio (`HistoryService`), consumidor de HTTP vía `HttpClient`.

## Responsabilidades
- Guardar una consulta de clima en el historial del usuario (`AddAsync`), traduciendo un `WeatherData` (más el `userId`) al cuerpo JSON que espera el backend.
- Obtener las últimas búsquedas de un usuario (`GetRecentAsync`), deserializadas como lista de `HistoryEntry`.
- Propagar tal cual, en el mensaje de la excepción, el error que devuelve el backend cuando se viola la regla de "no repetir la misma ciudad dentro de 24 horas", en vez de reimplementar esa validación en el cliente.

## Dependencias
- `HttpClient`: inyectado por constructor, mismo singleton compartido que el resto de los servicios.
- `movilTravelCompanion.Core.Models.WeatherData` (parámetro de entrada de `AddAsync`).
- `movilTravelCompanion.Core.Models.HistoryEntry` (tipo de retorno de `GetRecentAsync`).

## Miembros públicos clave

### Propiedades / Campos
Ninguno público.

### Métodos
| Firma | Descripción |
|---|---|
| `Task AddAsync(int userId, WeatherData weather)` | `POST /api/history` con `{ userId, city, latitude, longitude, temperature, humidity, wind_speed, weather_code }`, extraídos de `weather`. No devuelve el `id` generado (aunque el backend lo incluye en la respuesta, según `CONTEXT.md`); el método retorna `Task` sin valor. |
| `Task<List<HistoryEntry>> GetRecentAsync(int userId)` | `GET /api/history/{userId}/recent`. Devuelve hasta las últimas 3 entradas (límite aplicado por el backend). Si el cuerpo deserializa a `null`, devuelve lista vacía en vez de `null`. |

## Flujo y lógica relevante
`AddAsync` arma el cuerpo del POST con un objeto anónimo que mapea explícitamente los nombres `snake_case` que espera el backend (`wind_speed`, `weather_code`) a partir de las propiedades C# en `PascalCase` de `WeatherData`. Si la respuesta no es exitosa, lee el cuerpo de error y lo envuelve en `HttpRequestException` — el comentario en el código aclara explícitamente que la regla de "misma ciudad dentro de 24hs" la valida el backend y el cliente solo repropaga el mensaje de error tal cual, sin duplicar la regla. `GetRecentAsync` sigue el mismo patrón de manejo de error que los demás servicios, y usa el operador `??` con `[]` para nunca devolver `null` al llamador aunque el body deserialice a `null`.

## Relaciones
- **Quién consume esta clase:** en el proyecto MAUI, `DashboardViewModel` — llama `AddAsync` al buscar una ciudad nueva desde el Dashboard (que se guarda en el historial) y `GetRecentAsync` para poblar el `CollectionView` de últimas 3 búsquedas.
- **A quién usa esta clase:** `HttpClient` (inyectado), `WeatherData` (entrada), `HistoryEntry` (salida).

## Notas de diseño
Refleja dos reglas de negocio de `CONTEXT.md`: "no se puede guardar la misma ciudad dos veces en el historial dentro de 24 horas (validar en backend, pero reflejar el estado en la UI)" y "solo se muestran las últimas 3 búsquedas del historial en el dashboard". Ambas reglas se aplican del lado del servidor; este servicio solo transporta la solicitud/respuesta y deja que la UI muestre el mensaje de error o la lista ya recortada tal como llega.
