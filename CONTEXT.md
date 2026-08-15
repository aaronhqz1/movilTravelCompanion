# Contexto del Proyecto: movilTravelCompanion

## Objetivo
Migrar la aplicación web **WeatherApp** (repo original) a una app **.NET MAUI** para **Android únicamente**, reutilizando el backend existente y reescribiendo el frontend como cliente MAUI (C#/XAML, MVVM).

## Repos
- **Origen (referencia histórica):** https://github.com/aaronhqz1/WeatherApp
- **Destino (proyecto MAUI en desarrollo):** https://github.com/aaronhqz1/movilTravelCompanion

## Backend: ubicación y edición — DECISIÓN TOMADA
- El backend Node/Express/SQLite fue **copiado físicamente a `backend/`** en la raíz de este repo (`movilTravelCompanion`), en vez de vivir solo en el repo `WeatherApp` separado.
- Motivo: ambos repos son propios (mismo dueño, no es una dependencia de un tercero), el backend no tiene desarrollo paralelo activo en `WeatherApp` (es código congelado de referencia), y tenerlo todo en un solo repo elimina el punto de fallo cruzado y reduce la fricción para herramientas de trabajo (Claude Code, búsquedas, etc.) que operan mejor con un solo contexto.
- **A diferencia de la decisión original, el backend copiado en `backend/` SÍ se puede editar si hace falta** (por ejemplo, para corregir un bug o ajustar un endpoint durante la migración). El repo `WeatherApp` original queda como referencia histórica de dónde vino, pero no es la fuente de la verdad operativa una vez copiado.
- `node_modules/` del backend está en `.gitignore` (no se commitea); correr `npm install` en `backend/` después de clonar.

## Stack del proyecto original
- **Frontend (a reemplazar por MAUI):** React 18.3 + Vite + Axios
- **Backend:** Node.js + Express 4.18 + SQLite3 + bcrypt (factor 12) + CORS
- **APIs externas consumidas:** Open-Meteo (clima) y Geocoding API de Open-Meteo (búsqueda de ciudades) — ambas gratuitas, sin API key

## Alcance de la migración
- El backend Node/Express/SQLite se reutiliza tal cual vino, con ediciones puntuales permitidas si son necesarias (ver sección "Backend: ubicación y edición" arriba). MAUI consumirá los mismos endpoints REST vía HTTP, igual que lo hacía React con Axios.
- Se reescribe **el cliente**: de componentes React a Views (XAML) + ViewModels (MVVM) en MAUI.
- Target exclusivo: **Android** (no se instala workload de iOS, no se requiere Mac).

## Alcance de v1 (MVP) — DECISIÓN TOMADA
**Dentro de alcance:** todo el flujo de navegación real encontrado en `App.jsx`:
`home → login → register → registration-success → travel-destination → dashboard`

**Actualización (sesión del 2026-08-09):** la recomendación de ropa vía OpenAI, originalmente marcada "fuera de alcance para v1", **se implementó en esta sesión** junto con un menú de navegación (Shell Flyout) y una pantalla de Preferencias nueva. Ver sección "Preferencias, menú Flyout y recomendación de vestimenta" más abajo para el detalle completo.

## Flujo real de navegación (según App.jsx, más completo que el README)
- `App.jsx` maneja el estado de vista actual (`currentView`) y el usuario logueado (`user`) a nivel raíz — es el equivalente a un "Shell"/navegación central en MAUI.
- Usa `localStorage` para persistir sesión entre recargas → en MAUI esto se reemplaza con `Preferences` o `SecureStorage` (API nativa de MAUI para guardar datos localmente en el dispositivo).
- Tras login, SIEMPRE pasa por `travel-destination` (elegir ciudad de viaje) antes de llegar al `dashboard`.

## Mapeo de componentes React → MAUI

| Componente React | Rol | Equivalente en MAUI |
|---|---|---|
| `Home.jsx` | Pantalla inicial, clima de ciudad aleatoria + búsqueda | `HomePage.xaml` + `HomeViewModel` |
| `Login.jsx` | Autenticación de usuario | `LoginPage.xaml` + `LoginViewModel` |
| `Register.jsx` | Registro de usuario nuevo | `RegisterPage.xaml` + `RegisterViewModel` |
| `RegistrationSuccess.jsx` | Confirmación post-registro | `RegistrationSuccessPage.xaml` |
| `TravelDestination.jsx` | Selección de ciudad de destino de viaje | `TravelDestinationPage.xaml` + `TravelDestinationViewModel` |
| `Dashboard.jsx` | Panel post-login: clima del destino + búsqueda + historial | `DashboardPage.xaml` + `DashboardViewModel` |
| `WeatherCard.jsx` | Componente reutilizable de UI para mostrar clima | `WeatherCardView` (ContentView reutilizable) |
| Recomendación de ropa (OpenAI) | Sugerencia de vestimenta según clima + estilo + sensibilidad al frío | Sección nueva en `DashboardPage` (implementada en la sesión del 2026-08-09) |
| — | Preferencias de usuario (nuevo, no existía en la web original) | `PreferencesPage.xaml` + `PreferencesViewModel` |

## Networking: backend local, cliente en emulador y dispositivo físico — DECISIÓN TOMADA
- Backend sigue corriendo local (`npm start` en la PC), sin desplegar a internet por ahora.
- PC y celular físico deben estar en la misma red WiFi.
- URL base de la API según entorno de ejecución:
  - Emulador Android → `http://10.0.2.2:3000`
  - Dispositivo físico → `http://<IP-LAN-de-la-PC>:3000` (obtener con `ipconfig`)
- Se implementará como configuración centralizada (una sola constante/servicio), no hardcodeada en cada Service.

## Endpoints del backend (contrato ya definido, no cambia)

**Autenticación**
```
POST /api/auth/register
Body: { username, password, homeCity }
Response: { message, userId }

POST /api/auth/login
Body: { username, password }
Response: { message, userId, username, homeCity, homeLatitude, homeLongitude }
```

**Clima**
```
GET /api/weather/random
Response: { city, latitude, longitude, temperature, humidity, wind_speed, weather_code }

GET /api/weather/search?city={cityName}
Response: { city, latitude, longitude, temperature, humidity, wind_speed, weather_code, hourly_forecast }

GET /api/weather/coordinates?lat={lat}&lon={lon}
Response: { temperature, humidity, wind_speed, weather_code, hourly_forecast }
```

**Historial**
```
POST /api/history
Body: { userId, city, latitude, longitude, temperature, humidity, wind_speed, weather_code }
Response: { message, id }

GET /api/history/:userId/recent
Response: [últimas 3 consultas]
```

**Configuración**
```
PUT /api/user/:userId/home
Body: { homeCity }
Response: { message, homeLatitude, homeLongitude }
```

**Preferencias de usuario** (nuevo, sesión 2026-08-09)
```
GET /api/user/:userId/preferences
Response: { defaultClothingStyle, coldSensitivity }
  (si el usuario nunca guardó preferencias, devuelve defaults sin crear fila:
  { defaultClothingStyle: "casual", coldSensitivity: "normal" })

PUT /api/user/:userId/preferences
Body: { defaultClothingStyle, coldSensitivity }
Response: { message, defaultClothingStyle, coldSensitivity }
  defaultClothingStyle: "casual" | "formal" | "deportivo"
  coldSensitivity: "friolento" | "normal" | "caluroso"
```

**Recomendación de vestimenta (OpenAI)** (ya existía en el backend, sin usar desde el cliente hasta esta sesión)
```
POST /api/ai/clothing-recommendation
Body: { city, temperature, weatherCode, humidity, windSpeed, clothingStyle, coldSensitivity }
  clothingStyle: "casual" | "formal" | "deportivo" (opcional, default "casual")
  coldSensitivity: "friolento" | "normal" | "caluroso" (opcional; ausente o null
    equivalen a "sin preferencia", el backend trata ambos casos igual)
Response: { success, city, weather, clothingStyle, coldSensitivity, recommendation, timestamp }
```

## Reglas de negocio a replicar en el cliente
- No se puede guardar la misma ciudad dos veces en el historial dentro de 24 horas (validar en backend, pero reflejar el estado en la UI).
- Solo se muestran las últimas 3 búsquedas del historial en el dashboard.
- La ciudad de origen es opcional al registrarse; puede configurarse después desde "Configuración".
- Usuario no autenticado ve clima de una ciudad aleatoria (de una lista de 50 ciudades predefinidas en el backend).
- Códigos de clima siguen el estándar WMO (0 = despejado, 1-3 = parcialmente nublado, 45/48 = niebla, 51-57 = llovizna, 61-67 = lluvia, 71-77 = nieve, 80-82 = chubascos, 85-86 = chubascos de nieve, 95-99 = tormenta).

## Estructura de proyecto objetivo

```
movilTravelCompanion.sln
├── movilTravelCompanion/              (proyecto MAUI, target: net10.0-android)
│   ├── Views/
│   │   ├── LoginPage.xaml
│   │   ├── RegisterPage.xaml
│   │   ├── RegistrationSuccessPage.xaml
│   │   ├── TravelDestinationPage.xaml
│   │   ├── DashboardPage.xaml
│   │   ├── HomePage.xaml
│   │   └── PreferencesPage.xaml       (nuevo, sesión 2026-08-09)
│   ├── ViewModels/
│   │   ├── LoginViewModel.cs
│   │   ├── RegisterViewModel.cs
│   │   ├── RegistrationSuccessViewModel.cs
│   │   ├── TravelDestinationViewModel.cs
│   │   ├── DashboardViewModel.cs
│   │   ├── HomeViewModel.cs
│   │   └── PreferencesViewModel.cs    (nuevo, sesión 2026-08-09)
│   ├── Services/
│   │   └── PreferencesSessionStore.cs  (implementación de ISessionStore con Preferences; vive acá y no en Core porque necesita el workload de plataforma)
│   ├── Controls/
│   │   └── WeatherCardView.xaml / .xaml.cs   (ContentView reutilizable: ciudad + temperatura + detalles opcionales; usado en HomePage, TravelDestinationPage y DashboardPage)
│   ├── ShellNavigationHelper.cs       (nuevo, sesión 2026-08-09: recorta el NavigationStack al navegar por el Flyout, ver "Notas técnicas")
│   ├── AppShell.xaml / AppShell.xaml.cs  (Flyout habilitado, sesión 2026-08-09)
│   ├── Platforms/
│   ├── Resources/
│   └── MauiProgram.cs
├── movilTravelCompanion.Core/         (librería .NET pura net10.0, lógica de negocio y servicios)
│   ├── Models/
│   │   ├── User.cs
│   │   ├── WeatherData.cs
│   │   ├── HourlyForecast.cs
│   │   ├── HistoryEntry.cs
│   │   ├── UserPreferences.cs         (nuevo, sesión 2026-08-09)
│   │   └── ClothingRecommendation.cs  (nuevo, sesión 2026-08-09)
│   ├── Services/
│   │   ├── IAuthService.cs / AuthService.cs
│   │   ├── IWeatherApiService.cs / WeatherApiService.cs
│   │   ├── IHistoryService.cs / HistoryService.cs
│   │   ├── IPreferencesService.cs / PreferencesService.cs   (nuevo, sesión 2026-08-09)
│   │   ├── IClothingService.cs / ClothingService.cs         (nuevo, sesión 2026-08-09)
│   │   └── ISessionStore.cs            (solo la interfaz; la implementación está en el proyecto MAUI)
│   └── movilTravelCompanion.Core.csproj
└── movilTravelCompanion.Tests/        (tests unitarios de Core — aún no creado)
```

Backend (`backend/src/`): nuevo `controllers/preferencesController.js` (GET/PUT preferencias), `config/database.js` con la tabla `user_preferences`, y `controllers/openaiController.js` extendido con `coldSensitivity` opcional (ver sección de Preferencias más abajo).

## Decisiones ya tomadas
- Arquitectura: **MVVM** con `CommunityToolkit.Mvvm`.
- Solo target Android — `.csproj` limitado a `net10.0-android`.
- Separación estricta: la UI (proyecto MAUI) no contiene lógica de negocio; toda lógica y llamadas HTTP viven en `movilTravelCompanion.Core`.
- `.gitignore` ya creado en el repo destino (estándar .NET MAUI, incluye exclusión de `.env`/`secrets.json` por si se maneja alguna clave sensible del lado cliente).

## Nivel del desarrollador
Sin experiencia previa en C#/OOP. Se requiere explicación de conceptos nuevos a medida que aparecen en el código (clases, interfaces, async/await, inyección de dependencias, data binding, etc.), comparando con JS/React cuando sea posible.

## Estado actual
- [x] Proyecto MAUI creado (`dotnet new maui -n movilTravelCompanion`)
- [x] `.gitignore` creado
- [x] Alcance de v1 definido (sin recomendación OpenAI)
- [x] Estrategia de networking definida (10.0.2.2 / IP LAN)
- [x] Crear proyecto `movilTravelCompanion.Core` (class library)
- [x] Definir modelos (`User`, `WeatherData`, `HourlyForecast`, `HistoryEntry`)
- [x] Implementar `WeatherApiService` (HttpClient consumiendo el backend Express)
- [x] Implementar `AuthService`
- [x] Configurar inyección de dependencias en `MauiProgram.cs`
- [x] Construir `LoginPage.xaml` + `LoginViewModel`
- [x] Construir `RegisterPage.xaml` + `RegisterViewModel` (con validación client-side de contraseñas)
- [x] Construir `RegistrationSuccessPage.xaml` + `RegistrationSuccessViewModel` (recibe `Username` por query parameter de Shell)
- [x] Configurar navegación real con Shell: `AppShell` como raíz de la app (ya no hay hack de mostrar `LoginPage` directo desde `App.xaml.cs`), rutas registradas para `RegisterPage` y `RegistrationSuccessPage`, navegación con `Shell.Current.GoToAsync(...)`
- [x] Confirmado corriendo en el emulador Pixel 10a API 37: la app abre sin crashear, Login → Register navega correctamente (probado con `adb` + capturas de pantalla)
- [x] Implementar `IHistoryService`/`HistoryService` en Core (POST /api/history, GET /api/history/:userId/recent)
- [x] Implementar manejo de sesión: `ISessionStore` (interfaz, en Core) + `PreferencesSessionStore` (implementación con `Preferences`, en el proyecto MAUI — Core no puede usar `Preferences` directo porque apunta a `net10.0` puro, sin el workload de plataforma)
- [x] Construir `TravelDestinationPage.xaml` + `TravelDestinationViewModel` (busca ciudad con `IWeatherApiService`, guarda destino elegido en la sesión, navega a Dashboard)
- [x] Construir `DashboardPage.xaml` + `DashboardViewModel` (clima del destino al entrar, búsqueda de otra ciudad que se guarda en historial, últimas 3 búsquedas en `CollectionView`, cerrar sesión)
- [x] `LoginViewModel` guarda la sesión al loguearse y navega a `TravelDestinationPage` (push relativo; ver fix de routing más abajo)
- [x] **Verificado en runtime con datos reales y con el backend corriendo**: flujo completo Register → Login → TravelDestination (buscar Madrid) → Dashboard (buscar París, se guarda en historial) → Cerrar sesión, probado a mano en el emulador Pixel 10a API 37 con `adb`. Requirió corregir dos bugs, ver detalle en "Notas técnicas / troubleshooting" abajo:
  - `android:usesCleartextTraffic="true"` faltante en `AndroidManifest.xml` (causaba "Connection failure" en toda llamada HTTP)
  - Navegación absoluta (`"//"`) rota hacia rutas globales de Shell (`TravelDestinationPage`, `DashboardPage`)
- [x] Construir `HomePage`/`HomeViewModel` (clima de ciudad aleatoria sin login + búsqueda de ciudad, botones "Iniciar Sesión"/"Crear cuenta"). `HomePage` pasó a ser el `ShellContent` inicial de `AppShell` (antes era `LoginPage`), siguiendo el flujo real de la app original (`home → login → ...`). `LoginPage` pasó a registrarse como ruta global igual que `RegisterPage`. Ver detalle del cambio de entry point y de las navegaciones que hubo que ajustar en "Notas técnicas / troubleshooting" abajo.
- [x] Creado `Controls/WeatherCardView.xaml` (+ `.xaml.cs`): `ContentView` reutilizable con `BindableProperty` para ciudad/temperatura/detalles, usado ahora en `HomePage`, `TravelDestinationPage` y `DashboardPage` (antes cada página duplicaba el mismo bloque de `Label`s).
- [x] Verificado en runtime: HomePage abre sin crashear mostrando clima de ciudad aleatoria real, la búsqueda de ciudad funciona, navegación Home↔Login y Home↔Register funciona en ambos sentidos (botón "atrás" del emulador vuelve a Home correctamente), y el flujo completo Login→TravelDestination→Dashboard→Cerrar sesión sigue funcionando tras el cambio de entry point (cerrar sesión ahora vuelve a Home, no a Login).

**Sesión 2026-08-09 — Preferencias, menú Flyout y recomendación de vestimenta:**
- [x] Backend: tabla `user_preferences` + `preferencesController.js` (GET/PUT), rutas registradas. Probado con `curl` (defaults sin fila previa, guardar, releer, rechazar valor inválido).
- [x] Backend: `openaiController.js` acepta `coldSensitivity` opcional, ajusta el prompt para compensar (ej. "friolento" → recomienda capas extra aunque la temperatura no lo amerite). Estilos de vestimenta unificados a español (`casual`/`formal`/`deportivo`) en ambos controllers, reemplazando el `athletic` en inglés que tenía `openaiController.js` originalmente.
- [x] Fix: `coldSensitivity` explícito como `null` (lo que manda `System.Text.Json` del lado del cliente al serializar una propiedad ausente) se trata igual que ausente en la validación del backend.
- [x] Core: `UserPreferences.cs`, `ClothingRecommendation.cs`, `IPreferencesService`/`PreferencesService`, `IClothingService`/`ClothingService` (mismo patrón HTTP que los servicios existentes), registrados en `MauiProgram.cs`.
- [x] Shell Flyout habilitado (`FlyoutBehavior="Flyout"`) con 4 `MenuItem` (Viaje Actual/Cambiar Destino/Preferencias/Cerrar Sesión) — **no** `FlyoutItem` real, ver "Notas técnicas" para el motivo. `Shell.NavBarIsVisible="True"` solo en Dashboard/TravelDestination/Preferencias; `Shell.FlyoutBehavior="Disabled"` explícito en Home/Login/Register/RegistrationSuccess (Flyout solo accesible con sesión iniciada).
- [x] `PreferencesPage.xaml` + `PreferencesViewModel`: lee/guarda contra el backend real, `RadioButtonGroup.SelectedValue` bindea directo el string elegido sin necesitar convertidor.
- [x] `DashboardPage`: sección "Sugerencia de vestimenta" (RadioButton x3 + botón + resultado), consumiendo `IClothingService` con el clima del destino actual. `DashboardViewModel.ApplyDestinationWeather` ahora retiene el `WeatherData` completo (antes solo extraía temperatura/humedad/viento a strings, se perdía el `weatherCode` necesario para el endpoint de OpenAI).
- [x] `DashboardViewModel.LoadAsync` precarga `SelectedClothingStyle` y la sensibilidad al frío desde Preferencias (fallback silencioso a los defaults si falla, no bloquea el resto del Dashboard).
- [x] Botón "Cerrar sesión" y su `Command` movidos de `DashboardPage`/`DashboardViewModel` al `MenuItem` del Flyout (`AppShell.xaml.cs`), sin duplicar lógica.
- [x] **Verificado en runtime con backend y OpenAI reales**: Tokio 31.3°C + Deportivo devolvió ropa liviana transpirable; Lima 19.8°C + Deportivo + sensibilidad "friolento" (precargada desde Preferencias) devolvió capas extra pese a la temperatura templada, confirmando que ambos valores llegan correctamente al prompt.
- [x] Fix de navegación: `ShellNavigationHelper.TrimNavigationStack()` evita que el `NavigationStack` crezca sin límite al navegar repetidamente por el Flyout entre Dashboard/TravelDestination (ver "Notas técnicas" — el riesgo ya documentado resultó peor de lo anticipado: 6 pantallas apiladas tras un solo ciclo de login + un uso del menú).
- [ ] Probar en dispositivo físico Android

## Notas técnicas / troubleshooting

### Crash "StaticResource not found for key Headline" al abrir la app — RESUELTO
Tenía dos causas superpuestas:
1. **Orden de inicialización de DI**: `App.xaml.cs` originalmente pedía `LoginPage` como parámetro del constructor (`public App(LoginPage loginPage)`), lo que forzaba a construir `LoginPage` (y su XAML, que busca `StaticResource Headline`) *antes* de que `InitializeComponent()` de `App` cargara `Application.Resources`. Fix: inyectar `IServiceProvider` en el constructor y resolver la página recién dentro de `CreateWindow(...)`, que corre después de `InitializeComponent()`.
2. **Assemblies viejos en el dispositivo (la causa que hacía que el fix de arriba "no se viera aplicado")**: los builds Debug de .NET para Android usan *Fast Deployment*: el `.apk` no lleva los `.dll` embebidos, se esperan empujar aparte al dispositivo (vía `dotnet build -t:Run` o el deploy del IDE) a una carpeta `.__override__` en los datos privados de la app. Como el flujo de trabajo era `dotnet build` + `adb install` manual, esa carpeta nunca se actualizaba y la app seguía cargando código viejo (o, tras un `adb uninstall`, fallaba directamente con `No assemblies found ... Assuming this is part of Fast Deployment`). Fix: se agregó `<EmbedAssembliesIntoApk>true</EmbedAssembliesIntoApk>` condicionado a `Configuration=Debug` en `movilTravelCompanion.csproj`, para que el `.apk` de Debug sea autocontenido y `adb install` alcance por sí solo (el `.apk` pasó de ~12MB a ~85MB, confirmando que ahora sí embebe los ensamblados).
- **Importante para el futuro**: si se vuelve a ver comportamiento "viejo" pese a haber compilado de nuevo, sospechar primero de la carpeta `.__override__` desactualizada — un `adb uninstall <package>` antes de reinstalar la limpia (borra los datos privados de la app).

### F5 en VS Code — diagnóstico parcial, no resuelto del todo
- `.vs/ProjectSettings.json` tiene `"CurrentProjectSetting": null` — el "Startup Project" nunca quedó persistido pese a haber usado "Set as Startup Project" en el Solution Explorer de C# Dev Kit. Esto es consistente con el error "Debugging canceled: startup project not found": versiones recientes de la extensión `ms-dotnettools.dotnet-maui` parecen depender del proyecto de inicio que gestiona C# Dev Kit (visible en Solution Explorer) más que de la propiedad `"project"` de `launch.json` (que ya está bien configurada).
- **Se confirmó que el toolchain de build/deploy en sí funciona**: `dotnet build movilTravelCompanion/movilTravelCompanion.csproj -t:Run -f:net10.0-android` corrió limpio (0 errores) y desplegó+abrió la app en el emulador — contradice la nota anterior de que `-t:Run` fallaba con MSB3072/MSB6011 (probablemente asociado al mismo problema de ensamblados/build incremental corrupto que ya se resolvió arriba). O sea: el bloqueo de F5 es puntualmente de integración VS Code ↔ C# Dev Kit, no del build.
- JDK detectado: Microsoft OpenJDK 17.0.20 (`C:\Program Files\Microsoft\jdk-17.0.20.8-hotspot`) — es el JDK recomendado para el workload Android de .NET, el warning de versión visto en el panel de output probablemente no es la causa raíz.
- **Pendiente (requiere interacción manual en la UI de VS Code, no se puede automatizar desde la terminal)**: en el panel Solution Explorer (ícono de C# Dev Kit), clic derecho sobre `movilTravelCompanion` (NO `.Core`) → "Set as Startup Project", confirmar que quede marcado en negrita/con ícono distintivo, y si no persiste, probar "Developer: Reload Window" después de marcarlo y recién ahí F5.
- Mientras tanto, el flujo `dotnet build ... -f:net10.0-android` + `adb install -r <Signed.apk>` (o incluso `-t:Run` directo, que ahora sí funciona) sigue siendo válido.

### "Connection failure" en Register/Login pese a que `ApiConfig.BaseUrl` apunta bien a `10.0.2.2:3000` — RESUELTO
- Causa: no existía `network_security_config` ni `android:usesCleartextTraffic` en `AndroidManifest.xml`. Desde Android 9 (API 28) el tráfico HTTP sin cifrar se bloquea por defecto a nivel de SO, y el proyecto apunta a `target_sdk_version=36` — muy por encima de ese límite —, así que la conexión se descartaba antes de llegar al backend.
- Fix: agregado `android:usesCleartextTraffic="true"` al `<application>` de `movilTravelCompanion/Platforms/Android/AndroidManifest.xml`.
- Nota de la sesión: al aplicar el fix quedó un `<application>` duplicado en el manifest (dos elementos `<application>`, uno sin el atributo y otro con él) — XML inválido que hubiera roto el build. Se corrigió dejando un solo `<application>` con `usesCleartextTraffic="true"`.
- Verificado en runtime: antes del fix, Register tiraba "Connection failure" a nivel de SO; después del fix, el POST llegó al backend real (primero devolvió 400 por validación de contraseña, después 200 con "¡Cuenta creada!").

### Navegación con ruta absoluta ("//") rota hacia `TravelDestinationPage` y `DashboardPage` — RESUELTO
- Síntoma: tras un login exitoso contra el backend (sin error de conexión ni de credenciales), la app quedaba trabada en `LoginPage` con el error en pantalla: *"Global routes currently cannot be the only page on the stack, so absolute routing to global routes is not supported."*
- Causa: en `AppShell.xaml`, el único `ShellContent` real es `LoginPage`; `RegisterPage`, `RegistrationSuccessPage`, `TravelDestinationPage` y `DashboardPage` se registran como rutas "sueltas" (`Routing.RegisterRoute` en `AppShell.xaml.cs`), no como `ShellContent`. MAUI Shell no permite navegación absoluta (`"//" + ruta`) hacia una ruta de ese tipo cuando quedaría como única página en la pila.
- Fix: en `LoginViewModel.cs` (línea ~48) y `TravelDestinationViewModel.cs` (línea ~94), se cambió `Shell.Current.GoToAsync("//" + nameof(Pagina))` por `Shell.Current.GoToAsync(nameof(Pagina))` (push relativo en vez de ruta absoluta). Efecto secundario que en su momento quedó aceptado para v1 (el botón "atrás" podía volver a una pantalla intermedia) — **resuelto en la sesión 2026-08-09** por `ShellNavigationHelper.TrimNavigationStack()`, ver más abajo.
- En esta sesión, los `GoToAsync("//" + nameof(LoginPage))` usados en logout todavía apuntaban a `LoginPage` porque en ese momento **sí** era el `ShellContent` real. Ver la nota siguiente: al mover el `ShellContent` a `HomePage`, estos se volvieron a romper y hubo que corregirlos de nuevo (ahora apuntan a `HomePage`).
- Verificado en runtime (en esta sesión, antes del cambio de entry point): flujo completo Login → TravelDestination (buscar Madrid, confirmar) → Dashboard (ver clima de Madrid, buscar París, aparece en historial) → Cerrar sesión (en ese momento volvía a Login), probado a mano con `adb` en el emulador Pixel 10a API 37.

### Cambio de entry point: `HomePage` pasa a ser el `ShellContent` inicial (antes era `LoginPage`)
- Motivo: al construir `HomePage` (clima de ciudad aleatoria sin login), se decidió seguir el flujo real de la app original (`App.jsx`: `home → login → register → ... → dashboard`, ver sección "Alcance de v1" arriba) en vez de dejar `LoginPage` como pantalla de entrada.
- Cambios en `AppShell.xaml`: el único `ShellContent` ahora es `HomePage` (antes era `LoginPage`).
- Cambios en `AppShell.xaml.cs`: se agregó `Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage))` — `LoginPage` pasa a ser una ruta global más, igual que `RegisterPage`.
- Efecto colateral: esto **volvió a romper** los `GoToAsync("//" + nameof(LoginPage))` usados para logout y para los guards de "sesión perdida" (mismo bug de "Global routes currently cannot be the only page on the stack" que ya se había resuelto para `TravelDestinationPage`/`DashboardPage`), porque `LoginPage` dejó de ser el `ShellContent` real. Se corrigieron cambiándolos a `"//" + nameof(HomePage)` (que sí es válido, es el `ShellContent` real) en:
  - `DashboardViewModel.cs`: guard de sesión perdida y `LogoutAsync()` (este último se movió a `AppShell.xaml.cs` en la sesión 2026-08-09, ver más abajo — ya no existe en `DashboardViewModel`)
  - `TravelDestinationViewModel.cs`: guard de sesión perdida
  - `RegistrationSuccessViewModel.cs`: `ContinueAsync()` (este caso es navegación hacia adelante, no reset de stack, así que se cambió a push relativo `GoToAsync(nameof(LoginPage))`, no a `"//HomePage"`)
- Verificado en runtime: Home abre sin crashear con clima de ciudad aleatoria real, búsqueda funciona, Home↔Login y Home↔Register navegan y vuelven bien con el botón "atrás" del emulador, y el flujo Login→TravelDestination→Dashboard→Cerrar sesión sigue funcionando de punta a punta (cerrar sesión ahora deja al usuario en Home, no en Login).

### FlyoutItem vs MenuItem para el menú del Flyout — DECISIÓN TOMADA (sesión 2026-08-09)
- La especificación original de esta sesión pedía `FlyoutItem` para las 3 páginas navegables (Viaje Actual/Cambiar Destino/Preferencias) y `MenuItem` solo para Cerrar Sesión.
- Problema real, no de estilo: un `FlyoutItem` de MAUI Shell no es "un ítem de menú que navega a una página" — es una *sección* completa con su propio `ShellContent` y su propia pila de navegación, y la navegación entre `FlyoutItem`s usa rutas absolutas (`"//"`). `DashboardPage` y `TravelDestinationPage` ya estaban registradas como rutas sueltas (`Routing.RegisterRoute`) para navegación por *push*, usadas por `LoginViewModel`/`TravelDestinationViewModel`. Convertirlas también en `FlyoutItem` hubiera creado dos formas conflictivas de llegar a la misma página.
- Decisión: los 4 ítems del Flyout son `MenuItem` con `Command` que llaman `Shell.Current.GoToAsync(...)`, mismo patrón de push que ya usaba toda la app. Se pierde el agrupamiento visual nativo de Shell (FlyoutItems arriba / MenuItems abajo con separador), pero no había forma de tener FlyoutItem real sin refactorizar la navegación existente.

### Icono de hamburguesa del Flyout no aparece en páginas alcanzadas por push — RESUELTO
- Síntoma: con `FlyoutBehavior="Flyout"` y `Shell.NavBarIsVisible="True"` en Dashboard/TravelDestination/Preferencias, la barra superior mostraba la flecha de "atrás" pero nunca el ícono de hamburguesa para abrir el menú.
- Causa: el ícono de hamburguesa automático de Shell solo aparece en la página raíz de una sección (sin pila de navegación detrás). Como estas 3 páginas siempre se alcanzan por *push* (nunca son la raíz), Shell prioriza mostrar la flecha de "atrás". El gesto de swipe desde el borde tampoco sirve de alternativa: compite con el gesto nativo de Android para volver atrás y en la práctica dispara ese gesto en vez de abrir el Flyout (confirmado en el emulador).
- Fix: `ToolbarItem` explícito ("Menú") en las 3 páginas, con `Clicked` en el code-behind que hace `Shell.Current.FlyoutIsPresented = true`.

### `NavigationStack` crece sin límite al navegar por el Flyout — RESUELTO
- Riesgo ya anticipado al planificar esta sesión (agregar "Cambiar Destino" como ítem de menú alcanzable *desde* Dashboard, algo que antes solo pasaba una vez durante el registro). Verificado explícitamente y resultó **peor de lo esperado**: no es solo "Dashboard duplicado una vez" — cada ciclo Dashboard → menú → Cambiar Destino → confirmar apila una instancia nueva de `TravelDestinationPage` y `DashboardPage` sin sacar las anteriores. Un solo login + un uso del menú ya dejaba **6 pantallas apiladas** (`Home ← Login ← TravelDestination ← Dashboard ← TravelDestination ← Dashboard`), y crecía sin límite con cada uso repetido.
- Fix: `ShellNavigationHelper.TrimNavigationStack()` (archivo nuevo `movilTravelCompanion/ShellNavigationHelper.cs`) usa `Navigation.RemovePage()` (técnica estándar de MAUI Shell) para sacar del `NavigationStack` todo lo que no sea la raíz del Shell (`HomePage`) ni la página recién alcanzada. Se llama al final de los 3 comandos de navegación del Flyout (`AppShell.xaml.cs`) y después del push a `DashboardPage` en `TravelDestinationViewModel.ConfirmAsync`.
- Efecto secundario (mejora, no regresión): el botón "atrás" desde Dashboard/TravelDestination/Preferencias ahora vuelve directo a `HomePage` en vez de volver a reproducir Login/TravelDestination — esto además resuelve el efecto secundario que había quedado aceptado para v1 en la nota "Navegación con ruta absoluta" más arriba.
- Verificado en runtime: dos ciclos completos de "Cambiar Destino" (Berlín, luego Cairo) seguidos de un solo toque de "atrás" siguen llevando directo a Home; la pila no crece con el uso repetido.

## Mejoras futuras / backlog
No implementadas en esta sesión, quedan documentadas para retomar más adelante:
- **Historial de viajes anteriores** ("Crear Viaje" / "Viajes Anteriores"): implica una tabla nueva en el backend. Descartado explícitamente para la sesión 2026-08-09 — el destino de viaje sigue siendo un único valor activo que se pisa al cambiarlo (`PreferencesSessionStore`, solo en el dispositivo).
- **Duración del viaje actual**: agregar fecha de inicio/fin al destino elegido.
- **Historial de consultas relacionado a su viaje**: hoy el historial (`weather_history`) no distingue a qué viaje/destino perteneció cada consulta.
- **GPS en HomePage**: usar la ubicación real del dispositivo para determinar automáticamente la ciudad del usuario, en vez de mostrar siempre una ciudad aleatoria a los no logueados.

## Siguiente paso concreto
1. Terminar de confirmar F5 en VS Code siguiendo los pasos manuales documentados arriba (clic derecho → "Set as Startup Project" en Solution Explorer de C# Dev Kit).
2. Probar en dispositivo físico Android (nunca se hizo, solo emulador).
3. Evaluar los ítems del backlog de arriba para una futura sesión.
