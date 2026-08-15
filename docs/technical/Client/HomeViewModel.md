# movilTravelCompanion.ViewModels.HomeViewModel

## Ubicación
- ViewModel: `movilTravelCompanion/ViewModels/HomeViewModel.cs`
- Page asociada: `movilTravelCompanion/Views/HomePage.xaml` + `movilTravelCompanion/Views/HomePage.xaml.cs`

## Propósito
ViewModel de la pantalla de entrada de la app (`HomePage`, único `ShellContent` de `AppShell`). Muestra el clima de una ciudad aleatoria al abrir la app (sin necesidad de sesión) y permite buscar el clima de otra ciudad o navegar a Login/Registro. Equivalente MAUI de `Home.jsx` de la app web original.

## Tipo
ViewModel (`ObservableObject` de `CommunityToolkit.Mvvm`) + Page (code-behind + XAML), acoplados 1:1.

## Responsabilidades
- Cargar clima de una ciudad aleatoria al aparecer la página (`LoadRandomWeatherAsync`, llamado desde `HomePage.OnAppearing()`).
- Buscar el clima de una ciudad ingresada por el usuario.
- Navegar a `LoginPage` y `RegisterPage`.
- No requiere sesión iniciada — es la única página accesible sin login (junto con Login/Register).

## Dependencias
- `IWeatherApiService` (`movilTravelCompanion.Core.Services`, inyectado por constructor) — clima aleatorio y búsqueda por ciudad.

## Miembros públicos clave

### Propiedades observables / Bindable
| Nombre | Tipo | Descripción |
|---|---|---|
| `CityQuery` | `string` | Texto ingresado en el `Entry` de búsqueda. |
| `ResultCity` | `string` | Ciudad del resultado de clima mostrado (aleatorio o buscado). |
| `ResultTemperature` | `string` | Temperatura formateada (`"18.3 °C"`). |
| `ResultDetails` | `string` | Humedad y viento formateados (`"Humedad 60% · Viento 12 km/h"`). |
| `HasResult` | `bool` | Controla la visibilidad de `WeatherCardView` en el XAML. |
| `IsLoading` | `bool` | Muestra/oculta el `ActivityIndicator`. |
| `ErrorMessage` | `string` | Mensaje de error mostrado en rojo si falla la carga o búsqueda. |

### Comandos / Métodos
| Nombre | Descripción |
|---|---|
| `LoadRandomWeatherAsync()` | Método público (no `[RelayCommand]`, se llama manualmente desde `OnAppearing`): pide clima aleatorio a `IWeatherApiService.GetRandomWeatherAsync()` y aplica el resultado. |
| `SearchCommand` (`SearchAsync`) | Busca clima de `CityQuery` (no-op si está vacío); en éxito llena `ResultCity`/`ResultTemperature`/`ResultDetails` y setea `HasResult = true`. |
| `GoToLoginCommand` (`GoToLoginAsync`) | `Shell.Current.GoToAsync(nameof(Views.LoginPage))` — push relativo. |
| `GoToRegisterCommand` (`GoToRegisterAsync`) | `Shell.Current.GoToAsync(nameof(Views.RegisterPage))` — push relativo. |
| `ApplyWeather(WeatherData)` | Método `private`: mapea el `WeatherData` recibido a las propiedades de texto formateadas y setea `HasResult = true`. |

## Flujo y lógica relevante
- `HomePage.OnAppearing()` llama a `_viewModel.LoadRandomWeatherAsync()` — el equivalente MAUI a un `useEffect(() => {...}, [])` que corre cada vez que la pantalla se muestra, necesario porque el constructor de `HomeViewModel` no puede ser `async`.
- Sin manejo de excepciones especial más allá de capturar y mostrar `ex.Message` en `ErrorMessage`; no hay reintentos ni validación de formato de ciudad.
- No navega a ninguna ruta con `"//"` (siempre push relativo), porque `HomePage` es la raíz — no tiene sentido "resetear" la pila hacia sí misma desde aquí.

## Data binding (si es Page/ViewModel)
`HomePage.xaml` (`x:DataType="viewmodels:HomeViewModel"`, `Shell.FlyoutBehavior="Disabled"`):
- `ActivityIndicator.IsRunning`/`IsVisible` ← `IsLoading`.
- `Label.Text` (error) ← `ErrorMessage`.
- `controls:WeatherCardView` ← `IsVisible` a `HasResult`, `CityText`/`TemperatureText`/`DetailsText` a `ResultCity`/`ResultTemperature`/`ResultDetails`, `ShowDetails="True"`.
- `Entry.Text` ← `CityQuery`, con `ReturnCommand="{Binding SearchCommand}"` (buscar al presionar Enter/Intro).
- Botón "Buscar" ← `SearchCommand`.
- Botón "Iniciar Sesión" ← `GoToLoginCommand`.
- Botón "Crear cuenta" ← `GoToRegisterCommand`.

`HomePage.xaml.cs` recibe `HomeViewModel` por constructor (inyección de DI, registrado `Transient` en `MauiProgram.cs`), lo setea como `BindingContext`, y hace override de `OnAppearing()` para disparar la carga inicial.

## Relaciones
- Usa `IWeatherApiService` de `Core` (implementado por `WeatherApiService`, documentado en `docs/technical/Core/`).
- Navega hacia `LoginPage` y `RegisterPage`.
- `HomePage` es el único `ShellContent` de `AppShell.xaml` — todo el resto de la app se alcanza (directa o indirectamente) por push desde acá, y toda navegación de "reset" (logout, sesión perdida) vuelve acá con `"//" + nameof(HomePage)`.
- Usa `WeatherCardView` (Control reutilizable) para mostrar el resultado.

## Notas de diseño
Ver CONTEXT.md, sección *"Cambio de entry point: `HomePage` pasa a ser el `ShellContent` inicial (antes era `LoginPage`)"*: se decidió que `HomePage` sea la pantalla de entrada para replicar el flujo real de la app original (`App.jsx`: `home → login → register → ... → dashboard`), en vez de arrancar directo en `LoginPage` como en una versión anterior del proyecto. Este cambio rompió (y hubo que corregir) las navegaciones absolutas de logout/guards de sesión perdida en `DashboardViewModel`, `TravelDestinationViewModel` y `RegistrationSuccessViewModel`, que antes apuntaban a `"//" + nameof(LoginPage)`.
