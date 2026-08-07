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
TODO: completar cuando el flujo de build/run esté estable

## Cómo correr la app
TODO: completar cuando haya una primera pantalla funcional

## Arquitectura
MVVM con movilTravelCompanion (UI) y movilTravelCompanion.Core (lógica
de negocio y servicios HTTP), consumiendo el backend Node/Express original.

## Autor
Aaron Henriquez Leiva
