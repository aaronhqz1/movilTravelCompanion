# movilTravelCompanion.MainPage

## Ubicación
`movilTravelCompanion/MainPage.xaml`, `movilTravelCompanion/MainPage.xaml.cs`

## Propósito
Página de plantilla generada por defecto por `dotnet new maui` (el clásico contador "Click me"). No forma parte del flujo de navegación real de la app: no está declarada como `ShellContent` en `AppShell.xaml` ni registrada como ruta, por lo que es código muerto que quedó del scaffolding inicial del proyecto.

## Tipo
Page (code-behind + XAML) — plantilla sin uso, sin ViewModel ni MVVM.

## Responsabilidades
- Ninguna dentro del flujo real de la app. Tal como está, solo demuestra el patrón de contador de clicks del template estándar de MAUI (`OnCounterClicked` incrementa un campo `count` y actualiza el texto del botón).

## Dependencias
Ninguna — no inyecta servicios, no usa MVVM Toolkit.

## Miembros públicos clave

### Propiedades observables / Bindable
Ninguna (no usa `[ObservableProperty]` ni `INotifyPropertyChanged`; `count` es un campo privado plano).

### Comandos / Métodos
| Nombre | Descripción |
|---|---|
| `MainPage()` | Constructor, solo llama a `InitializeComponent()`. |
| `OnCounterClicked(object?, EventArgs)` | Handler de evento `Clicked` del botón `CounterBtn`: incrementa `count` y actualiza el texto (`"Clicked N times"`), con anuncio de accesibilidad vía `SemanticScreenReader.Announce`. |

## Flujo y lógica relevante
No participa en la navegación de Shell: `AppShell.xaml` declara `HomePage` como único `ShellContent` y no registra ninguna ruta hacia `MainPage`. No hay forma de llegar a esta página navegando la app normalmente.

## Data binding (si es Page/ViewModel)
No usa data binding — usa `x:Name="CounterBtn"` y manipulación directa del control en el code-behind (patrón code-behind clásico, no MVVM), a diferencia del resto de las páginas del proyecto.

## Relaciones
Ninguna con el resto de la app — no es referenciada desde `AppShell`, `MauiProgram.cs` no la registra en el contenedor de DI, y ninguna otra clase navega hacia ella.

## Notas de diseño
Se documenta por completitud (es una clase custom presente en el proyecto), pero es residuo del scaffolding de `dotnet new maui -n movilTravelCompanion` (ver CONTEXT.md, "Estado actual"). Candidato a eliminarse en una futura limpieza; no se referencia en ningún lado del código de negocio.
