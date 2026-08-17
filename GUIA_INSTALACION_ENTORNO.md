# Guía de Instalación del Entorno — movilTravelCompanion

Instrucciones para dejar tu computadora lista para trabajar en el proyecto: cliente **.NET MAUI (Android)** + backend **Node/Express**. Seguí los pasos en orden — cada uno depende del anterior.

**Sistema operativo:** esta guía asume **Windows**, que es lo que usa el equipo actualmente. Si trabajás desde Mac o Linux, avisale a Aaron antes de empezar (hay pasos que cambian).

---

## 0. Qué vas a instalar (resumen)

| # | Programa | Para qué |
|---|---|---|
| 1 | Git | Descargar y subir código del repositorio |
| 2 | Node.js | Correr el backend (Express) |
| 3 | .NET SDK | Compilar el cliente MAUI |
| 4 | Workload de Android para MAUI | Le agrega al SDK de .NET la capacidad de compilar para Android |
| 5 | Android Studio | SDK de Android + emulador para probar la app |
| 6 | Visual Studio Code + extensiones | Editor donde vamos a trabajar el código |

Calculá **entre 1 y 2 horas** para la instalación completa (la mayoría es tiempo de descarga).

---

## 1. Git

Descargá e instalá desde: **https://git-scm.com/download/win**

Durante la instalación, dejá todas las opciones por defecto (no hace falta tocar nada) salvo que sepas específicamente qué estás cambiando.

**Verificá que quedó instalado**, abriendo PowerShell y corriendo:
```powershell
git --version
```
Debería mostrar algo como `git version 2.4x.x`.

---

## 2. Node.js

Descargá la versión **LTS** (la que dice "Recommended for most users") desde: **https://nodejs.org**

Instalá con las opciones por defecto.

**Verificá:**
```powershell
node --version
npm --version
```
Ambos comandos deberían devolver un número de versión sin error.

---

## 3. .NET SDK

Este proyecto usa **.NET 10**. Descargalo desde: **https://dotnet.microsoft.com/download/dotnet/10.0**

Elegí el instalador del **SDK** (no el "Runtime" — el SDK incluye todo lo necesario para compilar, el Runtime solo para ejecutar).

**Verificá:**
```powershell
dotnet --version
```
Debería devolver una versión que empiece con `10.`.

---

## 4. Workload de Android para MAUI

Con el SDK de .NET ya instalado, agregale la capacidad de compilar apps Android. Corré en PowerShell:

```powershell
dotnet workload install maui-android
```

Esto puede tardar varios minutos — descarga varios componentes adicionales.

**Verificá** que quedó instalado:
```powershell
dotnet workload list
```
Deberías ver `maui-android` en la lista.

---

## 5. Android Studio (SDK + Emulador)

Aunque vamos a escribir el código en VS Code, necesitamos Android Studio para el **SDK de Android** y el **emulador** (la "computadora virtual" que simula un celular).

1. Descargalo desde: **https://developer.android.com/studio**
2. Instalalo con las opciones por defecto. Va a instalar también un JDK (Java) que Android necesita — no hace falta instalar Java por separado.
3. Abrí Android Studio. La primera vez te va a guiar por un asistente de configuración ("Setup Wizard") — dejalo en modo **"Standard"** y confirmá.

### Crear el emulador (dispositivo virtual)

Para mantener consistencia con lo que ya probó el equipo:

1. En Android Studio, andá a **More Actions → Virtual Device Manager** (o el ícono de celular en la barra de herramientas).
2. Hacé clic en **"Create Device"**.
3. Elegí un dispositivo tipo **Pixel** (el proyecto se probó con **Pixel 10a**; si no está disponible, cualquier Pixel reciente sirve).
4. En la selección de imagen de sistema, elegí **API 37** (o la más cercana disponible — si no está, usá la más nueva que tengas disponible y avisale a Aaron).
5. Finalizá la creación. Vas a ver el dispositivo listado — con el botón ▶ lo podés encender para probar que arranca.

---

## 6. Visual Studio Code + Extensiones

1. Descargá VS Code desde: **https://code.visualstudio.com**
2. Instalalo con las opciones por defecto.
3. Abrí VS Code, andá al ícono de **Extensiones** (los cuadraditos en la barra lateral izquierda, o `Ctrl+Shift+X`).
4. Instalá estas dos extensiones (buscalas por nombre):

| Extensión | Publisher |
|---|---|
| **C# Dev Kit** | Microsoft |
| **.NET MAUI** | Microsoft |

Al instalar **C# Dev Kit**, VS Code puede pedirte iniciar sesión con una cuenta Microsoft/GitHub — es gratis para uso individual, aceptá el inicio de sesión cuando te lo pida.

