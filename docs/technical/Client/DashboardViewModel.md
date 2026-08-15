# movilTravelCompanion.ViewModels.DashboardViewModel

## Ubicación
- ViewModel: `movilTravelCompanion/ViewModels/DashboardViewModel.cs`
- Page asociada: `movilTravelCompanion/Views/DashboardPage.xaml` + `movilTravelCompanion/Views/DashboardPage.xaml.cs`

## Propósito
ViewModel del panel principal post-login. Muestra el clima del destino de viaje elegido, permite obtener una recomendación de vestimenta (OpenAI) según ese clima y las preferencias del usuario, permite buscar el clima de otra ciudad (guardándolo en el historial), y muestra las últimas 3 búsquedas.

## Tipo
ViewModel (`ObservableObject`) + Page (code-behind + XAML), acoplados 1:1. Es el ViewModel con más dependencias y responsabilidades del proyecto.

## Responsabilidades
- Al aparecer: verificar sesión activa, cargar el clima del destino elegido, cargar el historial reciente, y precargar estilo de vestimenta/sensibilidad al frío desde Preferencias.
- Buscar el clima de otra ciudad y guardarlo en el historial del backend.
- Pedir una recomendación de vestimenta a `IClothingService` usando el clima del destino, el estilo elegido y la sensibilidad al frío precargada.
- Guard de sesión perdida: si no hay usuario, volver a `HomePage`.

## Dependencias
- `IWeatherApiService` (`movilTravelCompanion.Core.Services`) — clima del destino por coordenadas.
- `IHistoryService` (`movilTravelCompanion.Core.Services`) — guardar y leer historial de búsquedas.
- `IClothingService` (`movilTravelCompanion.Core.Services`) — recomendación de vestimenta vía OpenAI (backend).
- `IPreferencesService` (`movilTravelCompanion.Core.Services`) — lectura de preferencias del usuario (estilo de vestimenta, sensibilidad al frío).
- `ISessionStore` (`movilTravelCompanion.Core.Services`, implementado por `PreferencesSessionStore`) — usuario y destino de la sesión activa.

## Miembros públicos clave

### Propiedades observables / Bindable
| Nombre | Tipo | Descripción |
|---|---|---|
| `WelcomeMessage` | `string` | `"Hola, {username}"`. |
| `DestinationCity` | `string` | Ciudad de destino elegida (de la sesión). |
| `DestinationTemperature` | `string` | Temperatura formateada del destino. |
| `DestinationDetails` | `string` | Humedad y viento formateados del destino. |
| `SearchCityQuery` | `string` | Texto de búsqueda de otra ciudad (se guarda en historial). |
| `IsLoading` | `bool` | Estado de carga del clima del destino / búsqueda / historial. |
| `ErrorMessage` | `string` | Error de carga/búsqueda de clima. |
| `SelectedClothingStyle` | `string` | Estilo elegido (`"casual"` por default), bindeado directo a un `RadioButtonGroup.SelectedValue`; también se precarga desde Preferencias en `LoadAsync`. |
| `IsLoadingClothing` | `bool` | Estado de carga de la recomendación de vestimenta. |
| `ClothingErrorMessage` | `string` | Error de la recomendación de vestimenta. |
| `ClothingRecommendationText` | `string` | Texto de la recomendación devuelta por OpenAI. |
| `RecentHistory` | `ObservableCollection<HistoryEntry>` | Últimas 3 búsquedas, propiedad de solo colección (no `[ObservableProperty]`), bindeada al `CollectionView`. |

### Comandos / Métodos
| Nombre | Descripción |
|---|---|
| `LoadAsync()` | Método público (no `[RelayCommand]`, llamado desde `DashboardPage.OnAppearing()`): guard de sesión, carga clima del destino (si hay coordenadas guardadas), recarga historial, y precarga preferencias de vestimenta (falla en silencio). |
| `SearchCommand` (`SearchAsync`) | Busca clima de `SearchCityQuery`, lo guarda en el historial vía `IHistoryService.AddAsync`, limpia el campo de búsqueda y recarga el historial. |
| `GetClothingRecommendationCommand` (`GetClothingRecommendationAsync`) | Si no hay clima de destino cargado (`_destinationWeather is null`), muestra error sin llamar al backend; si lo hay, pide la recomendación a `IClothingService.GetRecommendationAsync(...)` con ciudad, temperatura, código de clima, humedad, viento, estilo elegido y sensibilidad al frío precargada. |
| `ReloadHistoryAsync()` | Método `private`: releé las últimas búsquedas vía `IHistoryService.GetRecentAsync` y refresca `RecentHistory`. |
| `ApplyDestinationWeather(WeatherData)` | Método `private`: guarda el `WeatherData` completo en el campo `_destinationWeather` (retiene `WeatherCode`, no solo temperatura/humedad/viento) y formatea `DestinationTemperature`/`DestinationDetails`. |

