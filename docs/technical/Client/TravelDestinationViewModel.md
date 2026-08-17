# movilTravelCompanion.ViewModels.TravelDestinationViewModel

## Ubicación
- ViewModel: `movilTravelCompanion/ViewModels/TravelDestinationViewModel.cs`
- Page asociada: `movilTravelCompanion/Views/TravelDestinationPage.xaml` + `movilTravelCompanion/Views/TravelDestinationPage.xaml.cs`

## Propósito
ViewModel de la pantalla de selección de ciudad de destino de viaje. Se alcanza siempre después del login (y también desde el Flyout, como "Cambiar Destino"), busca el clima de una ciudad y, al confirmar, guarda el destino en la sesión y navega al Dashboard.

## Tipo
ViewModel (`ObservableObject`) + Page (code-behind + XAML), acoplados 1:1.

## Responsabilidades
- Buscar el clima de una ciudad ingresada (`SearchAsync`), habilitando el resultado y el botón "Confirmar destino" solo si la búsqueda tuvo éxito.
- Al confirmar, actualizar el `User` de la sesión con el destino elegido (`TravelDestination`, `DestinationLatitude`, `DestinationLongitude`) y persistirlo vía `ISessionStore`.
- Navegar a `DashboardPage` tras confirmar, y recortar el `NavigationStack` para evitar crecimiento sin límite al usarse repetidamente desde el Flyout.
- Guard de sesión perdida: si no hay usuario en `ISessionStore` al confirmar, volver a `HomePage` en vez de crashear.

## Dependencias
- `IWeatherApiService` (`movilTravelCompanion.Core.Services`) — búsqueda de clima por ciudad.
- `ISessionStore` (`movilTravelCompanion.Core.Services`, implementado por `PreferencesSessionStore`) — lectura y escritura del `User` de la sesión.

## Miembros públicos clave

### Propiedades observables / Bindable
| Nombre | Tipo | Descripción |
|---|---|---|
| `CityQuery` | `string` | Texto ingresado en el `Entry` de búsqueda de destino. |
| `IsLoading` | `bool` | Estado de carga durante la búsqueda. |
| `ErrorMessage` | `string` | Mensaje de error de búsqueda. |
| `ResultCity` | `string` | Ciudad encontrada; queda vacía hasta que la búsqueda da resultado. |
| `ResultTemperature` | `string` | Temperatura formateada del resultado. |
| `HasResult` | `bool` | Controla la visibilidad del bloque de resultado + botón "Confirmar destino". |

### Comandos / Métodos
| Nombre | Descripción |
|---|---|
| `SearchCommand` (`SearchAsync`) | No-op si `CityQuery` está vacío; llama a `IWeatherApiService.SearchWeatherAsync(CityQuery)`, guarda el resultado completo en el campo privado `_searchResult` (necesario para tener lat/lon al confirmar) y llena `ResultCity`/`ResultTemperature`, `HasResult = true`. |
| `ConfirmCommand` (`ConfirmAsync`) | No-op si `_searchResult` es `null`; si no hay usuario en sesión, navega (`"//"`) a `HomePage` como guard defensivo; si hay usuario, actualiza sus campos de destino, lo persiste con `_sessionStore.SaveUser(user)`, navega (push) a `DashboardPage`, y llama a `ShellNavigationHelper.TrimNavigationStack()`. |

## Flujo y lógica relevante
- **`_searchResult` (campo privado, no observable) retiene el `WeatherData` completo** de la búsqueda, no solo los strings mostrados — necesario porque `ConfirmAsync` necesita `Latitude`/`Longitude`, que no se muestran en pantalla.
- **Guard de sesión perdida con `"//" + nameof(HomePage)`**: comentario explícito indica que "no debería pasar" (se llega aquí solo después de loguearse), pero por robustez se revisa igual. Usa ruta absoluta hacia `HomePage` porque es el único `ShellContent` válido para navegación `"//"`.
- **Push relativo hacia `DashboardPage`** (no `"//"`), con el mismo razonamiento que en `LoginViewModel`: `DashboardPage` es una ruta global (`Routing.RegisterRoute`), no un `ShellContent`, y la navegación absoluta a una ruta global que quedaría sola en la pila no está soportada por Shell.
- **`ShellNavigationHelper.TrimNavigationStack()` después del push a Dashboard**: sin esto, cada vez que se llega aquí desde el Flyout ("Cambiar Destino" con sesión ya iniciada), se apilarían `TravelDestinationPage` + `DashboardPage` nuevos sin sacar los anteriores, y la pila crecería sin límite. Efecto secundario (mejora): el botón "atrás" ahora vuelve directo a `HomePage` en vez de a `TravelDestinationPage`/`LoginPage`.

## Data binding (si es Page/ViewModel)
`TravelDestinationPage.xaml` (`x:DataType="viewmodels:TravelDestinationViewModel"`, `Shell.NavBarIsVisible="True"`):
- `Entry.Text` ← `CityQuery`, con `ReturnCommand="{Binding SearchCommand}"`.
- Botón "Buscar" ← `SearchCommand`.
- `ActivityIndicator` ← `IsLoading`; `Label` (error) ← `ErrorMessage`.
- Bloque `VerticalStackLayout` con `IsVisible="{Binding HasResult}"` que contiene `controls:WeatherCardView` (bindeado a `ResultCity`/`ResultTemperature`, sin `DetailsText` ni `ShowDetails`) y el botón "Confirmar destino" ← `ConfirmCommand`.

`TravelDestinationPage.xaml.cs` recibe el ViewModel por constructor (sin `OnAppearing`, a diferencia de Dashboard/Home/Preferences — no necesita cargar nada al abrir) y define `OnMenuClicked` para abrir el Flyout manualmente (`ToolbarItem` "Menú" en el XAML), porque el ícono automático de hamburguesa no aparece en páginas alcanzadas por push.

## Relaciones
- Usa `IWeatherApiService` (implementado por `WeatherApiService`) e `ISessionStore` (implementado por `PreferencesSessionStore`).
- Usa `WeatherCardView` (Control reutilizable) para mostrar el resultado.
- Navega hacia `DashboardPage` (confirmación) y `HomePage` (guard de sesión perdida).
- Alcanzada por push desde `LoginViewModel.LoginAsync()` (flujo normal post-login) y desde `AppShell.xaml.cs` (`GoToTravelDestinationCommand`, ítem "Cambiar Destino" del Flyout).

## Notas de diseño
Ver CONTEXT.md, sección *"`NavigationStack` crece sin límite al navegar por el Flyout — RESUELTO"*: el riesgo de agregar "Cambiar Destino" como ítem alcanzable repetidamente desde Dashboard estaba anticipado, pero verificado explícitamente resultó **peor de lo esperado** — un solo login + un uso del menú dejaba 6 pantallas apiladas. La llamada a `ShellNavigationHelper.TrimNavigationStack()` al final de `ConfirmAsync` es parte del fix. También ver *"Navegación con ruta absoluta ('//') rota"*: esta clase fue una de las dos (junto con `LoginViewModel`) donde se corrigió el cambio de `GoToAsync("//" + nombre)` a push relativo para llegar a `DashboardPage`.
