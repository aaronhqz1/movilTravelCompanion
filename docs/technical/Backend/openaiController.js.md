# backend/src/controllers/openaiController.js

## Propósito
Genera una recomendación de vestimenta en lenguaje natural llamando a la API de OpenAI (Chat Completions, `gpt-3.5-turbo`), a partir de datos de clima, el estilo de vestimenta elegido y (opcionalmente) la sensibilidad al frío/calor del usuario.

## Tipo
Controller de Express.

## Dependencias
- `openai` (^4.20.1) — SDK oficial de OpenAI para Node, cliente `OpenAI` inicializado con `process.env.OPENAI_API_KEY`.
- No usa `../config/database` (no toca SQLite).

## Endpoints expuestos (si aplica)
| Método | Ruta | Body/Query | Response | Descripción |
|---|---|---|---|---|
| POST | `/api/ai/clothing-recommendation` | `{ city, temperature, weatherCode, humidity, windSpeed, clothingStyle?, coldSensitivity? }` | `200 { success, city, weather, clothingStyle, coldSensitivity, recommendation, timestamp }` / `400`/`401`/`429`/`500 { error }` | Arma un prompt en español con los datos de clima y llama a OpenAI para generar una recomendación de vestimenta de 4 partes (descripción del clima, prendas, accesorios, consejo adicional). |

## Funciones exportadas
| Nombre | Firma/parámetros | Descripción |
|---|---|---|
| `getClothingRecommendation` | `(req, res) => Promise<void>`, lee `req.body.{city, temperature, weatherCode, humidity, windSpeed, clothingStyle, coldSensitivity}` | Valida datos requeridos y valores de estilo/sensibilidad, construye el prompt, llama a `openai.chat.completions.create(...)` y devuelve la recomendación junto con eco de los datos de clima usados. |

Función interna no exportada: `getWeatherDescription(code)` — mapea códigos WMO a descripciones en español (mismo estándar que usa el cliente, ver regla de negocio en CONTEXT.md).

## Esquema de datos (si aplica, ej. database.js)
No aplica (no toca base de datos).

## Lógica y validaciones relevantes
- **Validación de datos requeridos**: `city`, `temperature` y `weatherCode` son obligatorios (`temperature === undefined`/`weatherCode === undefined` en vez de falsy-check, para no rechazar temperatura `0`). Falta cualquiera → `400`.
- **Validación de `clothingStyle`**: valores válidos `['formal', 'casual', 'deportivo']` (comparación case-insensitive vía `.toLowerCase()`); si no viene, default `'casual'`; si viene un valor inválido → `400`.
- **Validación de `coldSensitivity` (opcional)**: valores válidos `['friolento', 'normal', 'caluroso']`. Usa `!= null` (no `!==`) deliberadamente: el comentario en el código explica que clientes que serializan con `System.Text.Json` (como el cliente MAUI) suelen mandar una propiedad ausente como `"coldSensitivity": null` en vez de omitir la clave, y `!= null` trata `null` y `undefined` de la misma forma ("sin preferencia").
- **Ajuste de prompt por sensibilidad**: si `coldSensitivity === 'friolento'`, se agrega una instrucción al prompt para recomendar capas extra aunque la temperatura no lo amerite estrictamente; si `'caluroso'`, se agrega instrucción para prendas livianas/transpirables aunque la temperatura sea fresca; si `'normal'` o ausente, no se agrega instrucción adicional.
- **Manejo de errores específicos de OpenAI**: `error.code === 'invalid_api_key'` → `401`; `error.code === 'insufficient_quota'` → `429`; cualquier otro error → `500` con `details: error.message`.
- El código de clima WMO se traduce a descripción en español vía `getWeatherDescription`, que cubre los mismos códigos documentados en CONTEXT.md como regla de negocio (0=despejado, 1-3=parcialmente nublado, 45/48=niebla, 51-55=llovizna, 61-65=lluvia, 71-75=nieve, 80-82=chubascos, 85-86=chubascos de nieve, 95-99=tormenta), aunque el mapeo interno del controller usa códigos específicos (51/53/55, 61/63/65, etc.) en vez de rangos.
- La respuesta hace eco de `coldSensitivity || 'normal'`, normalizando `undefined`/`null`/`''` a `'normal'` en la respuesta aunque no se haya validado explícitamente ese fallback antes (la validación permite `null`/`undefined` sin fallback explícito previo a este punto).

## Relaciones
- No depende de `../config/database`; es el único controller (junto con `weatherController.js`) que no toca SQLite.
- Su única función se importa y monta en `backend/src/routes/index.js`.
- Consumido desde el cliente MAUI por `movilTravelCompanion.Core/Services/ClothingService.cs` (vía `IClothingService`), usado por la sección "Sugerencia de vestimenta" de `DashboardPage`/`DashboardViewModel`.

## Notas de diseño
- Según CONTEXT.md, este endpoint "ya existía en el backend, sin usar desde el cliente hasta esta sesión" (2026-08-09): fue en esa sesión que se conectó por primera vez desde el cliente MAUI.
- CONTEXT.md documenta que en esa misma sesión se unificaron los estilos de vestimenta a español (`casual`/`formal`/`deportivo`) en este controller y en `preferencesController.js`, "reemplazando el `athletic` en inglés que tenía `openaiController.js` originalmente" — es decir, el valor `'deportivo'` visible en el código actual es resultado de ese cambio, no el original del backend heredado de WeatherApp.
- El manejo de `coldSensitivity` como `null`/`undefined` equivalentes (`!= null`) fue agregado específicamente para compatibilidad con la serialización de `System.Text.Json` del cliente MAUI — es una de las pocas piezas del backend adaptadas puntualmente al comportamiento del nuevo cliente, no heredadas de WeatherApp/React.
- CONTEXT.md verificó en runtime con OpenAI real que el ajuste de prompt por sensibilidad funciona: "Lima 19.8°C + Deportivo + sensibilidad 'friolento' ... devolvió capas extra pese a la temperatura templada".
