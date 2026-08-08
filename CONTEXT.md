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
**Fuera de alcance para v1:** recomendación de ropa vía OpenAI (requiere API key paga, se evalúa como fase 2).
**Dentro de alcance:** todo el flujo de navegación real encontrado en `App.jsx`:
`home → login → register → registration-success → travel-destination → dashboard`

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
| ~~Recomendación de ropa (OpenAI)~~ | Fuera de alcance v1 | — |

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
│   │   └── HomePage.xaml               (pendiente)
│   ├── ViewModels/
│   │   ├── LoginViewModel.cs
│   │   ├── RegisterViewModel.cs
│   │   ├── RegistrationSuccessViewModel.cs
│   │   ├── TravelDestinationViewModel.cs
│   │   ├── DashboardViewModel.cs
│   │   └── HomeViewModel.cs            (pendiente)
│   ├── Services/
│   │   └── PreferencesSessionStore.cs  (implementación de ISessionStore con Preferences; vive acá y no en Core porque necesita el workload de plataforma)
│   ├── Controls/
│   │   └── WeatherCardView.xaml        (pendiente)
│   ├── AppShell.xaml / AppShell.xaml.cs
│   ├── Platforms/
│   ├── Resources/
│   └── MauiProgram.cs
├── movilTravelCompanion.Core/         (librería .NET pura net10.0, lógica de negocio y servicios)
│   ├── Models/
│   │   ├── User.cs
│   │   ├── WeatherData.cs
│   │   ├── HourlyForecast.cs
│   │   └── HistoryEntry.cs
│   ├── Services/
│   │   ├── IAuthService.cs / AuthService.cs
│   │   ├── IWeatherApiService.cs / WeatherApiService.cs
│   │   ├── IHistoryService.cs / HistoryService.cs
│   │   └── ISessionStore.cs            (solo la interfaz; la implementación está en el proyecto MAUI)
│   └── movilTravelCompanion.Core.csproj
└── movilTravelCompanion.Tests/        (tests unitarios de Core — aún no creado)
```

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
- [x] `LoginViewModel` ahora guarda la sesión al loguearse y navega a `TravelDestinationPage` (ruta absoluta `"//..."`, resetea la pila de Shell)
- [ ] Construir `HomePage`/`HomeViewModel` (clima de ciudad aleatoria sin login — no forma parte del flujo login→dashboard, quedó afuera de esta tanda)
- [ ] **Sin verificar en runtime con datos reales**: el flujo completo Login → TravelDestination → Dashboard nunca se probó con el backend Express corriendo (no estaba levantado en esta sesión). Falta confirmar con `npm start` en el repo `WeatherApp` corriendo en la PC.
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

## Siguiente paso concreto
1. Levantar el backend Express (`npm start` en el repo `WeatherApp`, en la PC) y probar el flujo completo de punta a punta en el emulador: Login → TravelDestination → Dashboard (buscar ciudad, ver que se guarde en el historial, cerrar sesión). Es el primer test con datos reales de todo lo construido en esta sesión.
2. Construir `HomePage`/`HomeViewModel` (clima de ciudad aleatoria, pantalla para usuario no logueado).
3. Terminar de confirmar F5 en VS Code siguiendo los pasos manuales documentados arriba (clic derecho → "Set as Startup Project" en Solution Explorer de C# Dev Kit).