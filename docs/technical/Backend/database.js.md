# backend/src/config/database.js

## Propósito
Configura y abre la conexión a la base de datos SQLite del backend (archivo `database.sqlite` en la raíz de `backend/`), y crea (si no existen) las tres tablas del esquema: `users`, `weather_history` y `user_preferences`. Exporta la instancia de conexión (`db`) para que los controllers ejecuten queries directamente sobre ella.

## Tipo
Configuración de base de datos.

## Dependencias
- `sqlite3` (^5.1.6), usado en modo `.verbose()` (agrega stack traces más útiles a los errores async).
- `path` (módulo built-in de Node) — para resolver `dbPath` de forma independiente del directorio de trabajo (`path.join(__dirname, '../../database.sqlite')`, es decir la raíz de `backend/`).

## Endpoints expuestos (si aplica)
No aplica — este módulo no define rutas HTTP.

## Funciones exportadas
| Nombre | Firma/parámetros | Descripción |
|---|---|---|
| `db` (export por defecto de `module.exports`) | instancia de `sqlite3.Database` | Conexión abierta a `database.sqlite`, usada directamente por todos los controllers vía `require('../config/database')` para llamar `db.run`, `db.get`, `db.all`. |

`initializeDatabase()` es una función interna (no exportada) que se ejecuta automáticamente en el callback de conexión exitosa.

## Esquema de datos (si aplica, ej. database.js)

**Tabla `users`**
| Columna | Tipo | Constraints |
|---|---|---|
| `id` | INTEGER | PRIMARY KEY AUTOINCREMENT |
| `username` | TEXT | UNIQUE NOT NULL |
| `password` | TEXT | NOT NULL (hash bcrypt, no texto plano) |
| `home_city` | TEXT | nullable — ciudad de origen, opcional al registrarse |
| `home_latitude` | REAL | nullable |
| `home_longitude` | REAL | nullable |
| `created_at` | DATETIME | DEFAULT CURRENT_TIMESTAMP |

**Tabla `weather_history`**
| Columna | Tipo | Constraints |
|---|---|---|
| `id` | INTEGER | PRIMARY KEY AUTOINCREMENT |
| `user_id` | INTEGER | NOT NULL, FOREIGN KEY → `users(id)` |
| `city` | TEXT | NOT NULL |
| `latitude` | REAL | nullable |
| `longitude` | REAL | nullable |
| `temperature` | REAL | nullable |
| `humidity` | INTEGER | nullable |
| `wind_speed` | REAL | nullable |
| `weather_code` | INTEGER | nullable — código WMO |
| `query_time` | DATETIME | DEFAULT CURRENT_TIMESTAMP |

**Tabla `user_preferences`** (nueva, sesión 2026-08-09)
| Columna | Tipo | Constraints |
|---|---|---|
| `user_id` | INTEGER | PRIMARY KEY, FOREIGN KEY → `users(id)` (relación 1:1, no autoincremental) |
| `default_clothing_style` | TEXT | DEFAULT `'casual'` |
| `cold_sensitivity` | TEXT | DEFAULT `'normal'` |

Nota: las columnas `DEFAULT` de SQLite solo aplican en un `INSERT` que omita esa columna explícitamente; en la práctica `preferencesController.js` siempre pasa ambos valores (con su propio fallback en JS), por lo que estos `DEFAULT` actúan como red de seguridad adicional, no como el mecanismo primario de default.

## Lógica y validaciones relevantes
- La conexión se abre de forma perezosa/inmediata al hacer `require`, con un callback: si falla, solo loguea el error por consola (no lanza excepción ni bloquea el arranque del servidor).
- `initializeDatabase()` usa `db.serialize()` para garantizar que los tres `CREATE TABLE IF NOT EXISTS` se ejecuten en orden (aunque no hay dependencia real de orden entre ellos salvo legibilidad de logs).
- No hay migraciones formales: el esquema evoluciona agregando `CREATE TABLE IF NOT EXISTS` nuevos a este archivo (así se agregó `user_preferences` en la sesión 2026-08-09) — no hay soporte para `ALTER TABLE` de columnas existentes.
- Las foreign keys se declaran en el `CREATE TABLE` pero SQLite no las aplica por defecto salvo que se ejecute `PRAGMA foreign_keys = ON`, que este módulo no invoca — es decir, la integridad referencial no está forzada a nivel de motor, solo es documental/nominal.

## Relaciones
- Es importado por `authController.js`, `historyController.js` y `preferencesController.js` (todos hacen `require('../config/database')`). `openaiController.js` y `weatherController.js` no lo usan (no tocan la base de datos).
- No es consumido directamente por el cliente MAUI; es infraestructura interna del backend. Indirectamente sostiene los datos que devuelven `AuthService`, `HistoryService` y `PreferencesService` en `movilTravelCompanion.Core/Services/`.

## Notas de diseño
- Según CONTEXT.md, la tabla `user_preferences` y su columna par (`default_clothing_style`, `cold_sensitivity`) se agregaron en la sesión del 2026-08-09 junto con `preferencesController.js`, como parte de la feature nueva de Preferencias de usuario (que no existía en la app React original). El diseño 1:1 con `user_id` como PRIMARY KEY (en vez de un `id` autoincremental propio) refleja que cada usuario tiene a lo sumo una fila de preferencias.
- CONTEXT.md documenta explícitamente que el backend "se reutiliza tal cual vino, con ediciones puntuales permitidas si son necesarias" — este archivo es uno de los pocos efectivamente editados durante la migración.
