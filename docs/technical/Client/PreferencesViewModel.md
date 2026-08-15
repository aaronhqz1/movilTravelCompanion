# movilTravelCompanion.ViewModels.PreferencesViewModel

## Ubicación
- ViewModel: `movilTravelCompanion/ViewModels/PreferencesViewModel.cs`
- Page asociada: `movilTravelCompanion/Views/PreferencesPage.xaml` + `movilTravelCompanion/Views/PreferencesPage.xaml.cs`

## Propósito
ViewModel de la pantalla de Preferencias del usuario (nueva en la sesión 2026-08-09, sin equivalente en la app web original): lee y guarda contra el backend el estilo de vestimenta por defecto y la sensibilidad al frío/calor, usados luego por `DashboardViewModel` para precargar la sección de recomendación de vestimenta.

## Tipo
ViewModel (`ObservableObject`) + Page (code-behind + XAML), acoplados 1:1.

## Responsabilidades
- Al aparecer: verificar sesión activa y cargar las preferencias actuales del usuario desde el backend.
- Guardar las preferencias elegidas (estilo de vestimenta + sensibilidad al frío) contra el backend.
- Mostrar estado de carga, error y confirmación de guardado.

## Dependencias
- `IPreferencesService` (`movilTravelCompanion.Core.Services`) — GET/PUT de preferencias contra el backend.
- `ISessionStore` (`movilTravelCompanion.Core.Services`, implementado por `PreferencesSessionStore`) — usuario de la sesión activa (para obtener `UserId`).

## Miembros públicos clave

### Propiedades observables / Bindable
| Nombre | Tipo | Descripción |
|---|---|---|
| `SelectedClothingStyle` | `string` | Estilo de vestimenta elegido (`"casual"` por default), bindeado directo a un `RadioButtonGroup.SelectedValue`. Valores válidos: `casual`/`formal`/`deportivo`. |
| `SelectedColdSensitivity` | `string` | Sensibilidad al frío/calor elegida (`"normal"` por default), bindeado directo a otro `RadioButtonGroup.SelectedValue`. Valores válidos: `friolento`/`normal`/`caluroso`. |
| `IsLoading` | `bool` | Estado de carga (tanto al leer como al guardar). |
| `ErrorMessage` | `string` | Mensaje de error de lectura/guardado. |
| `StatusMessage` | `string` | Mensaje de confirmación tras guardar exitosamente (`"Preferencias guardadas."`), mostrado en verde. |

### Comandos / Métodos
| Nombre | Descripción |
|---|---|
| `LoadAsync()` | Método público (no `[RelayCommand]`, llamado desde `PreferencesPage.OnAppearing()`): guard de sesión; si hay usuario, llama a `IPreferencesService.GetPreferencesAsync(user.UserId)` y llena `SelectedClothingStyle`/`SelectedColdSensitivity`. |
| `SaveCommand` (`SaveAsync`) | Guard de sesión repetido (por si se perdió entre la carga y el guardado); llama a `IPreferencesService.UpdatePreferencesAsync(user.UserId, new UserPreferences {...})` y setea `StatusMessage` en éxito. |

## Flujo y lógica relevante
- **Guard de sesión duplicado en `LoadAsync` y `SaveAsync`**: ambos métodos revisan independientemente `_sessionStore.GetUser()` y navegan a `"//" + nameof(Views.HomePage)` si es `null` — no hay una verificación única compartida, cada acción se protege por separado.
- **`RadioButtonGroup.SelectedValue` bindea directo el string elegido, sin convertidor**: decisión explícita documentada en CONTEXT.md, posible porque los valores de los `RadioButton.Value` en el XAML (`"casual"`, `"formal"`, etc.) ya coinciden textualmente con lo que espera el backend.
- No hay validación client-side adicional — los únicos valores posibles son los que ofrecen los `RadioButton`, por lo que no puede llegar un valor inválido desde la UI.

## Data binding (si es Page/ViewModel)
`PreferencesPage.xaml` (`x:DataType="viewmodels:PreferencesViewModel"`, `Shell.NavBarIsVisible="True"`, `Title="Preferencias"`):
- `ActivityIndicator` ← `IsLoading`; `Label` (error) ← `ErrorMessage`; `Label` (status, verde) ← `StatusMessage`.
- `VerticalStackLayout` con `RadioButtonGroup.GroupName="ClothingStyle"` y `RadioButtonGroup.SelectedValue="{Binding SelectedClothingStyle}"`, con 3 `RadioButton` (Casual/Formal/Deportivo).
- `VerticalStackLayout` con `RadioButtonGroup.GroupName="ColdSensitivity"` y `RadioButtonGroup.SelectedValue="{Binding SelectedColdSensitivity}"`, con 3 `RadioButton` (Friolento/a, Normal, Caluroso/a).
- Botón "Guardar" ← `SaveCommand`.

`PreferencesPage.xaml.cs` recibe el ViewModel por constructor, override `OnAppearing()` llama `LoadAsync()`, y define `OnMenuClicked` (handler del `ToolbarItem` "Menú") que hace `Shell.Current.FlyoutIsPresented = true` — mismo patrón que `DashboardPage.xaml.cs` y `TravelDestinationPage.xaml.cs`.

## Relaciones
- Usa `IPreferencesService` (implementado por `PreferencesService` en Core) e `ISessionStore` (implementado por `PreferencesSessionStore`).
- Las preferencias guardadas acá son leídas después por `DashboardViewModel.LoadAsync()` para precargar `SelectedClothingStyle` y la sensibilidad al frío en la sección de recomendación de vestimenta.
- Alcanzada por push únicamente desde `AppShell.xaml.cs` (`GoToPreferencesCommand`, ítem "Preferencias" del Flyout) — no hay otro punto de entrada.

## Notas de diseño
Ver CONTEXT.md, sección "Preferencias, menú Flyout y recomendación de vestimenta" (sesión 2026-08-09): esta página y su ViewModel son enteramente nuevos, sin equivalente en la app web original (`—` en la tabla de mapeo de componentes React → MAUI). El texto de CONTEXT.md confirma explícitamente: "`PreferencesPage.xaml` + `PreferencesViewModel`: lee/guarda contra el backend real, `RadioButtonGroup.SelectedValue` bindea directo el string elegido sin necesitar convertidor". Solo es alcanzable con sesión iniciada (Flyout deshabilitado en Home/Login/Register/RegistrationSuccess).
