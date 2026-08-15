# movilTravelCompanion.Services.PreferencesSessionStore

## Ubicación
`movilTravelCompanion/Services/PreferencesSessionStore.cs`

## Propósito
Implementación concreta de `ISessionStore` (definida en `movilTravelCompanion.Core.Services`, documentada en `docs/technical/Core/`) usando la API `Preferences` de MAUI (`Microsoft.Maui.Storage`) — el equivalente a `localStorage` del navegador en la app web original. Persiste el usuario logueado y su destino de viaje en disco entre aperturas de la app.

## Tipo
Servicio (implementación de interfaz de `Core`, vive en el proyecto MAUI porque `Preferences` es una API específica de plataforma).

## Responsabilidades
- Guardar (`SaveUser`), leer (`GetUser`) y limpiar (`ClearUser`) el usuario de la sesión actual, descompuesto en 8 claves primitivas individuales (`Preferences` solo soporta tipos primitivos: `string`, `int`, `double`, `bool`; no puede serializar un objeto `User` completo).
- Manejar valores `null`/opcionales (`HomeCity`, `HomeLatitude`, `HomeLongitude`, `TravelDestination`, `DestinationLatitude`, `DestinationLongitude`) removiendo la clave en vez de guardar un valor centinela.

## Dependencias
- `Preferences.Default` (API estática de `Microsoft.Maui.Storage`, no inyectada — se accede directamente).
- `User` (`movilTravelCompanion.Core.Models`), el modelo que se serializa/deserializa a claves individuales.

## Miembros públicos clave

### Propiedades observables / Bindable
Ninguna — no es una clase con estado observable, es un servicio de persistencia sin propiedades públicas (todas las claves son `private const string`).

### Comandos / Métodos
| Nombre | Descripción |
|---|---|
| `SaveUser(User user)` | Guarda `UserId` y `Username` (siempre presentes) y los 6 campos opcionales vía `SetOrRemove` (guarda si no es `null`, remueve la clave si es `null`). |
| `GetUser()` | Devuelve `null` si no existe `session_userId` (no hay sesión guardada); si existe, reconstruye un `User` completo leyendo las 8 claves, usando `GetStringOrNull`/`GetDoubleOrNull` para los campos opcionales. |
| `ClearUser()` | Remueve las 8 claves de `Preferences.Default` incondicionalmente (no falla si alguna no existe). |
| `SetOrRemove(string, string?)` / `SetOrRemove(string, double?)` | Helpers `private static` sobrecargados: si el valor es `null`, remueve la clave; si no, la setea. |
| `GetStringOrNull(string)` / `GetDoubleOrNull(string)` | Helpers `private static`: devuelven `null` si la clave no existe (`ContainsKey`), o el valor si existe. |

## Flujo y lógica relevante
No hay lógica de negocio ni validaciones — es persistencia pura. El único patrón notable es el manejo explícito de "clave ausente = valor `null`" para los campos opcionales (`HomeCity`, coordenadas de origen y de destino), necesario porque `Preferences.Get(key, defaultValue)` no distingue entre "la clave no existe" y "la clave existe con el valor por defecto" sin el chequeo previo de `ContainsKey`.

## Data binding (si es Page/ViewModel)
No aplica — es un servicio sin UI ni binding propio.

## Relaciones
- Implementa `ISessionStore` (interfaz en `movilTravelCompanion.Core.Services`, documentada en `docs/technical/Core/ISessionStore.md`). Core solo conoce la interfaz; no puede referenciar esta clase directamente porque `movilTravelCompanion.Core` es una librería `net10.0` pura sin el workload de Android/`Preferences`.
- Registrada como `Singleton` en `MauiProgram.cs` (`builder.Services.AddSingleton<ISessionStore, PreferencesSessionStore>()`).
- Consumida (a través de `ISessionStore`) por `LoginViewModel`, `TravelDestinationViewModel`, `DashboardViewModel`, `PreferencesViewModel` y `AppShell.xaml.cs` (para logout).

## Notas de diseño
Ver CONTEXT.md, "Flujo real de navegación" y "Estructura de proyecto objetivo": la separación `ISessionStore` (interfaz en Core) / `PreferencesSessionStore` (implementación en el proyecto MAUI) es una decisión de arquitectura explícita — Core no puede usar `Preferences` directo porque apunta a `net10.0` puro sin el workload de plataforma. Es el reemplazo directo de `localStorage`, usado en la web original para persistir sesión entre recargas. El backlog de CONTEXT.md nota que el destino de viaje ("`TravelDestination`") sigue siendo un único valor activo que se pisa al cambiarlo — no hay historial de destinos anteriores, decisión descartada explícitamente para la sesión 2026-08-09.
