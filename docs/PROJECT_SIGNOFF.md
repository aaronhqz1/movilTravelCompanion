# Project Sign-Off Document — movilTravelCompanion

| Campo | Valor |
|---|---|
| Proyecto | movilTravelCompanion |
| Tipo | App Android nativa (.NET MAUI) — migración de la app web WeatherApp (React + Node/Express) |
| Autor / Desarrollador | Aaron Henriquez Leiva |
| Repositorio | https://github.com/aaronhqz1/movilTravelCompanion |
| Repositorio de referencia | https://github.com/aaronhqz1/WeatherApp |
| Período de desarrollo | 2026-08-07 a 2026-08-09 |
| Fecha de este documento | 2026-08-10 |
| Estado del proyecto | **MVP funcional, verificado en emulador. Pendiente de prueba en dispositivo físico.** |

---

## Tabla de contenidos
1. [Resumen ejecutivo](#resumen-ejecutivo)
2. [Objetivo del proyecto](#objetivo-del-proyecto)
3. [Alcance](#alcance)
4. [Arquitectura entregada](#arquitectura-entregada)
5. [Checklist de entregables](#checklist-de-entregables)
6. [Evidencia de aceptación / pruebas realizadas](#evidencia-de-aceptación--pruebas-realizadas)
7. [Incidencias resueltas durante el desarrollo](#incidencias-resueltas-durante-el-desarrollo)
8. [Limitaciones y riesgos conocidos](#limitaciones-y-riesgos-conocidos)
9. [Fuera de alcance / backlog futuro](#fuera-de-alcance--backlog-futuro)
10. [Documentación relacionada](#documentación-relacionada)
11. [Criterios de aceptación](#criterios-de-aceptación)
12. [Aprobación](#aprobación)

---

## Resumen ejecutivo

`movilTravelCompanion` es la migración de la aplicación web **WeatherApp** (React + Node/Express) a una **app nativa Android** construida con **.NET MAUI**, siguiendo el patrón **MVVM**. El backend Node/Express/SQLite original se reutiliza sin reescribir su lógica (fue copiado al mismo repositorio para simplificar el mantenimiento, con permiso de edición puntual cuando hizo falta corregir un bug o extender un endpoint).

El desarrollo se realizó en tres sesiones de trabajo (7, 8 y 9 de agosto de 2026) y cubrió el flujo completo de la app original — inicio sin sesión, registro, login, selección de destino de viaje, panel principal con clima e historial — más una funcionalidad que en la web original nunca se conectó al cliente: la **recomendación de vestimenta por IA (OpenAI)**, sumada a una pantalla de **Preferencias** y un **menú de navegación (Flyout)** que no existían en la versión web.

A la fecha de este documento, el flujo completo fue **verificado manualmente de punta a punta en el emulador Android** (Pixel 10a, API 37) contra el backend real y la API de OpenAI real. No se realizaron pruebas en dispositivo físico.

## Objetivo del proyecto

Migrar el cliente de WeatherApp de React a un cliente Android nativo en .NET MAUI, manteniendo el mismo backend y el mismo contrato de API REST, con arquitectura MVVM y separación estricta entre UI (proyecto MAUI) y lógica de negocio (proyecto `movilTravelCompanion.Core`).

## Alcance

### Dentro de alcance (implementado)
- Flujo de navegación completo replicado desde `App.jsx` del proyecto original: `Home → Login → Register → RegistrationSuccess → TravelDestination → Dashboard`.
- Autenticación (registro y login) contra el backend Express existente.
- Consulta de clima por ciudad aleatoria (usuario no autenticado), por búsqueda de ciudad, y por coordenadas, vía Open-Meteo (a través del backend).
- Selección de ciudad de destino de viaje, obligatoria tras el login.
- Panel principal (Dashboard) con clima del destino, búsqueda de otras ciudades, e historial de las últimas 3 búsquedas.
- Persistencia de sesión en el dispositivo (`Preferences` de MAUI, reemplazando `localStorage` de la web).
- **Agregado respecto al plan original de v1** (sesión 2026-08-09, ver `CONTEXT.md`):
  - Pantalla de **Preferencias** (estilo de vestimenta por defecto, sensibilidad al frío), nueva — no existía en la app web.
  - **Recomendación de vestimenta vía OpenAI**, conectada al cliente por primera vez (el endpoint ya existía en el backend original pero nunca se consumía desde ningún frontend).
  - **Menú de navegación (Shell Flyout)** con acceso a Viaje Actual, Cambiar Destino, Preferencias y Cerrar Sesión.

### Fuera de alcance (decisión explícita)
- Target **exclusivo Android** — no se instaló workload de iOS, no se generó build para iOS/macOS/Windows.
- No se modificó la lógica de negocio del backend salvo ediciones puntuales documentadas (ver `CONTEXT.md`, sección "Backend: ubicación y edición").
- No se implementó recuperación de contraseña (no existía en la app original).
- Historial de viajes anteriores, duración de viaje, y vínculo entre historial de clima y viaje específico — ver [Fuera de alcance / backlog futuro](#fuera-de-alcance--backlog-futuro).

## Arquitectura entregada

```
movilTravelCompanion.sln
├── movilTravelCompanion/          → Cliente MAUI (net10.0-android). Views (XAML) + ViewModels (MVVM, CommunityToolkit.Mvvm).
├── movilTravelCompanion.Core/     → Librería .NET pura: Models, Services (HTTP hacia el backend), Configuration.
└── backend/                       → Node/Express/SQLite (copiado desde WeatherApp, reutilizado tal cual con ediciones puntuales).
```

- **Patrón:** MVVM, con inyección de dependencias configurada en `MauiProgram.cs`.
- **Regla de separación respetada:** toda la lógica de negocio y las llamadas HTTP viven en `movilTravelCompanion.Core`; el proyecto MAUI solo contiene UI y navegación.
- **Networking:** backend corre localmente; el cliente apunta a `10.0.2.2:3000` en emulador o a la IP LAN de la PC en dispositivo físico, vía `ApiConfig` centralizado.

El detalle clase por clase está en la documentación técnica — ver [Documentación relacionada](#documentación-relacionada).

## Checklist de entregables

| # | Entregable | Estado |
|---|---|---|
| 1 | Proyecto MAUI creado y estructurado (`movilTravelCompanion` + `movilTravelCompanion.Core`) | ✅ Completo |
| 2 | Modelos de datos (`User`, `WeatherData`, `HourlyForecast`, `HistoryEntry`, `UserPreferences`, `ClothingRecommendation`) | ✅ Completo |
| 3 | Servicios HTTP hacia el backend (`AuthService`, `WeatherApiService`, `HistoryService`, `PreferencesService`, `ClothingService`) | ✅ Completo |
| 4 | Manejo de sesión en el dispositivo (`ISessionStore` / `PreferencesSessionStore`) | ✅ Completo |
| 5 | Pantalla y flujo de Registro, con validación client-side de contraseñas | ✅ Completo |
| 6 | Pantalla y flujo de Login | ✅ Completo |
| 7 | Pantalla de confirmación post-registro | ✅ Completo |
| 8 | Pantalla de selección de destino de viaje | ✅ Completo |
| 9 | Dashboard: clima del destino, búsqueda, historial (últimas 3) | ✅ Completo |
| 10 | Home pública (sin sesión): clima de ciudad aleatoria + búsqueda | ✅ Completo |
| 11 | Componente reutilizable `WeatherCardView` | ✅ Completo |
| 12 | Navegación real con Shell (rutas registradas, back stack controlado) | ✅ Completo |
| 13 | Pantalla de Preferencias (estilo de vestimenta, sensibilidad al frío) | ✅ Completo |
| 14 | Recomendación de vestimenta por IA (OpenAI) integrada en Dashboard | ✅ Completo |
| 15 | Menú de navegación (Shell Flyout) | ✅ Completo |
| 16 | Backend Node/Express/SQLite operativo, con extensión para Preferencias | ✅ Completo |
| 17 | `android:usesCleartextTraffic` y configuración de red para HTTP local | ✅ Completo |
| 18 | Proyecto de tests unitarios (`movilTravelCompanion.Tests`) | ❌ **No creado** |
| 19 | Prueba en dispositivo Android físico | ❌ **No realizada** (solo emulador) |
| 20 | F5 / debugging integrado en VS Code | ⚠️ **Parcial** — el build y deploy funcionan por línea de comandos (`dotnet build -t:Run`); la integración de "Startup Project" con la extensión C# Dev Kit quedó pendiente de confirmar manualmente |

## Evidencia de aceptación / pruebas realizadas

Todas las pruebas se realizaron manualmente contra el backend real (no mockeado) y, cuando correspondía, contra la API real de OpenAI, en el emulador **Pixel 10a API 37**, usando `adb` para instalar y capturar pantallas.

- **Registro → Login → TravelDestination → Dashboard → Cerrar sesión**, flujo completo de punta a punta, con datos reales (ej. registrar cuenta, loguear, buscar Madrid como destino, buscar París en Dashboard y confirmar que queda en el historial).
- **Home pública**: clima de ciudad aleatoria real al abrir sin sesión, búsqueda de ciudad funcional, navegación Home ↔ Login y Home ↔ Register en ambos sentidos, incluyendo el botón "atrás" físico del emulador.
- **Preferencias + recomendación de vestimenta con OpenAI real**: casos probados incluyeron Tokio 31.3 °C + estilo Deportivo (devolvió ropa liviana transpirable) y Lima 19.8 °C + Deportivo + sensibilidad "friolento" precargada desde Preferencias (devolvió recomendación de capas extra pese a la temperatura templada), confirmando que ambos parámetros llegan correctamente al prompt.
- **Menú Flyout y control del back stack**: dos ciclos completos de "Cambiar Destino" (Berlín, luego Cairo) seguidos de un solo toque de "atrás" llevan directo a Home sin acumular pantallas; antes del fix, un solo ciclo dejaba 6 pantallas apiladas.

No se realizaron pruebas automatizadas (unit tests) ni pruebas en dispositivo físico — ver limitaciones.

## Incidencias resueltas durante el desarrollo

Registro resumido; el detalle técnico completo de cada una está en `CONTEXT.md` (sección "Notas técnicas / troubleshooting"):

| Incidencia | Causa raíz | Resolución |
|---|---|---|
| Crash al iniciar la app (`StaticResource not found for key Headline`) | Orden de inicialización de DI + assemblies desactualizados por Fast Deployment | Resolver la página dentro de `CreateWindow` en vez del constructor de `App`; `EmbedAssembliesIntoApk=true` en Debug |
| `Connection failure` en Register/Login | Falta `android:usesCleartextTraffic` en el manifest (Android bloquea HTTP sin cifrar desde API 28) | Agregado el atributo en `AndroidManifest.xml` |
| Navegación absoluta rota hacia rutas globales de Shell | MAUI Shell no permite `"//"` hacia una ruta que no es `ShellContent` si queda sola en la pila | Cambiado a push relativo (`GoToAsync(nameof(Pagina))`) |
| `NavigationStack` crecía sin límite al usar el menú Flyout | Cada navegación por Flyout apilaba páginas nuevas sin sacar las anteriores | `ShellNavigationHelper.TrimNavigationStack()` |
| Ícono de hamburguesa no aparecía en páginas alcanzadas por *push* | Shell solo muestra el ícono automático en la raíz de una sección | `ToolbarItem` explícito que fuerza `FlyoutIsPresented = true` |
| `coldSensitivity` nulo rechazado por el backend | `System.Text.Json` serializa una propiedad ausente como `null` explícito, no ausente | Backend trata `null` igual que ausente en `openaiController.js` |

## Limitaciones y riesgos conocidos

- **No probado en dispositivo físico.** Todas las pruebas fueron en emulador. El flujo de red por IP LAN (a diferencia de `10.0.2.2` del emulador) no fue validado end-to-end.
- **Sin cobertura de tests automatizados.** El proyecto `movilTravelCompanion.Tests` está planificado en la estructura objetivo pero no fue creado.
- **Backend solo local.** No hay despliegue a internet; requiere que el backend corra en la misma máquina/red que el dispositivo, lo cual es una limitación operativa para cualquier uso fuera de desarrollo/demo.
- **API key de OpenAI de terceros.** La recomendación de vestimenta depende de una clave de API paga/con cuota, gestionada fuera del repo (`.env`, no versionado); si la clave no está configurada, esa función específica no funciona (el resto de la app no depende de ella).
- **Debugging F5 en VS Code no confirmado del todo** — funciona por línea de comandos, pero la integración con "Startup Project" de la extensión C# Dev Kit quedó como paso manual pendiente.

## Fuera de alcance / backlog futuro

Documentado explícitamente en `CONTEXT.md` como no implementado en esta entrega, para retomar en una futura iteración:
- Historial de viajes anteriores ("Crear Viaje" / "Viajes Anteriores") — requiere tabla nueva en el backend.
- Duración del viaje actual (fecha de inicio/fin del destino elegido).
- Vínculo entre el historial de consultas de clima y el viaje/destino al que pertenecen.
- Uso de GPS en Home para determinar automáticamente la ciudad del usuario, en vez de mostrar una ciudad aleatoria a usuarios no autenticados.

## Documentación relacionada

| Documento | Contenido |
|---|---|
| [`docs/MANUAL_USUARIO.md`](./MANUAL_USUARIO.md) | Manual de usuario final — cómo usar la app, pantalla por pantalla |
| [`docs/technical/Core/INDEX.md`](./technical/Core/INDEX.md) | Documentación técnica de Modelos y Servicios (`movilTravelCompanion.Core`) |
| [`docs/technical/Client/INDEX.md`](./technical/Client/INDEX.md) | Documentación técnica del cliente MAUI (Views, ViewModels, Controls) |
| [`docs/technical/Backend/INDEX.md`](./technical/Backend/INDEX.md) | Documentación técnica del backend Node/Express |
| [`CONTEXT.md`](../CONTEXT.md) | Bitácora completa de decisiones técnicas, contrato de API y troubleshooting |
| [`README.md`](../README.md) | Instrucciones de instalación y configuración del entorno de desarrollo |

## Criterios de aceptación

Este proyecto se considera **aceptado como MVP funcional para entorno de desarrollo/demo** si se cumplen los siguientes criterios. Marcar el estado real de cada uno antes de firmar:

| Criterio | Cumple |
|---|---|
| El flujo completo (Home → Registro → Login → Destino → Dashboard → Cerrar sesión) funciona sin errores contra el backend real | ✅ Sí (verificado en emulador) |
| La recomendación de vestimenta por IA responde correctamente con distintas combinaciones de clima/estilo/sensibilidad | ✅ Sí (verificado en emulador) |
| La app no crashea al iniciar ni al navegar por el menú repetidamente | ✅ Sí (verificado en emulador) |
| La app corre en un dispositivo Android físico, no solo emulador | ❌ Pendiente |
| Existe cobertura de tests automatizados sobre `movilTravelCompanion.Core` | ❌ Pendiente |

**Nota:** los dos últimos criterios están pendientes; si son requisito de aceptación formal para esta entrega, el sign-off debe marcarse como condicional hasta resolverlos.

## Aprobación

| Rol | Nombre | Firma | Fecha |
|---|---|---|---|
| Desarrollador | Aaron Henriquez Leiva | ______________________ | ____________ |
| Revisor / Aprobador | ______________________ | ______________________ | ____________ |

**Observaciones del aprobador:**

```




```
