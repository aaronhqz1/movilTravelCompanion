# movilTravelCompanion.Core.Models.User

## Ubicación
`movilTravelCompanion.Core/Models/User.cs`

## Propósito
Representa al usuario autenticado de la aplicación: sus datos de identidad, su ciudad de origen (`home`) y su destino de viaje actual. Es el modelo central de sesión: lo que se guarda al iniciar sesión y lo que persiste `ISessionStore` mientras la app está en uso.

## Tipo
Modelo de datos (POCO), usado como DTO de deserialización de respuesta HTTP y como objeto de sesión persistido localmente.

## Responsabilidades
- Mapear la respuesta de `POST /api/auth/login` (identidad + ciudad de origen) a un objeto tipado.
- Transportar también el destino de viaje elegido (`TravelDestination` y sus coordenadas), que **no** viene del backend de login sino que se completa del lado cliente después de elegir destino en `TravelDestinationPage`.
- Servir de forma/contrato para lo que `ISessionStore` guarda y recupera (ver `PreferencesSessionStore` en el proyecto MAUI).

## Dependencias
Ninguna en tiempo de ejecución. Usa `System.Text.Json.Serialization.JsonPropertyName`.

## Miembros públicos clave

### Propiedades / Campos
| Nombre | Tipo | Descripción |
|---|---|---|
| `UserId` | `int` | Identificador del usuario. Mapea `"userId"`. |
| `Username` | `string` | Nombre de usuario. Mapea `"username"`. Default `string.Empty`. |
| `HomeCity` | `string?` | Ciudad de origen del usuario; opcional (puede no haberse configurado al registrarse). Mapea `"homeCity"`. |
| `HomeLatitude` | `double?` | Latitud de la ciudad de origen. Mapea `"homeLatitude"`. |
| `HomeLongitude` | `double?` | Longitud de la ciudad de origen. Mapea `"homeLongitude"`. |
| `TravelDestination` | `string?` | Ciudad de destino del viaje actual, elegida tras el login. Mapea `"travelDestination"`. |
| `DestinationLatitude` | `double?` | Latitud del destino de viaje. Mapea `"destinationLatitude"`. |
| `DestinationLongitude` | `double?` | Longitud del destino de viaje. Mapea `"destinationLongitude"`. |

### Métodos
Ninguno (POCO sin lógica).

## Flujo y lógica relevante
`AuthService.LoginAsync` deserializa la respuesta de `POST /api/auth/login` directamente a este tipo. Según el contrato en `CONTEXT.md`, esa respuesta solo trae `userId`, `username`, `homeCity`, `homeLatitude`, `homeLongitude` — los tres campos de destino (`TravelDestination`, `DestinationLatitude`, `DestinationLongitude`) quedan en `null` tras el login y se completan recién cuando `TravelDestinationViewModel` guarda el destino elegido en la sesión (vía `ISessionStore.SaveUser`), reflejando la regla de negocio "tras login, siempre pasa por travel-destination antes del dashboard". Todos los campos relacionados a ciudad/coordenadas son anulables (`string?`/`double?`) porque tanto la ciudad de origen como el destino son opcionales o se completan en un paso posterior — esto evita depender de valores centinela como `0` o `""` para representar "sin dato".

## Relaciones
- **Quién consume esta clase:** `AuthService.LoginAsync` (la produce), `ISessionStore` (interfaz que la persiste/recupera; implementada como `PreferencesSessionStore` en el proyecto MAUI), y los ViewModels que leen la sesión actual (`LoginViewModel`, `TravelDestinationViewModel`, `DashboardViewModel`, `PreferencesViewModel`, según el grep de uso de `ISessionStore` en el proyecto MAUI).
- **A quién usa esta clase:** a nadie; no tiene dependencias propias.

## Notas de diseño
`CONTEXT.md` señala que "la ciudad de origen es opcional al registrarse; puede configurarse después desde Configuración" — de ahí que `HomeCity`/`HomeLatitude`/`HomeLongitude` sean anulables. La separación entre "ciudad de origen" (persistida en el backend) y "destino de viaje" (solo en la sesión local del dispositivo, vía `PreferencesSessionStore`) es una decisión documentada en el backlog de `CONTEXT.md`: no existe todavía una tabla de historial de viajes en el backend, así que el destino activo "se pisa" al cambiarlo y vive únicamente en este objeto de sesión.
