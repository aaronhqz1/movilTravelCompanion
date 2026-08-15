# movilTravelCompanion.MauiProgram

## Ubicación
`movilTravelCompanion/MauiProgram.cs`

## Propósito
Punto de entrada de configuración de la app: construye el `MauiApp`, registra fuentes, y configura el contenedor de inyección de dependencias (DI) con todos los servicios de `Core`, la implementación de sesión, y todas las páginas/ViewModels de la app.

## Tipo
Clase de arranque (DI) — clase `static` con un único método `CreateMauiApp()`.

## Responsabilidades
- Configurar fuentes tipográficas (`OpenSans-Regular`, `OpenSans-Semibold`).
- Registrar el `HttpClient` singleton con `BaseAddress = ApiConfig.BaseUrl`, compartido por todos los servicios HTTP de `Core`.
- Registrar como `Singleton` las implementaciones de los servicios de `Core`: `IWeatherApiService`, `IAuthService`, `IHistoryService`, `IPreferencesService`, `IClothingService`, y `ISessionStore` (implementado por `PreferencesSessionStore`, del proyecto MAUI).
- Registrar `AppShell` como `Transient`.
- Registrar cada par ViewModel + Page (7 pares) como `Transient`.
- Habilitar logging a la consola de debug en builds `DEBUG`.

## Dependencias
No recibe dependencias inyectadas (es la clase que configura el contenedor); referencia directamente las clases concretas que registra: `ApiConfig` (`movilTravelCompanion.Core.Configuration`), todos los `I*Service`/`*Service` de `movilTravelCompanion.Core.Services`, `PreferencesSessionStore` (`movilTravelCompanion.Services`), y todos los ViewModels/Pages de `movilTravelCompanion.ViewModels` / `movilTravelCompanion.Views`.

## Miembros públicos clave

### Propiedades observables / Bindable
No aplica (clase estática de configuración, sin estado).

### Comandos / Métodos
| Nombre | Descripción |
|---|---|
| `CreateMauiApp()` | Método estático único: construye `MauiAppBuilder`, configura fuentes, registra todos los servicios/páginas/ViewModels en `builder.Services`, habilita logging de debug, y devuelve el `MauiApp` construido. |

## Flujo y lógica relevante
- El `HttpClient` se registra como fábrica (`_ => new HttpClient { BaseAddress = new Uri(ApiConfig.BaseUrl) }`) para inyectar la URL base una sola vez; todos los servicios HTTP de `Core` (`WeatherApiService`, `AuthService`, `HistoryService`, `PreferencesService`, `ClothingService`) reciben esta misma instancia por ser `Singleton`.
- `ISessionStore` se resuelve a `PreferencesSessionStore` — la única implementación concreta, ya que `Core` no puede usar `Preferences` (API de plataforma) al ser una librería `net10.0` pura sin el workload de Android.
- Los servicios son `Singleton` (una sola instancia para toda la vida de la app); las páginas y ViewModels son `Transient` (una instancia nueva cada vez que se resuelven), patrón estándar en MAUI para que cada navegación obtenga un ViewModel limpio.
- No hay lógica condicional de entorno (por ejemplo, para elegir entre emulador y dispositivo físico): el cambio de `ApiConfig.BaseUrl` es manual y requiere recompilar.

## Data binding (si es Page/ViewModel)
No aplica — no es una Page ni un ViewModel.

## Relaciones
- Construye el grafo de dependencias completo consumido por `App.xaml.cs` (que resuelve `AppShell`), y transitivamente por todas las páginas/ViewModels/servicios de la app.
- Es el único lugar donde se conectan explícitamente las interfaces de `movilTravelCompanion.Core.Services` con sus implementaciones concretas.

## Notas de diseño
Refleja la decisión de arquitectura documentada en CONTEXT.md ("Decisiones ya tomadas"): separación estricta entre UI (proyecto `movilTravelCompanion`) y lógica de negocio (`movilTravelCompanion.Core`) mediante `CommunityToolkit.Mvvm` + inyección de dependencias. La ausencia de `PreferencesService`/`ClothingService` en versiones anteriores de este archivo (ver historial de CONTEXT.md) corresponde a la sesión 2026-08-09, cuando se agregó la funcionalidad de Preferencias y recomendación de vestimenta.
