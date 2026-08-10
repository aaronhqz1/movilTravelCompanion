# movilTravelCompanion.ViewModels.RegistrationSuccessViewModel

## Ubicación
- ViewModel: `movilTravelCompanion/ViewModels/RegistrationSuccessViewModel.cs`
- Page asociada: `movilTravelCompanion/Views/RegistrationSuccessPage.xaml` + `movilTravelCompanion/Views/RegistrationSuccessPage.xaml.cs`

## Propósito
ViewModel de la pantalla de confirmación post-registro. Muestra un mensaje de bienvenida con el nombre de usuario recién creado (recibido por query parameter de Shell) y permite continuar hacia el login.

## Tipo
ViewModel (`ObservableObject`), el más simple de los 7 — sin dependencias inyectadas y con `[QueryProperty]` para recibir datos de la navegación anterior.

## Responsabilidades
- Recibir el `Username` pasado por `RegisterViewModel.RegisterAsync()` vía query parameter de Shell.
- Mostrar el mensaje de bienvenida formateado.
- Navegar hacia `LoginPage` al presionar "Continuar".

## Dependencias
Ninguna — es el único ViewModel del proyecto sin ningún servicio inyectado por constructor (no tiene constructor explícito).

## Miembros públicos clave

### Propiedades observables / Bindable
| Nombre | Tipo | Descripción |
|---|---|---|
| `Username` | `string` | Nombre del usuario recién registrado; poblado automáticamente por el framework de Shell vía `[QueryProperty(nameof(Username), "Username")]` a nivel de clase, no seteado manualmente. |

### Comandos / Métodos
| Nombre | Descripción |
|---|---|
| `ContinueCommand` (`ContinueAsync`) | `Shell.Current.GoToAsync(nameof(Views.LoginPage))` — push relativo hacia `LoginPage`. |

## Flujo y lógica relevante
- **Recepción de parámetros vía `[QueryProperty]`**: el atributo `[QueryProperty(nameof(Username), "Username")]` sobre la clase le indica a Shell que, al navegar aquí con un diccionario `{ ["Username"] = ... }` (como hace `RegisterViewModel`), asigne automáticamente el valor a la propiedad `Username` de esta instancia — sin código manual de parseo de query string.
- **Push relativo, no absoluto, hacia `LoginPage`**: comentario explícito en el código explica que esto es navegación *hacia adelante* (el usuario recién registrado debe loguearse antes de continuar), no un reset de pila — por eso se usa `GoToAsync(nameof(Views.LoginPage))` y no `"//" + nameof(HomePage)`. Como `LoginPage` es una ruta global (no `ShellContent`), tampoco sería válido navegar ahí con `"//"`.

## Data binding (si es Page/ViewModel)
`RegistrationSuccessPage.xaml` (`x:DataType="viewmodels:RegistrationSuccessViewModel"`, `Shell.FlyoutBehavior="Disabled"`):
- `Label.Text` con `StringFormat='Bienvenido, {0}. Tu cuenta se creo correctamente.'` ← `Username`.
- Botón "Continuar" ← `ContinueCommand`.

`RegistrationSuccessPage.xaml.cs` es minimalista: recibe el ViewModel por constructor y lo asigna a `BindingContext`, sin `OnAppearing` (no necesita cargar nada — `Username` ya llega poblado por Shell antes de que la página se muestre).

## Relaciones
- Alcanzada por push desde `RegisterViewModel.RegisterAsync()`, que le pasa `Username` por query parameter.
- Navega hacia `LoginPage`.

## Notas de diseño
Ver CONTEXT.md, "Estado actual": explícitamente documentado que esta página "recibe `Username` por query parameter de Shell". También en la sección *"Cambio de entry point"*: se aclara que el `ContinueAsync()` de esta clase es "navegación hacia adelante, no reset de stack", por lo que **no** se cambió a `"//HomePage"` cuando se corrigieron el resto de las navegaciones absolutas rotas tras mover el `ShellContent` de `LoginPage` a `HomePage` — se mantuvo como push relativo a `LoginPage` desde el principio, sin necesitar fix.