---

## 7. Clonar el repositorio y preparar el proyecto

Elegí una carpeta donde vas a guardar el proyecto (por ejemplo `Documents\Code Projects`) y corré:

```powershell
git clone https://github.com/aaronhqz1/movilTravelCompanion.git
cd movilTravelCompanion
```

### Preparar el cliente MAUI

```powershell
dotnet restore movilTravelCompanion.sln
```

Esto descarga todas las librerías (paquetes NuGet) que usa el proyecto. Puede tardar varios minutos la primera vez.

### Preparar el backend

```powershell
cd backend
npm install
cd ..
```

Esto descarga las dependencias de Node (Express, SQLite, etc.).

---

## 8. Abrir el proyecto en VS Code

```powershell
code .
```
*(desde la carpeta raíz del repo — el punto significa "esta carpeta")*

La primera vez que abrís el proyecto, C# Dev Kit va a tardar un rato "cargando" el proyecto en segundo plano (lo ves en la barra inferior de VS Code) — esperá a que termine antes de intentar compilar.

**Importante — Startup Project:** este repo tiene 2 proyectos (`movilTravelCompanion` y `movilTravelCompanion.Core`). Antes de compilar necesitás indicarle a C# Dev Kit cuál es el que se ejecuta:

1. En la barra lateral, abrí el ícono de **C# Dev Kit** (parece una carpeta con "C#").
2. Buscá el proyecto **`movilTravelCompanion`** (no `.Core`).
3. Clic derecho → **"Set as Startup Project"**.

---

## 9. Primera prueba: levantar el backend

En una terminal de VS Code (`Ctrl+ñ` o Terminal → New Terminal):

```powershell
cd backend
npm start
```

Deberías ver:
```
Servidor corriendo en http://localhost:3000
Base de datos SQLite inicializada
```

Dejá esta terminal abierta y corriendo mientras trabajás — el backend tiene que estar siempre levantado para que la app pueda pedirle datos.

---

## 10. Primera prueba: compilar e instalar la app

Abrí el emulador que creaste en el paso 5 (desde Android Studio, botón ▶ en el Virtual Device Manager) y esperá a que termine de arrancar.

En **otra** terminal de VS Code (dejando la del backend corriendo aparte):

```powershell
dotnet build movilTravelCompanion/movilTravelCompanion.csproj -f:net10.0-android
```

Cuando termine sin errores, buscá el archivo `.apk` generado:

```powershell
Get-ChildItem -Recurse -Filter "*.apk" .\movilTravelCompanion\bin\Debug\
```

E instalalo en el emulador (reemplazá la ruta si tu archivo tiene otro nombre):

```powershell
adb install -r "movilTravelCompanion\bin\Debug\net10.0-android\com.companyname.moviltravelcompanion-Signed.apk"
```

Abrí la app manualmente en el emulador (buscala en el cajón de apps). Si ves la pantalla de inicio con el clima de una ciudad al azar, **¡ya está funcionando!**

---

## 11. Nota conocida: F5 en VS Code

Al día de hoy, compilar y correr con **F5** directo desde VS Code puede fallar con el error `Debugging canceled: startup project not found`, incluso después de marcar "Set as Startup Project". Es un problema conocido, todavía sin resolver del todo — está documentado en `CONTEXT.md` del repo, sección "Notas técnicas / troubleshooting".

**Mientras tanto, usá el flujo manual del paso 10** (`dotnet build` + `adb install`) — funciona sin problemas y es el que usa el resto del equipo.

---

## 12. Si algo no funciona

- **Revisá primero `CONTEXT.md`** en la raíz del repo — tiene una sección de troubleshooting con problemas ya resueltos anteriormente (y cómo se resolvieron).
- Si el problema no está documentado ahí, avisale a Aaron con:
  - El comando exacto que corriste.
  - El mensaje de error completo (copiá y pegá el texto, no solo una foto si podés evitarlo).
- No hagas `git push` a ninguna rama compartida (`QA`, `UAT`, `main`) sin haber leído el `MANUAL_GIT_COLABORADORES.md`.

---

## Checklist final

Antes de empezar a programar, confirmá que podés hacer todo esto:

- [ ] `git --version`, `node --version`, `dotnet --version` funcionan sin error.
- [ ] `dotnet workload list` muestra `maui-android`.
- [ ] El emulador Android abre y arranca desde Android Studio.
- [ ] VS Code tiene instaladas **C# Dev Kit** y **.NET MAUI**.
- [ ] `npm start` en `backend/` levanta el servidor sin errores.
- [ ] Pudiste compilar e instalar el `.apk`, y la app abre en el emulador mostrando datos reales.

Si marcaste todo, estás listo para trabajar.