## Flujo y lógica relevante
- **Guard de sesión en `LoadAsync`**: si `ISessionStore.GetUser()` devuelve `null`, navega con `"//" + nameof(Views.HomePage)` (única ruta válida para navegación absoluta) y retorna sin cargar nada más.
- **Carga de clima del destino condicional**: solo llama a `IWeatherApiService.GetWeatherByCoordinatesAsync(lat, lon)` si el usuario tiene `DestinationLatitude`/`DestinationLongitude` seteados (siempre debería tenerlos si pasó por `TravelDestinationPage`, pero el código lo trata como opcional).
- **Precarga de preferencias con fallo silencioso**: el bloque `try/catch` que llama a `IPreferencesService.GetPreferencesAsync` no propaga el error a `ErrorMessage` — si falla, `SelectedClothingStyle`/`_coldSensitivity` quedan en sus defaults (`"casual"` / `null`), sin bloquear el resto del Dashboard.
- **`_destinationWeather` retiene el objeto completo**, no solo strings formateados — cambio explícito de la sesión 2026-08-09 para no perder `WeatherCode` (necesario para el endpoint de OpenAI, que antes solo tenía acceso a temperatura/humedad/viento).
- **Búsqueda ≠ destino**: `SearchAsync` (buscar otra ciudad) es independiente de la sección de vestimenta — solo agrega al historial, no cambia `DestinationCity` ni `_destinationWeather`.
- **`SelectedClothingStyle` bindea directo un `RadioButtonGroup.SelectedValue`** sin necesitar convertidor, porque el valor del grupo de radio buttons ya es el string `"casual"`/`"formal"`/`"deportivo"` esperado por el backend.

## Data binding (si es Page/ViewModel)
`DashboardPage.xaml` (`x:DataType="viewmodels:DashboardViewModel"`, `Shell.NavBarIsVisible="True"`, `Title="Panel"`):
- `Label.Text` ← `WelcomeMessage`; `ActivityIndicator` ← `IsLoading`; `Label` (error) ← `ErrorMessage`.
- `controls:WeatherCardView` ← `DestinationCity`/`DestinationTemperature`/`DestinationDetails`, `ShowDetails="True"`.
- `RadioButtonGroup.SelectedValue` ← `SelectedClothingStyle` (grupo `DashboardClothingStyle`, valores `casual`/`formal`/`deportivo`).
- Botón "Obtener Recomendación de Vestimenta" ← `GetClothingRecommendationCommand`; `ActivityIndicator` ← `IsLoadingClothing`; `Label` (error) ← `ClothingErrorMessage`; `Label` (resultado) ← `ClothingRecommendationText`.
- `Entry.Text` ← `SearchCityQuery`, `ReturnCommand` ← `SearchCommand`; botón "Buscar" ← `SearchCommand`.
- `CollectionView.ItemsSource` ← `RecentHistory`, con `DataTemplate x:DataType="models:HistoryEntry"` que bindea `City` y `Temperature` (con `StringFormat`) por cada item.

`DashboardPage.xaml.cs` recibe el ViewModel por constructor, override `OnAppearing()` llama `LoadAsync()`, y define `OnMenuClicked` (handler del `ToolbarItem` "Menú") que hace `Shell.Current.FlyoutIsPresented = true`.

## Relaciones
- Usa `IWeatherApiService`, `IHistoryService`, `IClothingService`, `IPreferencesService` (todos de Core) e `ISessionStore` (implementado por `PreferencesSessionStore`).
- Usa `WeatherCardView` (Control reutilizable) y el modelo `HistoryEntry` (`Core.Models`) dentro del `DataTemplate` del `CollectionView`.
- Alcanzada por push desde `TravelDestinationViewModel.ConfirmAsync()` y desde `AppShell.xaml.cs` (`GoToDashboardCommand`, ítem "Viaje Actual" del Flyout).
- Navega hacia `HomePage` solo como guard defensivo de sesión perdida (no como flujo normal).

## Notas de diseño
Varias decisiones de esta clase provienen de la sesión 2026-08-09 documentada en CONTEXT.md:
- **`ApplyDestinationWeather` ahora retiene el `WeatherData` completo**: antes solo extraía temperatura/humedad/viento a strings, y se perdía el `weatherCode` necesario para el endpoint de recomendación de vestimenta — corregido explícitamente en esa sesión.
- **`LoadAsync` precarga `SelectedClothingStyle` y la sensibilidad al frío desde Preferencias**, con fallback silencioso a los defaults si falla, para no bloquear el resto del Dashboard.
- **Botón "Cerrar sesión" removido de esta clase**: en versiones anteriores vivía acá; se movió al `MenuItem` del Flyout en `AppShell.xaml.cs` en la sesión 2026-08-09, "sin duplicar lógica" (ver CONTEXT.md, "Estado actual").
- **Verificado en runtime con backend y OpenAI reales**: Tokio 31.3°C + Deportivo devolvió ropa liviana transpirable; Lima 19.8°C + Deportivo + sensibilidad "friolento" (precargada desde Preferencias) devolvió capas extra pese a la temperatura templada — confirma que ambos valores (clima y sensibilidad) llegan correctamente al prompt de OpenAI.
- Ver también "Navegación con ruta absoluta ('//') rota" en CONTEXT.md: el guard de sesión perdida de esta clase apuntaba originalmente a `"//" + nameof(LoginPage)` y se corrigió a `"//" + nameof(HomePage)` cuando `HomePage` pasó a ser el `ShellContent` real.
