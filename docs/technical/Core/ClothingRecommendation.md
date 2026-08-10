# movilTravelCompanion.Core.Models.ClothingRecommendation

## Ubicación
`movilTravelCompanion.Core/Models/ClothingRecommendation.cs`

## Propósito
Representa la respuesta del endpoint de recomendación de vestimenta generada por OpenAI en el backend. Existe para deserializar tipadamente el JSON devuelto por `POST /api/ai/clothing-recommendation` y exponerlo al ViewModel del Dashboard sin manipular JSON crudo.

## Tipo
Modelo de datos (POCO), usado como DTO de deserialización de respuesta HTTP.

## Responsabilidades
- Mapear el subconjunto relevante de la respuesta del endpoint de IA (ciudad, estilo de ropa aplicado, sensibilidad al frío aplicada y el texto de la recomendación) a propiedades tipadas de C#.
- Proveer valores por defecto seguros (`ClothingStyle = ""`, `ColdSensitivity = "normal"`, `Recommendation = ""`) para evitar `null` en propiedades no anulables si el JSON llega incompleto.

## Dependencias
Ninguna en tiempo de ejecución. Usa `System.Text.Json.Serialization.JsonPropertyName` para el mapeo de nombres JSON → propiedades C#.

## Miembros públicos clave

### Propiedades / Campos
| Nombre | Tipo | Descripción |
|---|---|---|
| `City` | `string?` | Ciudad para la que se generó la recomendación. Mapea `"city"`. Anulable. |
| `ClothingStyle` | `string` | Estilo de vestimenta aplicado por el backend (`"casual"`, `"formal"` o `"deportivo"`). Mapea `"clothingStyle"`. Default `string.Empty`. |
| `ColdSensitivity` | `string` | Sensibilidad al frío aplicada por el backend. Mapea `"coldSensitivity"`. Default `"normal"`. |
| `Recommendation` | `string` | Texto de la recomendación de vestimenta generado por OpenAI. Mapea `"recommendation"`. Default `string.Empty`. |

### Métodos
Ninguno (POCO sin lógica).

## Flujo y lógica relevante
No contiene lógica: es un contenedor pasivo poblado por `System.Text.Json` al deserializar la respuesta HTTP en `ClothingService.GetRecommendationAsync`. Nótese que el modelo **no** mapea todos los campos que el backend documenta en `CONTEXT.md` para esta respuesta (`success`, `weather`, `timestamp` quedan sin propiedad correspondiente); `System.Text.Json` ignora silenciosamente las propiedades del JSON que no tienen contraparte en la clase, así que esos campos simplemente se descartan al deserializar.

## Relaciones
- **Quién consume esta clase:** `ClothingService.GetRecommendationAsync` (la deserializa desde la respuesta HTTP) y, en el proyecto MAUI, `DashboardViewModel` (la sección "Sugerencia de vestimenta" del Dashboard, según `CONTEXT.md`).
- **A quién usa esta clase:** a nadie; no tiene dependencias propias.

## Notas de diseño
Corresponde al endpoint `POST /api/ai/clothing-recommendation` documentado en `CONTEXT.md`, implementado en el backend antes de esta migración pero conectado desde el cliente MAUI recién en la sesión del 2026-08-09, junto con las preferencias de usuario y el menú Flyout.
