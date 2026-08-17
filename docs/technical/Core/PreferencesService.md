# movilTravelCompanion.Core.Services.PreferencesService

## Ubicación
- `movilTravelCompanion.Core/Services/PreferencesService.cs` (implementación)
- `movilTravelCompanion.Core/Services/IPreferencesService.cs` (interfaz)

## Propósito
Encapsula la comunicación HTTP con los endpoints de preferencias de usuario (`/api/user/:userId/preferences`), permitiendo leer y guardar el estilo de vestimenta por defecto y la sensibilidad al frío. Es un servicio nuevo de la sesión 2026-08-09, agregado junto con la pantalla de Preferencias y la recomendación de vestimenta.

## Tipo
Interfaz de servicio (`IPreferencesService`) + Implementación de servicio (`PreferencesService`), consumidor de HTTP vía `HttpClient`.

## Responsabilidades
- Obtener las preferencias actuales de un usuario (`GetPreferencesAsync`).
- Guardar/actualizar las preferencias de un usuario (`UpdatePreferencesAsync`).
- Manejar errores HTTP con el mismo patrón que el resto de los servicios de `Core` (mensaje descriptivo con código HTTP y cuerpo del backend).
- Devolver un `UserPreferences` con los valores por defecto (`"casual"`/`"normal"`) si el cuerpo de la respuesta llega vacío, en vez de propagar `null`.

## Dependencias
- `HttpClient`: inyectado por constructor, mismo singleton compartido que el resto de los servicios.
- `movilTravelCompanion.Core.Models.UserPreferences` (tipo de entrada/salida de ambos métodos).

## Miembros públicos clave

### Propiedades / Campos
Ninguno público.

### Métodos
| Firma | Descripción |
|---|---|
| `Task<UserPreferences> GetPreferencesAsync(int userId)` | `GET /api/user/{userId}/preferences`. Si el usuario nunca guardó preferencias, el backend responde con los defaults sin crear fila; si el cuerpo deserializa a `null` de todos modos, el método devuelve `new UserPreferences()` (mismos defaults, por construcción del modelo). |
| `Task<UserPreferences> UpdatePreferencesAsync(int userId, UserPreferences preferences)` | `PUT /api/user/{userId}/preferences`, enviando `preferences` como cuerpo JSON directamente (serializado tal cual, sin envoltorio). Devuelve las preferencias confirmadas por el backend; si el cuerpo de respuesta es `null`, devuelve el mismo `preferences` que se envió como fallback. |

## Flujo y lógica relevante
`GetPreferencesAsync` y `UpdatePreferencesAsync` siguen el patrón estándar de los servicios de `Core`: verificar `IsSuccessStatusCode`, si falla leer el cuerpo de error y lanzar `HttpRequestException` con contexto; si tiene éxito, deserializar y aplicar un fallback no nulo. La particularidad de este servicio es que ambos fallbacks (`new UserPreferences()` en el GET, y el propio `preferences` de entrada en el PUT) ya representan datos "razonables" en vez de un error — reflejando que, según `CONTEXT.md`, un cuerpo vacío/sin preferencias previas no es un caso de error sino el comportamiento esperado del backend.

## Relaciones
- **Quién consume esta clase:** en el proyecto MAUI, `PreferencesViewModel` (lee/guarda preferencias contra el backend real) y `DashboardViewModel` (precarga `SelectedClothingStyle` y sensibilidad al frío desde Preferencias al entrar al Dashboard, con fallback silencioso a los defaults si la llamada falla — ese fallback está en el ViewModel, no en este servicio).
- **A quién usa esta clase:** `HttpClient` (inyectado), `UserPreferences` (modelo de entrada/salida).

## Notas de diseño
Implementa el contrato de `CONTEXT.md` sección "Preferencias de usuario (nuevo, sesión 2026-08-09)": valores válidos `defaultClothingStyle` ∈ {`"casual"`, `"formal"`, `"deportivo"`} y `coldSensitivity` ∈ {`"friolento"`, `"normal"`, `"caluroso"`} — esta clase no valida esos valores (la validación es responsabilidad del backend), solo transporta lo que la UI le pasa.
