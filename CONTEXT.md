# Contexto del Proyecto: movilTravelCompanion

## Objetivo
Migrar la aplicación web **WeatherApp** (repo original) a una app **.NET MAUI** para **Android únicamente**, reutilizando el backend existente y reescribiendo el frontend como cliente MAUI (C#/XAML, MVVM).

## Repos
- **Origen (referencia, no se toca el backend):** https://github.com/aaronhqz1/WeatherApp
- **Destino (proyecto MAUI en desarrollo):** https://github.com/aaronhqz1/movilTravelCompanion

## Stack del proyecto original
- **Frontend (a reemplazar por MAUI):** React 18.3 + Vite + Axios
- **Backend (se mantiene tal cual, corre como API REST):** Node.js + Express 4.18 + SQLite3 + bcrypt (factor 12) + CORS
- **APIs externas consumidas:** Open-Meteo (clima) y Geocoding API de Open-Meteo (búsqueda de ciudades) — ambas gratuitas, sin API key

## Alcance de la migración
- El backend Node/Express/SQLite **no se reescribe**. MAUI consumirá los mismos endpoints REST vía HTTP, igual que lo hacía React con Axios.
- Se reescribe **solo el cliente**: de componentes React a Views (XAML) + ViewModels (MVVM) en MAUI.
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
│   │   ├── HomePage.xaml
│   │   ├── LoginPage.xaml
│   │   ├── RegisterPage.xaml
│   │   └── DashboardPage.xaml
│   ├── ViewModels/
│   │   ├── HomeViewModel.cs
│   │   ├── LoginViewModel.cs
│   │   ├── RegisterViewModel.cs
│   │   └── DashboardViewModel.cs
│   ├── Controls/
│   │   └── WeatherCardView.xaml
│   ├── Platforms/
│   ├── Resources/
│   └── MauiProgram.cs
├── movilTravelCompanion.Core/         (librería .NET, lógica de negocio y servicios)
│   ├── Models/
│   │   ├── User.cs
│   │   ├── WeatherData.cs
│   │   ├── HourlyForecast.cs
│   │   └── HistoryEntry.cs
│   ├── Services/
│   │   ├── IAuthService.cs / AuthService.cs
│   │   ├── IWeatherApiService.cs / WeatherApiService.cs
│   │   └── IHistoryService.cs / HistoryService.cs
│   └── movilTravelCompanion.Core.csproj
└── movilTravelCompanion.Tests/        (tests unitarios de Core)
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
- [ ] Implementar manejo de sesión con `Preferences`/`SecureStorage`
- [ ] Construir Views + ViewModels (Home, Login, Register, RegistrationSuccess, TravelDestination, Dashboard)
- [ ] Configurar navegación (Shell) entre páginas
- [ ] Probar en emulador Android
- [ ] Probar en dispositivo físico Android

## Siguiente paso concreto
Crear las Views + ViewModels en el proyecto MAUI, empezando por `LoginPage.xaml` + `LoginViewModel.cs` (consumiendo `IAuthService.LoginAsync`, ya registrado en `MauiProgram.cs`). Es el primer punto donde se junta MVVM con data binding real: la Vista (XAML) se enlaza a propiedades del ViewModel, y el ViewModel llama a los servicios de `movilTravelCompanion.Core`.