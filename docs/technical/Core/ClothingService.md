# movilTravelCompanion.Core.Services.ClothingService

## Ubicación
- `movilTravelCompanion.Core/Services/ClothingService.cs` (implementación)
- `movilTravelCompanion.Core/Services/IClothingService.cs` (interfaz)

## Propósito
Encapsula la comunicación HTTP con el endpoint de recomendación de vestimenta generada por OpenAI (`POST /api/ai/clothing-recommendation`). Es un servicio nuevo de la sesión 2026-08-09: el endpoint ya existía en el backend desde antes, pero recién en esta sesión se conectó desde el cliente MAUI.

## Tipo
Interfaz de servicio (`IClothingService`) + Implementación de servicio (`ClothingService`), consumidor de HTTP vía `HttpClient`.

## Responsabilidades
- Pedir al backend una recomendación de vestimenta en base al clima actual de una ciudad y las preferencias del usuario (estilo de ropa y sensibilidad al frío).
- Manejar errores HTTP con el mismo patrón que el resto de los servicios de `Core`.
- Lanzar excepción explicativa si la respuesta exitosa viene con cuerpo vacío.

## Dependencias
- `HttpClient`: inyectado por constructor, mismo singleton compartido que el resto de los servicios.
- `movilTravelCompanion.Core.Models.ClothingRecommendation` (tipo de retorno).

## Miembros públicos clave

### Propiedades / Campos
Ninguno público.

### Métodos
| Firma | Descripción |
|---|---|
| `Task<ClothingRecommendation> GetRecommendationAsync(string city, double temperature, int weatherCode, double humidity, double windSpeed, string clothingStyle, string? coldSensitivity)` | `POST /api/ai/clothing-recommendation` con `{ city, temperature, weatherCode, humidity, windSpeed, clothingStyle, coldSensitivity }`. `coldSensitivity` es anulable: `null` se serializa como JSON `null`, que el backend trata igual que "ausente" (equivalente a "sin preferencia"). |

## Flujo y lógica relevante
Método único que arma el cuerpo del POST con un objeto anónimo (nombres de propiedad en `camelCase`, coincidiendo directamente con lo que espera el backend — no hace falta `JsonPropertyName` porque `System.Text.Json` serializa por defecto los nombres del objeto anónimo tal cual están escritos, ya en camelCase). Sigue el patrón estándar: verificar éxito HTTP, propagar el error del backend si falla, deserializar a `ClothingRecommendation` y lanzar excepción si el cuerpo viene vacío pese al éxito HTTP. La particularidad de `coldSensitivity` como `string?` (a diferencia de `clothingStyle`, que es `string` no anulable) refleja una decisión explícita documentada en `CONTEXT.md`: el backend fue corregido en esta misma sesión para tratar `coldSensitivity` explícito como `null` (lo que efectivamente manda `System.Text.Json` al serializar una propiedad ausente/nula) igual que si estuviera completamente ausente del JSON.

## Relaciones
- **Quién consume esta clase:** en el proyecto MAUI, `DashboardViewModel`, sección "Sugerencia de vestimenta" (RadioButton x3 de estilo + botón + resultado), pasando el clima del destino actual (retenido como `WeatherData` completo, no solo strings sueltos) y el estilo/sensibilidad seleccionados o precargados desde `IPreferencesService`.
- **A quién usa esta clase:** `HttpClient` (inyectado), `ClothingRecommendation` (modelo de retorno).

## Notas de diseño
Implementa el contrato de `CONTEXT.md` sección "Recomendación de vestimenta (OpenAI)". Documentado explícitamente en `CONTEXT.md`: "`clothingStyle`: opcional, default `casual`" del lado backend, y "`coldSensitivity`: opcional; ausente o `null` equivalen a 'sin preferencia', el backend trata ambos casos igual" — motivo por el cual esta firma acepta `string? coldSensitivity` mientras que `clothingStyle` se mantiene `string` no anulable (siempre se envía un valor concreto desde el cliente, aun si es el default). También se ajustó en el backend, en la misma sesión, la unificación de los estilos de vestimenta a español (`casual`/`formal`/`deportivo`), reemplazando el `athletic` en inglés que tenía originalmente `openaiController.js` — este servicio ya asume los valores en español.
