# movilTravelCompanion.ShellNavigationHelper

## Ubicación
`movilTravelCompanion/ShellNavigationHelper.cs`

## Propósito
Evita que el `NavigationStack` de Shell crezca sin límite cuando el usuario navega repetidamente entre Dashboard/TravelDestination/Preferencias usando el Flyout, recortando la pila a solo la raíz (`HomePage`) y la página recién alcanzada.

## Tipo
Helper estático (`internal static class` con un único método estático, sin estado).

## Responsabilidades
- Recorrer el `NavigationStack` actual de `Shell.Current.Navigation` y eliminar (`Navigation.RemovePage(...)`) todas las páginas intermedias entre la raíz (`stack[0]`, siempre `HomePage`) y la página recién agregada (`stack[^1]`).

## Dependencias
Ninguna inyectada — accede directamente a `Shell.Current` (API estática de MAUI Shell), no recibe nada por constructor.

## Miembros públicos clave

### Propiedades observables / Bindable
Ninguna.

### Comandos / Métodos
| Nombre | Descripción |
|---|---|
| `TrimNavigationStack()` | Método estático público (visibilidad `internal` a nivel de clase): itera `stack.Count - 2` hasta `1` (de atrás hacia adelante, salteando índice 0 y el último) removiendo cada página intermedia del `NavigationStack` de Shell. |

## Flujo y lógica relevante
Se invoca **después** de cada push relevante, nunca antes:
- Al final de los 3 `[RelayCommand]` de navegación del Flyout en `AppShell.xaml.cs` (`GoToDashboardAsync`, `GoToTravelDestinationAsync`, `GoToPreferencesAsync`).
- Al final de `TravelDestinationViewModel.ConfirmAsync()`, tras el push a `DashboardPage`.

El recorrido va de atrás hacia adelante (`for i = stack.Count - 2; i >= 1; i--`) para no invalidar los índices de las posiciones aún no procesadas al remover elementos del medio. El índice `0` (la raíz, `HomePage`) y el último índice (la página recién alcanzada) quedan intactos siempre.

## Data binding (si es Page/ViewModel)
No aplica — no es Page ni ViewModel, es una utilidad estática consumida desde código.

## Relaciones
- Consumida por `AppShell.xaml.cs` (los 4 comandos del Flyout menos `LogoutAsync`, que usa ruta absoluta y no necesita recorte) y por `TravelDestinationViewModel.cs` (`ConfirmAsync`).
- Opera sobre `Shell.Current.Navigation`, que en este proyecto siempre tiene a `HomePage` como raíz (único `ShellContent` de `AppShell.xaml`).

## Notas de diseño
Ver CONTEXT.md, sección *"`NavigationStack` crece sin límite al navegar por el Flyout — RESUELTO"*. El riesgo ya estaba anticipado al planificar la sesión del 2026-08-09 (agregar "Cambiar Destino" como ítem de menú alcanzable *desde* Dashboard), pero al verificarlo resultó **peor de lo esperado**: no era solo "Dashboard duplicado una vez" — cada ciclo Dashboard → menú → Cambiar Destino → confirmar apilaba una instancia nueva de `TravelDestinationPage` y `DashboardPage` sin sacar las anteriores, y un solo login + un uso del menú ya dejaba 6 pantallas apiladas (`Home ← Login ← TravelDestination ← Dashboard ← TravelDestination ← Dashboard`), creciendo sin límite con cada uso repetido.

La técnica usada (`Navigation.RemovePage()` sobre todo lo que no sea raíz ni página actual) es estándar de MAUI Shell para este problema. Efecto secundario positivo, no regresión: el botón "atrás" físico de Android desde Dashboard/TravelDestination/Preferencias ahora vuelve directo a `HomePage` en vez de reproducir Login/TravelDestination intermedios — esto además resuelve el efecto secundario que había quedado aceptado para v1 en la nota de CONTEXT.md sobre navegación con ruta absoluta rota (cuando se cambió `"//" + nombre` por push relativo en `LoginViewModel`/`TravelDestinationViewModel`, el botón "atrás" podía volver a una pantalla intermedia; con `TrimNavigationStack()` ese problema queda resuelto de forma incidental).

Verificado en runtime: dos ciclos completos de "Cambiar Destino" (Berlín, luego Cairo) seguidos de un solo toque de "atrás" siguen llevando directo a Home; la pila no crece con el uso repetido.
