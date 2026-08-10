# movilTravelCompanion.Core.Services.ISessionStore

## Ubicación
`movilTravelCompanion.Core/Services/ISessionStore.cs`

## Propósito
Define la abstracción sobre "dónde y cómo se guarda la sesión del usuario actual", sin comprometer a `Core` con una API específica de plataforma. Reemplaza el uso de `localStorage` que tenía la app web original (React) para persistir la sesión entre recargas.

## Tipo
Interfaz de servicio (sin implementación en este proyecto — ver "Relaciones").

## Responsabilidades
- Declarar el contrato para guardar el usuario logueado (`SaveUser`).
- Declarar el contrato para recuperar el usuario actual, si existe (`GetUser`).
- Declarar el contrato para limpiar la sesión (`ClearUser`), usado al cerrar sesión.

## Dependencias
- `movilTravelCompanion.Core.Models.User` (tipo de entrada/salida de los métodos).

## Miembros públicos clave

### Propiedades / Campos
Ninguno (es una interfaz, sin campos).

### Métodos
| Firma | Descripción |
|---|---|
| `void SaveUser(User user)` | Persiste el usuario dado como la sesión activa. Síncrono (no `Task`), a diferencia de los servicios HTTP de `Core`. |
| `User? GetUser()` | Devuelve el usuario de la sesión activa, o `null` si no hay ninguna sesión guardada (usuario no logueado / sesión cerrada / primera ejecución). |
| `void ClearUser()` | Elimina la sesión activa, si existe. |

## Flujo y lógica relevante
Es una interfaz pura, sin lógica: el propósito explícito, documentado en un comentario del propio archivo, es que `Core` "solo conoce esta interfaz, igual que conoce `IAuthService` sin saber que hay un `HttpClient` del otro lado" — es decir, `Core` depende de la abstracción de almacenamiento de sesión sin saber si detrás hay `Preferences`, `SecureStorage`, o cualquier otro mecanismo. La API es intencionalmente síncrona (`void`/valor de retorno directo, sin `async`/`Task`) porque el almacenamiento subyacente (`Preferences` de MAUI Essentials) es una operación local rápida, a diferencia de las llamadas HTTP de los demás servicios de `Core`.

## Relaciones
- **Quién consume esta clase:** en el proyecto MAUI, `LoginViewModel` (guarda la sesión al loguearse), `TravelDestinationViewModel` (guarda el destino elegido actualizando el `User` de la sesión), `DashboardViewModel` (lee la sesión para obtener `userId`/destino, y como guard de "sesión perdida" que redirige a `HomePage`), `PreferencesViewModel` (lee `userId` de la sesión para las llamadas a `IPreferencesService`), y `AppShell.xaml.cs` (comando de "Cerrar sesión" del Flyout, que llama `ClearUser`).
- **A quién usa esta clase:** `User` (modelo transportado).
- **Implementación concreta:** `PreferencesSessionStore`, ubicada en `movilTravelCompanion/Services/PreferencesSessionStore.cs` (proyecto MAUI, no en `Core`). Usa la API `Preferences` de MAUI Essentials para persistir el usuario serializado en el almacenamiento local del dispositivo. Vive fuera de `Core` porque `Core` es una librería `net10.0` pura sin el workload de plataforma Android/MAUI necesario para acceder a `Preferences`. Su documentación detallada corresponde a otro documento técnico (fuera del alcance de este documento).

## Notas de diseño
`CONTEXT.md` documenta esta separación explícitamente en la sección de estructura del proyecto: "`ISessionStore.cs` (solo la interfaz; la implementación está en el proyecto MAUI)" y, en "Estado actual": "`ISessionStore` (interfaz, en Core) + `PreferencesSessionStore` (implementación con `Preferences`, en el proyecto MAUI — Core no puede usar `Preferences` directo porque apunta a `net10.0` puro, sin el workload de plataforma)". Es el mismo patrón de inversión de dependencias que separa `IAuthService`/`AuthService` de `HttpClient`, pero aplicado a almacenamiento local en vez de red. También reemplaza conceptualmente a `localStorage` de la versión web original (React), mencionado en `CONTEXT.md` en la sección "Flujo real de navegación".
