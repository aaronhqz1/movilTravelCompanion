# movilTravelCompanion.AppShell

## Ubicación
`movilTravelCompanion/AppShell.xaml`, `movilTravelCompanion/AppShell.xaml.cs`

## Propósito
Shell raíz de navegación de toda la app. Define la única página de entrada (`HomePage`), registra las rutas "sueltas" a las que se navega por push (`Login`, `Register`, `RegistrationSuccess`, `TravelDestination`, `Dashboard`, `Preferences`), y expone el menú Flyout (accesible solo con sesión iniciada) con las acciones Viaje Actual / Cambiar Destino / Preferencias / Cerrar Sesión.

## Tipo
Shell raíz (`Microsoft.Maui.Controls.Shell`), con su propio `BindingContext = this` (actúa como su propio ViewModel para los comandos del Flyout).

## Responsabilidades
- Declarar `HomePage` como el único `ShellContent` real de la app (la raíz de la pila de navegación).
- Registrar en el constructor, vía `Routing.RegisterRoute(...)`, las páginas que se alcanzan por navegación push (`Shell.Current.GoToAsync(nameof(Pagina))`) en vez de ser tabs/secciones propias.
- Exponer 4 `MenuItem` en el Flyout, cada uno con un `[RelayCommand]` que navega y luego recorta la pila de navegación con `ShellNavigationHelper.TrimNavigationStack()`.
- Implementar el cierre de sesión: limpiar el `ISessionStore` y volver a `HomePage` con ruta absoluta.

## Dependencias
- `ISessionStore` (inyectado por constructor, desde `movilTravelCompanion.Core.Services`) — usado en `LogoutAsync()` para limpiar la sesión persistida.

## Miembros públicos clave

### Propiedades observables / Bindable
Ninguna propiedad observable propia; el XAML bindea directamente los `[RelayCommand]` generados (ver tabla siguiente) porque `BindingContext = this`.

### Comandos / Métodos
| Nombre | Descripción |
|---|---|
| `GoToDashboardCommand` (`GoToDashboardAsync`) | Cierra el Flyout, navega a `DashboardPage` (push relativo) y recorta la pila. |
| `GoToTravelDestinationCommand` (`GoToTravelDestinationAsync`) | Cierra el Flyout, navega a `TravelDestinationPage` (push relativo) y recorta la pila. |
| `GoToPreferencesCommand` (`GoToPreferencesAsync`) | Cierra el Flyout, navega a `PreferencesPage` (push relativo) y recorta la pila. |
| `LogoutCommand` (`LogoutAsync`) | Cierra el Flyout, llama a `_sessionStore.ClearUser()` y navega con ruta **absoluta** (`"//" + nameof(HomePage)`) de vuelta a `HomePage`. |

## Flujo y lógica relevante
- **`HomePage` es el único `ShellContent`.** Todas las demás páginas de negocio (`LoginPage`, `RegisterPage`, `RegistrationSuccessPage`, `TravelDestinationPage`, `DashboardPage`, `PreferencesPage`) se registran como rutas globales sueltas y se alcanzan por *push* (`GoToAsync(nameof(Pagina))`), nunca como `ShellContent`/tab.
- **Navegación absoluta solo hacia `HomePage`.** MAUI Shell no permite navegación absoluta (`"//"`) hacia una ruta registrada con `Routing.RegisterRoute` si esa ruta quedaría como única página en la pila ("Global routes currently cannot be the only page on the stack"). Por eso `LogoutAsync` usa `"//" + nameof(HomePage)` (válido, es el `ShellContent` real) y no `"//" + nameof(LoginPage)`.
- **`ShellNavigationHelper.TrimNavigationStack()` después de cada navegación del Flyout.** Sin esto, cada ciclo Dashboard → menú → otra página apilaba páginas nuevas sin sacar las anteriores, haciendo crecer el `NavigationStack` sin límite (ver Notas de diseño).
- **`MenuItem`, no `FlyoutItem`.** Decisión explícita: ver Notas de diseño.

