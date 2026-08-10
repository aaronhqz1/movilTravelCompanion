# movilTravelCompanion.ViewModels.LoginViewModel

## Ubicación
- ViewModel: `movilTravelCompanion/ViewModels/LoginViewModel.cs`
- Page asociada: `movilTravelCompanion/Views/LoginPage.xaml` + `movilTravelCompanion/Views/LoginPage.xaml.cs`

## Propósito
ViewModel de la pantalla de autenticación. Valida credenciales contra el backend, guarda la sesión resultante y navega siempre a `TravelDestinationPage` (nunca directo a Dashboard), replicando el flujo real de `App.jsx` de la app original.

## Tipo
ViewModel (`ObservableObject`) + Page (code-behind + XAML), acoplados 1:1.

## Responsabilidades
- Capturar usuario/contraseña y enviarlos a `IAuthService.LoginAsync`.
- Persistir el usuario devuelto en `ISessionStore` tras un login exitoso.
- Navegar a `TravelDestinationPage` tras login exitoso; navegar a `RegisterPage` desde el link "Crear cuenta".
- Mostrar estado de carga y errores (credenciales inválidas, fallo de red, etc.).

## Dependencias
- `IAuthService` (`movilTravelCompanion.Core.Services`) — llamada HTTP de login.
- `ISessionStore` (`movilTravelCompanion.Core.Services`, implementado por `PreferencesSessionStore`) — persistencia de la sesión.

## Miembros públicos clave

### Propiedades observables / Bindable
| Nombre | Tipo | Descripción |
|---|---|---|
| `Username` | `string` | Texto del campo usuario. |
| `Password` | `string` | Texto del campo contraseña (`Entry IsPassword="True"`). |
| `IsLoading` | `bool` | Estado de carga durante el login. |
| `ErrorMessage` | `string` | Mensaje de error (credenciales inválidas, fallo de conexión, etc.). |

### Comandos / Métodos
| Nombre | Descripción |
|---|---|
| `LoginCommand` (`LoginAsync`) | Llama a `_authService.LoginAsync(Username, Password)`; en éxito, guarda el `User` en `ISessionStore` y navega (push relativo) a `TravelDestinationPage`. En error, captura la excepción y la muestra en `ErrorMessage`. |
| `GoToRegisterCommand` (`GoToRegisterAsync`) | `Shell.Current.GoToAsync(nameof(Views.RegisterPage))` — push relativo. |

## Flujo y lógica relevante
- **Tras login, SIEMPRE se pasa por `TravelDestinationPage`** antes de llegar al Dashboard, replicando la regla de negocio documentada en CONTEXT.md ("Flujo real de navegación").
- **Navegación por push relativo, no absoluto**: `Shell.Current.GoToAsync(nameof(Views.TravelDestinationPage))`, sin `"//"`. Esto es intencional y está documentado en un comentario extenso en el código: `TravelDestinationPage` es una ruta global (`Routing.RegisterRoute` en `AppShell.xaml.cs`), no un `ShellContent`, y Shell no permite navegación absoluta hacia una ruta global que quedaría como única página en la pila (error "Global routes currently cannot be the only page on the stack"). Con push simple, el botón "atrás" podía (en versiones anteriores) volver a Login — comportamiento luego resuelto de forma incidental por `ShellNavigationHelper.TrimNavigationStack()`, aplicado desde `TravelDestinationViewModel.ConfirmAsync`.
- `Debug.WriteLine` registra el login exitoso (solo para depuración, no visible en producción).

## Data binding (si es Page/ViewModel)
`LoginPage.xaml` (`x:DataType="viewmodels:LoginViewModel"`, `Shell.FlyoutBehavior="Disabled"`):
- `Entry.Text` (usuario) ← `Username`.
- `Entry.Text` (contraseña, `IsPassword="True"`) ← `Password`.
- Botón "Iniciar Sesión" ← `LoginCommand`.
- Botón "Crear cuenta" ← `GoToRegisterCommand`.
- `ActivityIndicator` ← `IsLoading`.
- `Label` (error) ← `ErrorMessage`.

`LoginPage.xaml.cs` es minimalista: solo recibe `LoginViewModel` por constructor y lo setea como `BindingContext` — no hace override de `OnAppearing` (a diferencia de Home/Dashboard/Preferences) porque no necesita cargar nada al abrir.

## Relaciones
- Usa `IAuthService` (implementado por `AuthService` en Core) y `ISessionStore` (implementado por `PreferencesSessionStore`).
- Navega hacia `TravelDestinationPage` (éxito) y `RegisterPage` (link "Crear cuenta").
- Registrada como ruta global en `AppShell.xaml.cs`, alcanzable por push desde `HomePage` (`GoToLoginCommand` de `HomeViewModel`) y desde `RegistrationSuccessViewModel.ContinueAsync()`.

## Notas de diseño
Ver CONTEXT.md, sección *"Navegación con ruta absoluta ('//') rota hacia `TravelDestinationPage` y `DashboardPage` — RESUELTO"*: el bug original (login exitoso pero la app quedaba trabada en `LoginPage` con el error de "Global routes...") se corrigió cambiando `GoToAsync("//" + nameof(Pagina))` por `GoToAsync(nameof(Pagina))` en esta clase (línea ~48 según la nota) y en `TravelDestinationViewModel`. También se documenta que los `GoToAsync("//" + nameof(LoginPage))` usados en logout en su momento apuntaban correctamente a `LoginPage` porque entonces sí era el `ShellContent` real; al mover el `ShellContent` a `HomePage` (sesión posterior) esos casos se rompieron y corrigieron por separado — no afecta a esta clase, que nunca navega de vuelta a sí misma con ruta absoluta.
