# movilTravelCompanion.Core.Configuration.ApiConfig

## Ubicación
`movilTravelCompanion.Core/Configuration/ApiConfig.cs`

## Propósito
Centraliza la URL base del backend Express en una única constante, evitando que cada servicio HTTP tenga la dirección del servidor hardcodeada por separado. Existe porque la URL correcta depende del entorno de ejecución (emulador Android vs. dispositivo físico) y se necesita un único punto de cambio.

## Tipo
Configuración estática (clase `static` con una constante `public const string`).

## Responsabilidades
- Exponer `BaseUrl`, la URL raíz del backend (`http://10.0.2.2:3000` por defecto, IP especial que el emulador Android usa para redirigir al `localhost` de la PC anfitriona).
- Documentar en comentarios cómo cambiar el valor para probar en un dispositivo físico (IP LAN de la PC, misma red WiFi que el celular).

## Dependencias
Ninguna. No depende de otras clases ni recibe nada por inyección; es una constante de compilación.

## Miembros públicos clave

### Propiedades / Campos
| Nombre | Tipo | Descripción |
|---|---|---|
| `BaseUrl` | `const string` | URL base del backend Express, usada como `HttpClient.BaseAddress` para todos los servicios HTTP de `Core`. Valor por defecto: `"http://10.0.2.2:3000"`. |

### Métodos
Ninguno.

## Flujo y lógica relevante
No tiene lógica ejecutable: es solo una constante leída una vez, en `MauiProgram.cs`, al construir el `HttpClient` singleton (`new HttpClient { BaseAddress = new Uri(ApiConfig.BaseUrl) }`). Cambiar de emulador a dispositivo físico requiere editar manualmente este archivo y recompilar (no es una configuración dinámica ni leída de un `appsettings`).

## Relaciones
- **Quién consume esta clase:** `movilTravelCompanion/MauiProgram.cs` (proyecto MAUI), al registrar el `HttpClient` singleton inyectado luego en `AuthService`, `WeatherApiService`, `HistoryService`, `PreferencesService` y `ClothingService`.
- **A quién usa esta clase:** a nadie; no tiene dependencias.

## Notas de diseño
Según `CONTEXT.md` ("Networking: backend local, cliente en emulador y dispositivo físico"), esta configuración centralizada fue una decisión explícita para no hardcodear la URL del backend en cada `Service` por separado. El backend corre local (`npm start` en la PC) sin desplegarse a internet, y PC/celular deben compartir la misma red WiFi cuando se prueba en dispositivo físico.
