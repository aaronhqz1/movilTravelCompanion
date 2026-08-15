# movilTravelCompanion.ViewModels.RegisterViewModel

## Ubicación
- ViewModel: `movilTravelCompanion/ViewModels/RegisterViewModel.cs`
- Page asociada: `movilTravelCompanion/Views/RegisterPage.xaml` + `movilTravelCompanion/Views/RegisterPage.xaml.cs`

## Propósito
ViewModel de la pantalla de registro de usuario nuevo. Valida client-side que las contraseñas coincidan, crea la cuenta contra el backend, y navega a `RegistrationSuccessPage` pasando el nombre de usuario como query parameter de Shell.

## Tipo
ViewModel (`ObservableObject`) + Page (code-behind + XAML), acoplados 1:1.

## Responsabilidades
- Capturar usuario, contraseña, confirmación de contraseña y ciudad de origen (opcional).
- Validar client-side que `Password == ConfirmPassword` antes de llamar al backend.
- Registrar la cuenta vía `IAuthService.RegisterAsync`.
- Navegar a `RegistrationSuccessPage` pasando `Username` como query parameter tras éxito.
- Navegar hacia atrás (`".."`) hacia Login desde el link "Ya tengo cuenta".

## Dependencias
- `IAuthService` (`movilTravelCompanion.Core.Services`) — llamada HTTP de registro.

## Miembros públicos clave

### Propiedades observables / Bindable
| Nombre | Tipo | Descripción |
|---|---|---|
| `Username` | `string` | Usuario elegido. |
| `Password` | `string` | Contraseña. |
| `ConfirmPassword` | `string` | Confirmación de contraseña, validada contra `Password` client-side. |
| `HomeCity` | `string` | Ciudad de origen, opcional — se envía `null` al backend si está vacía. |
| `IsLoading` | `bool` | Estado de carga durante el registro. |
| `ErrorMessage` | `string` | Mensaje de error (contraseñas no coinciden, usuario duplicado, fallo de red, etc.). |

### Comandos / Métodos
| Nombre | Descripción |
|---|---|
| `RegisterCommand` (`RegisterAsync`) | Valida `Password == ConfirmPassword` (si no coinciden, setea `ErrorMessage` y retorna sin llamar al backend); si coinciden, llama a `_authService.RegisterAsync(Username, Password, homeCityOrNull)` y en éxito navega a `RegistrationSuccessPage` pasando `Username` como parámetro de query de Shell. |
| `GoToLoginCommand` (`GoToLoginAsync`) | `Shell.Current.GoToAsync("..")` — navegación hacia atrás en la pila (pop), no push a una ruta nombrada. |

## Flujo y lógica relevante
- **Validación client-side de contraseñas** (ver CONTEXT.md, "Estado actual"): antes de tocar la red, si `Password != ConfirmPassword` se corta el flujo con `ErrorMessage = "Las contraseñas no coinciden."`.
- **`HomeCity` es opcional**: `string.IsNullOrWhiteSpace(HomeCity) ? null : HomeCity` — replica la regla de negocio de CONTEXT.md ("La ciudad de origen es opcional al registrarse; puede configurarse después").
- **Paso de parámetros vía Shell**: `GoToAsync(nameof(Views.RegistrationSuccessPage), new Dictionary<string, object> { ["Username"] = Username })` — mecanismo estándar de Shell para pasar datos entre páginas sin un servicio de estado compartido; el receptor lo declara con `[QueryProperty(nameof(Username), "Username")]` (ver `RegistrationSuccessViewModel`).
- **`GoToLoginAsync` usa `".."` en vez de `nameof(LoginPage)`**: pop relativo (vuelve a la página anterior en la pila), distinto del resto de los `GoToRegisterAsync`/`GoToLoginAsync` de otros ViewModels que navegan por nombre — aquí se asume que se llegó a `RegisterPage` por push desde `LoginPage` o `HomePage`, y ".." simplemente deshace ese push.

## Data binding (si es Page/ViewModel)
`RegisterPage.xaml` (`x:DataType="viewmodels:RegisterViewModel"`, `Shell.FlyoutBehavior="Disabled"`):
- 4 `Entry` bindean a `Username`, `Password` (`IsPassword="True"`), `ConfirmPassword` (`IsPassword="True"`), `HomeCity`.
- Botón "Registrarme" ← `RegisterCommand`.
- Botón "Ya tengo cuenta" ← `GoToLoginCommand`.
- `ActivityIndicator` ← `IsLoading`; `Label` (error) ← `ErrorMessage`.

`RegisterPage.xaml.cs` es minimalista, igual patrón que `LoginPage.xaml.cs`: recibe el ViewModel por constructor, lo asigna a `BindingContext`, sin lógica de `OnAppearing`.

## Relaciones
- Usa `IAuthService` (implementado por `AuthService` en Core).
- Navega hacia `RegistrationSuccessPage` (éxito, con query parameter) y hacia atrás con `".."` (link "Ya tengo cuenta").
- Alcanzable por push desde `HomePage` (`GoToRegisterCommand` de `HomeViewModel`) y desde `LoginPage` (`GoToRegisterCommand` de `LoginViewModel`).

## Notas de diseño
Ver CONTEXT.md, "Estado actual": la validación client-side de contraseñas fue explícitamente parte del alcance de v1 ("Construir `RegisterPage.xaml` + `RegisterViewModel` (con validación client-side de contraseñas)"). No hay notas de troubleshooting específicas de navegación para esta clase — el uso de `".."` en `GoToLoginAsync` nunca causó el bug de "Global routes cannot be the only page on the stack" porque no es una navegación absoluta hacia una ruta global, sino un pop simple.
