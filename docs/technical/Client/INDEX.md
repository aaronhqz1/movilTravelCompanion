# Índice técnico — Cliente (movilTravelCompanion, proyecto MAUI)

Documentación técnica de todas las clases custom del proyecto cliente `movilTravelCompanion` (app .NET MAUI para Android). Para los servicios/modelos compartidos de `movilTravelCompanion.Core`, ver `docs/technical/Core/`. Para el backend Node/Express, ver `docs/technical/Backend/`.

| Clase | Propósito | Link |
|---|---|---|
| `App` | Raíz de `Application`: carga recursos globales y construye la ventana resolviendo `AppShell` por DI. | [App.md](./App.md) |
| `AppShell` | Shell raíz de navegación: `HomePage` como único `ShellContent`, rutas globales registradas, y menú Flyout (Viaje Actual / Cambiar Destino / Preferencias / Cerrar Sesión). | [AppShell.md](./AppShell.md) |
| `MainPage` | Página de plantilla del scaffolding de `dotnet new maui`, sin uso real en la app (código muerto). | [MainPage.md](./MainPage.md) |
| `MauiProgram` | Configuración de arranque: registro de fuentes, `HttpClient`, servicios de `Core` y todas las páginas/ViewModels en el contenedor de DI. | [MauiProgram.md](./MauiProgram.md) |
| `ShellNavigationHelper` | Helper estático que recorta el `NavigationStack` de Shell tras navegar por el Flyout, evitando crecimiento sin límite. | [ShellNavigationHelper.md](./ShellNavigationHelper.md) |
| `WeatherCardView` | `ContentView` reutilizable (ciudad + temperatura + detalles opcionales), usado en Home/TravelDestination/Dashboard. | [WeatherCardView.md](./WeatherCardView.md) |
| `PreferencesSessionStore` | Implementación de `ISessionStore` (Core) con `Preferences` de MAUI — equivalente a `localStorage`. | [PreferencesSessionStore.md](./PreferencesSessionStore.md) |
| `HomeViewModel` (+ `HomePage`) | Pantalla de entrada: clima de ciudad aleatoria sin login, búsqueda, y accesos a Login/Registro. | [HomeViewModel.md](./HomeViewModel.md) |
| `LoginViewModel` (+ `LoginPage`) | Autenticación: login contra el backend, guarda sesión, navega siempre a `TravelDestinationPage`. | [LoginViewModel.md](./LoginViewModel.md) |
| `RegisterViewModel` (+ `RegisterPage`) | Registro de usuario nuevo con validación client-side de contraseñas. | [RegisterViewModel.md](./RegisterViewModel.md) |
| `RegistrationSuccessViewModel` (+ `RegistrationSuccessPage`) | Confirmación post-registro; recibe `Username` por query parameter de Shell. | [RegistrationSuccessViewModel.md](./RegistrationSuccessViewModel.md) |
| `TravelDestinationViewModel` (+ `TravelDestinationPage`) | Selección de ciudad de destino de viaje; guarda destino en la sesión y navega al Dashboard. | [TravelDestinationViewModel.md](./TravelDestinationViewModel.md) |
| `DashboardViewModel` (+ `DashboardPage`) | Panel principal: clima del destino, recomendación de vestimenta (OpenAI), búsqueda con historial. | [DashboardViewModel.md](./DashboardViewModel.md) |
| `PreferencesViewModel` (+ `PreferencesPage`) | Preferencias de usuario (estilo de vestimenta, sensibilidad al frío/calor), leídas/guardadas contra el backend. | [PreferencesViewModel.md](./PreferencesViewModel.md) |

## Clases no documentadas: `Platforms/`

Las clases dentro de `movilTravelCompanion/Platforms/{Android,iOS,MacCatalyst,Windows}/` (`MainActivity`, `MainApplication`, `AppDelegate`, `SceneDelegate`, `App.xaml.cs` de Windows, etc.) son boilerplate estándar generado por el template de `dotnet new maui`, sin lógica custom del proyecto. Dado que el target exclusivo de la app es Android (ver CONTEXT.md, "Alcance de la migración"), ni siquiera el resto de las carpetas de plataforma participan del build real. No se documentan en detalle en este índice.
