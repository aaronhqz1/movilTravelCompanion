# movilTravelCompanion

Breve descripción: app Android en .NET MAUI para consultar el clima,
migrada desde el proyecto web WeatherApp (React + Node/Express).

## Estado del proyecto
En desarrollo activo — ver CONTEXT.md para el detalle técnico y checklist actual.

## Requisitos
- .NET 10 SDK
- Workload maui-android (dotnet workload install maui-android)
- Android SDK + JDK 17 (ver sección de instalación)
- Backend corriendo (ver repo original WeatherApp)

## Instalación

```powershell
# 1. Cliente MAUI
dotnet workload install maui-android
dotnet restore movilTravelCompanion.sln

# 2. Backend (Node/Express, copiado en backend/)
cd backend
npm install
```

## Cómo correr la app

**1. Levantar el backend** (dejalo corriendo en su propia terminal):
```powershell
cd backend
npm start
```
Confirmá que loguea `Servidor corriendo en http://localhost:3000`.

**2. Compilar e instalar el cliente** (mientras el backend sigue corriendo):
```powershell
dotnet build movilTravelCompanion/movilTravelCompanion.csproj -f:net10.0-android
adb install -r movilTravelCompanion/bin/Debug/net10.0-android/com.companyname.moviltravelcompanion-Signed.apk
```

**3. URL base según entorno:**
- Emulador Android → `http://10.0.2.2:3000` (ya configurado por defecto)
- Dispositivo físico → `http://<IP-LAN-de-la-PC>:3000`, con la PC y el celular en la misma red WiFi (ver `ApiConfig` en `movilTravelCompanion.Core/Configuration`)

## Arquitectura
MVVM con movilTravelCompanion (UI) y movilTravelCompanion.Core (lógica
de negocio y servicios HTTP), consumiendo el backend Node/Express original.

## Autor
Aaron Henriquez Leiva
