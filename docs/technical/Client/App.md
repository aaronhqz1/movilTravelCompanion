# movilTravelCompanion.App

## Ubicación
`movilTravelCompanion/App.xaml`, `movilTravelCompanion/App.xaml.cs`

## Propósito
Clase raíz de la aplicación MAUI (`Microsoft.Maui.Controls.Application`). Define los recursos globales (colores y estilos) y construye la ventana principal resolviendo `AppShell` desde el contenedor de inyección de dependencias.

## Tipo
Clase de arranque (raíz de `Application` + configuración de `Application.Resources` en XAML).

## Responsabilidades
- Fusionar los diccionarios de recursos globales `Resources/Styles/Colors.xaml` y `Resources/Styles/Styles.xaml` en `Application.Resources`, disponibles luego como `{StaticResource ...}` en cualquier página.
- Construir la única `Window` de la app en `CreateWindow(...)`, usando `AppShell` (resuelto por DI) como contenido raíz.

## Dependencias
- `IServiceProvider` (inyectado por constructor) — usado para resolver `AppShell` de forma diferida dentro de `CreateWindow`, no en el constructor.

## Miembros públicos clave

### Propiedades observables / Bindable
Ninguna (no es una clase con estado observable ni bindable).

### Comandos / Métodos
| Nombre | Descripción |
|---|---|
| `App(IServiceProvider serviceProvider)` | Constructor: llama a `InitializeComponent()` (carga `Application.Resources`) y guarda el `IServiceProvider` para uso posterior. |
| `CreateWindow(IActivationState?)` | Override de `Application`: resuelve `AppShell` vía `_serviceProvider.GetRequiredService<AppShell>()` y lo envuelve en una `Window` nueva. |

## Flujo y lógica relevante
No maneja navegación ni lógica de negocio; su único trabajo es el arranque de la ventana. El orden de operaciones dentro de `CreateWindow` es deliberado: `AppShell` (y por extensión `HomePage`, su `ShellContent`) se resuelve **después** de que `InitializeComponent()` del constructor ya cargó `Application.Resources`, para evitar que el XAML de las páginas busque `StaticResource`s (como `Headline`) antes de que existan.

## Data binding (si es Page/ViewModel)
No aplica — `App` no tiene `BindingContext` ni se bindea desde XAML de negocio.

## Relaciones
- Resuelve `AppShell` (`movilTravelCompanion/AppShell.xaml.cs`) vía el contenedor de DI configurado en `MauiProgram.cs`.
- Consume los recursos definidos en `Resources/Styles/Colors.xaml` y `Resources/Styles/Styles.xaml`.

## Notas de diseño
Ver CONTEXT.md, sección *"Crash 'StaticResource not found for key Headline' al abrir la app — RESUELTO"*. El diseño original de `App` pedía `LoginPage` como parámetro del constructor (`public App(LoginPage loginPage)`), lo que forzaba a MAUI a construir `LoginPage` (y evaluar su XAML, que busca `StaticResource Headline`) **antes** de que `InitializeComponent()` de `App` cargara `Application.Resources`, provocando un crash al abrir la app. El fix fue inyectar `IServiceProvider` en el constructor y resolver la página (hoy `AppShell`) recién dentro de `CreateWindow(...)`, que corre después de `InitializeComponent()`. La misma nota documenta una segunda causa superpuesta no relacionada con esta clase: los builds Debug de .NET para Android usan *Fast Deployment*, y sin `<EmbedAssembliesIntoApk>true</EmbedAssembliesIntoApk>` (agregado al `.csproj`) el dispositivo podía seguir ejecutando código viejo pese a recompilar.
