# movilTravelCompanion.Core.Models.UserPreferences

## Ubicación
`movilTravelCompanion.Core/Models/UserPreferences.cs`

## Propósito
Representa las preferencias de vestimenta del usuario (estilo por defecto y sensibilidad al frío) usadas tanto para precargar la pantalla de Preferencias como para alimentar la recomendación de vestimenta del Dashboard. Existe para tipar el cuerpo/respuesta de los endpoints de preferencias del backend.

## Tipo
Modelo de datos (POCO), usado tanto como DTO de deserialización de respuesta HTTP como de cuerpo de request (`PUT`).

## Responsabilidades
- Mapear `defaultClothingStyle` y `coldSensitivity` a propiedades tipadas de C#.
- Proveer valores por defecto (`"casual"` y `"normal"`) que coinciden exactamente con los defaults que documenta el backend para un usuario que nunca guardó preferencias.

## Dependencias
Ninguna en tiempo de ejecución. Usa `System.Text.Json.Serialization.JsonPropertyName`.

## Miembros públicos clave

### Propiedades / Campos
| Nombre | Tipo | Descripción |
|---|---|---|
| `DefaultClothingStyle` | `string` | Estilo de vestimenta preferido por el usuario. Mapea `"defaultClothingStyle"`. Valores válidos según backend: `"casual"`, `"formal"`, `"deportivo"`. Default `"casual"`. |
| `ColdSensitivity` | `string` | Sensibilidad al frío del usuario. Mapea `"coldSensitivity"`. Valores válidos según backend: `"friolento"`, `"normal"`, `"caluroso"`. Default `"normal"`. |

### Métodos
Ninguno (POCO sin lógica).

## Flujo y lógica relevante
Se usa en ambas direcciones del contrato HTTP: como tipo de retorno deserializado en `PreferencesService.GetPreferencesAsync`/`UpdatePreferencesAsync`, y como cuerpo serializado directamente (`PutAsJsonAsync(url, preferences)`) en la actualización. No valida los valores de `DefaultClothingStyle`/`ColdSensitivity` contra la lista permitida — esa validación vive en el backend (`preferencesController.js`, según `CONTEXT.md`); el cliente confía en que la UI (p. ej. un `RadioButtonGroup`) solo ofrezca las opciones válidas.

## Relaciones
- **Quién consume esta clase:** `PreferencesService` (la deserializa/serializa) y, en el proyecto MAUI, `PreferencesViewModel` (lee/guarda preferencias contra el backend real, según `CONTEXT.md`) y `DashboardViewModel` (precarga `SelectedClothingStyle` y la sensibilidad al frío desde Preferencias al cargar el Dashboard, con fallback silencioso a los defaults si la llamada falla).
- **A quién usa esta clase:** a nadie; no tiene dependencias propias.

## Notas de diseño
Según `CONTEXT.md`, si el usuario nunca guardó preferencias, el backend devuelve los defaults (`"casual"`/`"normal"`) **sin crear una fila** en la base — los valores por defecto de las propiedades de esta clase existen justamente para reflejar ese mismo comportamiento del lado cliente si la deserialización no completa algún campo. Esta clase y su servicio asociado son nuevos de la sesión 2026-08-09, junto con `ClothingRecommendation`/`ClothingService` y el menú Flyout.