## Data binding (si es Page/ViewModel)
`AppShell.xaml` bindea sus 4 `MenuItem.Command` contra los `[RelayCommand]` de `AppShell.xaml.cs` (`GoToDashboardCommand`, `GoToTravelDestinationCommand`, `GoToPreferencesCommand`, `LogoutCommand`), posible porque el constructor setea `BindingContext = this` — `AppShell` actúa como su propio "ViewModel" para estos comandos, no delega en una clase separada.

## Relaciones
- Contiene y navega hacia: `HomePage`, `LoginPage`, `RegisterPage`, `RegistrationSuccessPage`, `TravelDestinationPage`, `DashboardPage`, `PreferencesPage` (todas en `movilTravelCompanion.Views`).
- Usa `ISessionStore` (implementado por `PreferencesSessionStore`) para limpiar la sesión al cerrar sesión.
- Usa `ShellNavigationHelper` (clase estática, mismo proyecto) para recortar la pila de navegación.
- Resuelto por `App.xaml.cs` vía DI (`_serviceProvider.GetRequiredService<AppShell>()`), registrado como `Transient` en `MauiProgram.cs`.

## Notas de diseño
Varias decisiones de esta clase están documentadas extensamente en `CONTEXT.md`, sección "Preferencias, menú Flyout y recomendación de vestimenta" y "Notas técnicas / troubleshooting":

- **Cambio de entry point (`HomePage` en vez de `LoginPage`)**: originalmente el único `ShellContent` era `LoginPage`. Se cambió para replicar el flujo real de la app original (`App.jsx`: `home → login → register → ... → dashboard`). Este cambio rompió temporalmente los `GoToAsync("//" + nameof(LoginPage))` usados en logout, que hubo que corregir a `"//" + nameof(HomePage)`.
- **`MenuItem` en vez de `FlyoutItem` — decisión tomada explícitamente**: la especificación original pedía `FlyoutItem` para las 3 páginas navegables. Un `FlyoutItem` de Shell no es "un ítem de menú que navega a una página" sino una *sección* completa con su propio `ShellContent` y su propia pila, navegada con rutas absolutas — chocaría con las rutas sueltas (`Routing.RegisterRoute`) que `LoginViewModel`/`TravelDestinationViewModel` ya usan para llegar a las mismas páginas por push. Se optó por `MenuItem` con `Command` que hacen push, mismo patrón que el resto de la app, a costa de perder el agrupamiento visual nativo de Shell.
- **Icono de hamburguesa ausente en páginas alcanzadas por push**: el ícono automático de Shell para abrir el Flyout solo aparece en la página raíz de una sección; como Dashboard/TravelDestination/Preferencias siempre se alcanzan por push, Shell muestra la flecha de "atrás" en su lugar, y el gesto de swipe desde el borde compite con el gesto nativo de "atrás" de Android. Fix aplicado en esas 3 páginas (no en `AppShell`): un `ToolbarItem` explícito que hace `Shell.Current.FlyoutIsPresented = true`.
- **`NavigationStack` sin límite — RESUELTO**: verificado en runtime que un solo login + un uso del menú dejaba 6 pantallas apiladas (`Home ← Login ← TravelDestination ← Dashboard ← TravelDestination ← Dashboard`). Fix: `ShellNavigationHelper.TrimNavigationStack()` llamado al final de los 3 comandos de navegación de esta clase (y también desde `TravelDestinationViewModel.ConfirmAsync`). Efecto secundario positivo: el botón "atrás" desde Dashboard/TravelDestination/Preferencias ahora vuelve directo a `HomePage`.
- El botón "Cerrar sesión" y su lógica se movieron desde `DashboardPage`/`DashboardViewModel` hacia el `MenuItem` de esta clase en la sesión del 2026-08-09, sin duplicar lógica.
- `Shell.NavBarIsVisible="False"` a nivel de `AppShell`; se sobreescribe a `"True"` en las páginas que sí necesitan barra superior (Dashboard, TravelDestination, Preferencias), y explícitamente `Shell.FlyoutBehavior="Disabled"` en Home/Login/Register/RegistrationSuccess para que el Flyout solo sea alcanzable con sesión iniciada.
