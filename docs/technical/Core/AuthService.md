# movilTravelCompanion.Core.Services.AuthService

## Ubicación
- `movilTravelCompanion.Core/Services/AuthService.cs` (implementación)
- `movilTravelCompanion.Core/Services/IAuthService.cs` (interfaz)

## Propósito
Encapsula toda la comunicación HTTP con los endpoints de autenticación del backend (`/api/auth/register` y `/api/auth/login`), traduciendo request/response JSON a tipos de C# y a excepciones con mensaje claro en español ante fallos.

## Tipo
Interfaz de servicio (`IAuthService`) + Implementación de servicio (`AuthService`), consumidor de HTTP vía `HttpClient`.

## Responsabilidades
- Registrar un usuario nuevo (`RegisterAsync`) contra `POST /api/auth/register` y devolver el `userId` generado.
- Autenticar un usuario existente (`LoginAsync`) contra `POST /api/auth/login` y devolver un `User` completo.
- Detectar respuestas HTTP no exitosas y lanzar `HttpRequestException` con un mensaje descriptivo (incluye operación, código HTTP y cuerpo de error del backend) en vez de dejar pasar una excepción genérica de deserialización.
- Detectar respuestas exitosas pero con cuerpo vacío/nulo y lanzar igualmente una excepción explicativa, en vez de propagar un `NullReferenceException` más adelante.

## Dependencias
- `HttpClient`: inyectado por constructor. En `MauiProgram.cs` se registra como singleton único con `BaseAddress = ApiConfig.BaseUrl`, compartido entre todos los servicios HTTP de `Core`.
- `movilTravelCompanion.Core.Models.User` (tipo de retorno de `LoginAsync`).
- Clase privada anidada `RegisterResponse` (solo para deserializar `{ userId }` de la respuesta de registro; no se expone públicamente).

## Miembros públicos clave

### Propiedades / Campos
Ninguno público (el único campo, `_httpClient`, es privado).

### Métodos
| Firma | Descripción |
|---|---|
| `Task<int> RegisterAsync(string username, string password, string? homeCity)` | Envía `POST /api/auth/register` con `{ username, password, homeCity }`. Devuelve el `userId` recién creado. `homeCity` puede ser `null` (ciudad de origen opcional). |
| `Task<User> LoginAsync(string username, string password)` | Envía `POST /api/auth/login` con `{ username, password }`. Devuelve el `User` deserializado de la respuesta (identidad + ciudad de origen; sin datos de destino de viaje todavía). |

## Flujo y lógica relevante
Ambos métodos siguen el mismo patrón: `PostAsJsonAsync` → chequear `response.IsSuccessStatusCode` → si falla, leer el cuerpo como texto y envolverlo en un `HttpRequestException` con código HTTP + cuerpo crudo del backend (esto permite que la UI muestre, por ejemplo, un 400 de "usuario ya existe" o "contraseña inválida" tal como lo devuelve el backend, sin duplicar esa validación en el cliente) → si es exitosa, deserializar con `ReadFromJsonAsync<T>()` y, si el resultado es `null` (body vacío), lanzar una excepción explicativa en vez de devolver `default`. `RegisterAsync` usa el operador `??` sobre `result?.UserId` para ese último caso; `LoginAsync` usa `??` directo sobre el `User` deserializado.

## Relaciones
- **Quién consume esta clase:** en el proyecto MAUI, `LoginViewModel` (llama `LoginAsync` y guarda el resultado en `ISessionStore`, según `CONTEXT.md`: "LoginViewModel guarda la sesión al loguearse y navega a TravelDestinationPage") y `RegisterViewModel` (llama `RegisterAsync`, con validación client-side de contraseñas antes de llamar al servicio).
- **A quién usa esta clase:** `HttpClient` (inyectado), `User` (modelo de retorno).

## Notas de diseño
Implementa el contrato de `CONTEXT.md` sección "Endpoints del backend — Autenticación" sin variaciones. No implementa lógica de negocio propia (validaciones de contraseña, unicidad de usuario, etc.) — esas reglas viven exclusivamente en el backend, siguiendo la separación estricta documentada en `CONTEXT.md` ("la UI no contiene lógica de negocio; toda lógica y llamadas HTTP viven en Core", y a su vez Core delega las reglas de validación al servidor en vez de duplicarlas).
