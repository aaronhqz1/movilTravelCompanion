# Manual de Usuario — movilTravelCompanion

Guía de uso para la app Android **movilTravelCompanion**: clima y sugerencias de vestimenta para tus viajes.

## Tabla de contenidos

1. [Introducción](#1-introducción)
2. [Requisitos](#2-requisitos)
3. [Instalación](#3-instalación)
4. [Primeros pasos / Pantalla de inicio (Home)](#4-primeros-pasos--pantalla-de-inicio-home)
5. [Crear una cuenta](#5-crear-una-cuenta)
6. [Iniciar sesión](#6-iniciar-sesión)
7. [Elegir destino de viaje](#7-elegir-destino-de-viaje)
8. [Panel principal (Dashboard)](#8-panel-principal-dashboard)
9. [Menú (☰)](#9-menú-)
10. [Preferencias](#10-preferencias)
11. [Cerrar sesión](#11-cerrar-sesión)
12. [Preguntas frecuentes / Solución de problemas](#12-preguntas-frecuentes--solución-de-problemas)
13. [Glosario breve](#13-glosario-breve)

---

## 1. Introducción

**movilTravelCompanion** es una app para Android que te muestra el clima de cualquier ciudad del mundo y te ayuda a decidir qué ropa llevar en tu viaje. Es la versión para celular de una app web anterior llamada WeatherApp, pensada ahora para usarse cómodamente desde el bolsillo.

Podés consultar el clima sin necesidad de crear una cuenta, pero si te registrás vas a poder elegir un destino de viaje fijo, guardar un historial de tus últimas búsquedas y recibir una sugerencia de vestimenta generada automáticamente según el clima, tu estilo preferido (casual, formal o deportivo) y qué tan friolento/a o caluroso/a sos.

Este manual explica, pantalla por pantalla, qué podés hacer con la app y cómo hacerlo. No es documentación técnica: no necesitás saber programación para seguirlo.

---

## 2. Requisitos

- Un **dispositivo Android reciente** (la app está preparada para funcionar desde versiones de Android bastante antiguas, así que si tu celular es de los últimos años no vas a tener problemas de compatibilidad).
- **Conexión a internet** (WiFi o datos móviles) en el celular, ya que la app consulta el clima y (si iniciás sesión) tu cuenta contra un servidor.
- El **servidor (backend)** de la app tiene que estar corriendo y accesible desde tu celular. Si vos mismo administrás ese servidor (por ejemplo, en tu computadora), tenés que tenerlo encendido y estar conectado a la misma red WiFi que él. Ver la sección [Preguntas frecuentes](#12-preguntas-frecuentes--solución-de-problemas) para más detalle en términos simples.

---

## 3. Instalación

Para usar la app necesitás instalar su archivo `.apk` en tu celular Android (por ejemplo, transfiriéndolo por cable, por un enlace de descarga, o mediante quien te lo comparta).

Si vos (o quien te la va a instalar) va a **compilar la app desde el código fuente** en una computadora, esa parte es más técnica y ya está explicada paso a paso en el archivo `README.md` del repositorio del proyecto — incluye cómo instalar las herramientas necesarias, cómo levantar el servidor y cómo generar e instalar el `.apk`. Este manual no repite esos pasos técnicos: se enfoca en cómo usar la app una vez que ya está instalada y funcionando en tu celular.

Una vez instalada, vas a ver el ícono de **movilTravelCompanion** entre tus aplicaciones. Tocalo para abrir la app.

---

## 4. Primeros pasos / Pantalla de inicio (Home)

Al abrir la app por primera vez (sin haber iniciado sesión), vas a llegar directamente a la pantalla **Home**. Ahí vas a ver:

- El título **"movilTravelCompanion"**.
- Un texto de bienvenida que te invita a mirar el clima de una ciudad al azar, buscar la tuya, o iniciar sesión para guardar tu historial de viajes.
- Una **tarjeta con el clima de una ciudad elegida al azar** (entre un listado de ciudades conocidas), con su temperatura y detalles.
- Un campo de texto con el texto de ejemplo **"Buscar clima de otra ciudad"**.
- Un botón **"Buscar"**.
- Un botón **"Iniciar Sesión"**.
- Un botón **"Crear cuenta"**.

### Buscar el clima sin tener cuenta

No hace falta registrarte para consultar el clima:

1. Tocá el campo **"Buscar clima de otra ciudad"** y escribí el nombre de la ciudad que te interesa (por ejemplo, "Barcelona").
2. Tocá el botón **"Buscar"** (o simplemente confirmá desde el teclado).
3. La tarjeta de clima se actualiza mostrando la ciudad encontrada, su temperatura y otros detalles.

Esta búsqueda es libre y no queda guardada en ningún historial: el historial de búsquedas solo existe si iniciaste sesión (ver [sección 8](#8-panel-principal-dashboard)).

Si querés aprovechar las funciones de viaje (destino fijo, historial, sugerencias de vestimenta), necesitás [crear una cuenta](#5-crear-una-cuenta) o [iniciar sesión](#6-iniciar-sesión) desde esta misma pantalla.

---

## 5. Crear una cuenta

Desde la pantalla Home, tocá el botón **"Crear cuenta"**. Vas a ver la pantalla **"Crear Cuenta"** con estos campos:

- **Usuario**: el nombre con el que vas a identificarte en la app. Debe tener al menos 3 caracteres.
- **Contraseña**: tu clave de acceso. Por seguridad, debe cumplir con lo siguiente:
  - Al menos 8 caracteres.
  - Al menos una letra mayúscula.
  - Al menos una letra minúscula.
  - Al menos un número.
  - Al menos un carácter especial (por ejemplo `!`, `@`, `#`, `$`, `%`, `.`, etc.).
- **Confirmar contraseña**: repetí exactamente la misma contraseña. Si no coincide con la anterior, la app te va a avisar con el mensaje "Las contraseñas no coinciden." y no vas a poder continuar hasta corregirla.
- **Ciudad de origen (opcional)**: la ciudad donde vivís habitualmente. Podés dejar este campo vacío y completarlo más adelante desde [Preferencias](#10-preferencias)/configuración; no es obligatorio para registrarte.

Pasos:

1. Completá **Usuario**, **Contraseña** y **Confirmar contraseña**. La **Ciudad de origen** es opcional.
2. Tocá el botón **"Registrarme"**.
3. Si algún dato no es válido (usuario muy corto, contraseñas que no coinciden, contraseña débil, usuario ya existente, etc.), vas a ver un mensaje de error en rojo debajo del formulario explicando qué corregir.
4. Si todo está correcto, la app te lleva a la pantalla **"¡Listo!"**, que confirma con el mensaje "¡Cuenta creada! Bienvenido, `<tu usuario>`. Tu cuenta se creó correctamente." Tocá **"Continuar"** para pasar a la pantalla de inicio de sesión.

Si ya tenés una cuenta, en esta misma pantalla podés tocar **"Ya tengo cuenta"** para ir directo a Iniciar Sesión sin registrarte de nuevo.

---

## 6. Iniciar sesión

Desde Home tocá **"Iniciar Sesión"** (o llegá a esta pantalla después de crear tu cuenta). Vas a ver la pantalla **"Iniciar Sesión"** con:

- Campo **Usuario**.
- Campo **Contraseña** (oculta mientras escribís, como es habitual).
- Botón **"Iniciar Sesión"**.
- Botón **"Crear cuenta"**, por si todavía no tenés una cuenta.

Pasos:

1. Escribí tu usuario y contraseña.
2. Tocá **"Iniciar Sesión"**.
3. Si el usuario o la contraseña son incorrectos, o hay un problema de conexión, vas a ver un mensaje de error en rojo.
4. Si el ingreso es correcto, la app te lleva automáticamente a la pantalla **"Destino de Viaje"** (ver siguiente sección) — nunca vas directo al panel principal sin pasar antes por ahí.

Tu sesión queda guardada en el celular, así que la próxima vez que abras la app no necesariamente vas a tener que volver a loguearte (a menos que hayas cerrado sesión manualmente, ver [sección 11](#11-cerrar-sesión)).

---

## 7. Elegir destino de viaje

Después de iniciar sesión, **siempre** vas a pasar por la pantalla **"Destino de Viaje"** antes de llegar al panel principal. Esto es así tanto la primera vez que entrás como cada vez que volvés a loguearte, porque la app te pide confirmar cuál es la ciudad de tu viaje actual antes de mostrarte su clima.

En esta pantalla vas a ver:

- El título **"¿A dónde viajás?"**.
- El texto **"Elegí la ciudad de destino de tu viaje. Vas a ver su clima en el panel principal."**
- Un campo **"Ciudad de destino"**.
- Un botón **"Buscar"**.
- Una vez que la búsqueda encuentra una ciudad, aparece una tarjeta con su clima y un botón **"Confirmar destino"**.

Pasos:

1. Escribí el nombre de la ciudad a la que vas a viajar (por ejemplo, "Madrid").
2. Tocá **"Buscar"**.
3. Revisá la tarjeta de clima que aparece: ciudad y temperatura encontradas.
4. Si es la ciudad correcta, tocá **"Confirmar destino"** para continuar al panel principal (Dashboard).
5. Si no era la ciudad que buscabas, podés escribir otro nombre y volver a buscar antes de confirmar.

Tené en cuenta que la app **no guarda un historial de destinos de viaje pasados**: el destino de viaje es un único valor "activo" que se reemplaza cada vez que elegís uno nuevo (por ejemplo, desde el menú, con "Cambiar Destino"). No existe todavía una sección de "viajes anteriores".

---

## 8. Panel principal (Dashboard)

Esta es la pantalla principal una vez que iniciaste sesión y confirmaste tu destino. Se titula **"Panel"** y muestra, de arriba hacia abajo:

- Un mensaje de bienvenida.
- Una tarjeta con el **clima del destino de viaje elegido**: ciudad, temperatura y detalles (humedad, viento, etc.).
- La sección **"Sugerencia de vestimenta"** (ver más abajo).
- Un campo para **"Buscar clima de otra ciudad"** con su botón **"Buscar"**.
- La sección **"Historial reciente"**, con una lista de tus últimas búsquedas (ciudad y temperatura).

### Buscar otra ciudad desde el Dashboard

A diferencia de la búsqueda libre de la pantalla Home, cuando buscás una ciudad estando logueado, esa búsqueda **se guarda en tu historial**:

1. Escribí el nombre de la ciudad en el campo **"Buscar clima de otra ciudad"**.
2. Tocá **"Buscar"**.
3. El resultado se agrega a tu historial reciente.

Solo se muestran tus **últimas 3 búsquedas** en la lista de historial; las anteriores dejan de aparecer (aunque hayan quedado guardadas en el servidor). Además, no podés guardar la misma ciudad dos veces dentro de las 24 horas: si volvés a buscar una ciudad que ya consultaste recientemente, no se crea una entrada nueva en el historial.

### Cómo funciona la sugerencia de vestimenta

Esta sección te recomienda qué ropa llevar según el clima actual del destino, tu estilo preferido y tu sensibilidad al frío/calor. Pasos:

1. Elegí un **estilo de vestimenta** entre las tres opciones (botones de opción única):
   - **Casual**
   - **Formal**
   - **Deportivo**

   Si configuraste un estilo por defecto en [Preferencias](#10-preferencias), esta opción ya va a venir preseleccionada automáticamente al entrar al Dashboard (y también tu sensibilidad al frío, aunque no se muestre como control separado en esta pantalla — se usa por detrás al pedir la recomendación).
2. Tocá el botón **"Obtener Recomendación de Vestimenta"**.
3. Mientras se genera la sugerencia vas a ver un indicador de carga. Si algo falla (por ejemplo, no hay conexión), aparece un mensaje de error en rojo.
4. La recomendación aparece como texto debajo del botón, generada específicamente para la temperatura, el estado del clima y el estilo/sensibilidad elegidos. Por ejemplo, un clima cálido con estilo deportivo puede sugerir ropa liviana y transpirable, mientras que la misma temperatura con sensibilidad "friolento/a" puede sugerir capas extra.

Podés cambiar el estilo elegido y volver a tocar el botón las veces que quieras para obtener una nueva sugerencia.

---

## 9. Menú (☰)

Una vez que iniciaste sesión, en la barra superior de las pantallas de Destino de Viaje, Panel y Preferencias vas a ver un botón **"Menú"**. Tocalo para abrir el menú lateral (Flyout), que tiene estas cuatro opciones:

- **Viaje Actual** — te lleva directo al panel principal (Dashboard) con el clima de tu destino actual.
- **Cambiar Destino** — te lleva a la pantalla de Destino de Viaje para elegir una ciudad de destino distinta (reemplaza a la anterior; no queda un historial de destinos previos).
- **Preferencias** — te lleva a la pantalla de configuración de estilo de vestimenta y sensibilidad al frío (ver siguiente sección).
- **Cerrar Sesión** — cierra tu sesión actual y te devuelve a la pantalla Home (ver [sección 11](#11-cerrar-sesión)).

El menú solo está disponible cuando tenés una sesión iniciada; no aparece en Home, Login, Crear Cuenta ni en la pantalla de confirmación de registro.

---

## 10. Preferencias

En la pantalla **"Preferencias"** podés configurar, de una vez, los valores que van a usarse automáticamente cada vez que entrás al Dashboard y pedís una sugerencia de vestimenta:

- **Estilo de vestimenta por defecto**: elegí entre **Casual**, **Formal** o **Deportivo**.
- **Sensibilidad al frío/calor**: elegí entre **Friolento/a**, **Normal** o **Caluroso/a**.

Pasos:

1. Abrí el menú (☰) y tocá **"Preferencias"**.
2. Elegí la opción que prefieras en cada uno de los dos grupos.
3. Tocá el botón **"Guardar"**.
4. Si se guardó correctamente, vas a ver un mensaje de confirmación en verde. Si hubo un problema, el mensaje de error aparece en rojo.

Una vez guardadas, la próxima vez que entres al Dashboard estas dos opciones (estilo de vestimenta y sensibilidad al frío) van a venir precargadas automáticamente, así no tenés que elegirlas cada vez antes de pedir una recomendación de vestimenta. Igualmente, podés cambiar el estilo puntualmente desde el propio Dashboard sin que eso modifique lo guardado en Preferencias.

Si nunca guardaste preferencias, la app usa por defecto **Casual** y sensibilidad **Normal**.

---

## 11. Cerrar sesión

Para cerrar tu sesión:

1. Abrí el menú (☰) desde cualquier pantalla donde esté disponible (Destino de Viaje, Panel o Preferencias).
2. Tocá **"Cerrar Sesión"**.

La app cierra tu sesión de inmediato y te devuelve a la pantalla **Home**, donde podés volver a iniciar sesión con la misma cuenta u otra, o seguir usando la búsqueda de clima sin cuenta.

---

## 12. Preguntas frecuentes / Solución de problemas

**No me conecta / aparece un error de conexión al buscar clima, registrarme o iniciar sesión.**
La app necesita comunicarse con un servidor para funcionar. Si ese servidor no está encendido, o tu celular no está conectado a la misma red que el servidor, la app no va a poder traer datos y vas a ver un mensaje de error. Si vos mismo administrás el servidor de esta app, asegurate de tenerlo corriendo y de que tu celular esté conectado a la misma red WiFi que la computadora donde corre. Si la app te la instaló o configuró otra persona, consultale a ella si el servidor está disponible.

**Me olvidé mi contraseña, ¿cómo la recupero?**
Actualmente la app **no tiene una función de "olvidé mi contraseña"** ni de recuperación de cuenta. Si perdés tu contraseña, la única opción es crear una cuenta nueva con otro nombre de usuario. Tené esto en cuenta al elegir tu contraseña.

**¿Por qué no veo más de 3 búsquedas en mi historial?**
Es una limitación conocida de la app: el Dashboard solo muestra tus **últimas 3 búsquedas**. Las búsquedas más antiguas no se muestran en la lista, aunque hayan quedado registradas.

**Busqué la misma ciudad dos veces y no apareció de nuevo en el historial.**
Es intencional: no se puede guardar la misma ciudad dos veces en el historial dentro de un período de 24 horas. Si volvés a buscarla antes de que pase ese tiempo, va a mostrarte el clima igual, pero no se crea una entrada nueva en el historial.

**¿Puedo ver mis viajes anteriores o tener varios destinos guardados a la vez?**
No por ahora. La app solo mantiene **un destino de viaje activo** a la vez; al elegir uno nuevo (desde "Cambiar Destino"), reemplaza al anterior. No hay una sección de historial de viajes ni de fechas de inicio/fin de viaje en esta versión.

**¿La app detecta mi ubicación automáticamente?**
No. La ciudad que ves al abrir Home sin sesión es elegida al azar entre un listado de ciudades predefinidas, no se calcula por GPS ni ubicación real del dispositivo.

**Mi contraseña no es aceptada al registrarme, ¿por qué?**
Revisá que cumpla los cinco requisitos detallados en la sección [Crear una cuenta](#5-crear-una-cuenta): mínimo 8 caracteres, al menos una mayúscula, una minúscula, un número y un carácter especial.

---

## 13. Glosario breve

- **Ciudad de origen**: la ciudad donde vivís habitualmente. Se puede indicar (opcionalmente) al crear la cuenta y es independiente del destino de viaje.
- **Destino de viaje**: la ciudad a la que estás viajando, elegida después de iniciar sesión. Es un único valor activo por vez: al cambiarlo, se reemplaza el anterior.
- **Sensibilidad al frío/calor**: preferencia personal (Friolento/a, Normal o Caluroso/a) que ajusta la sugerencia de vestimenta — por ejemplo, sugiriendo capas extra para alguien "friolento/a" aunque la temperatura no sea muy baja.
- **Estilo de vestimenta**: el tipo de ropa que preferís para tus salidas (Casual, Formal o Deportivo), usado junto con el clima para generar la sugerencia.
- **Historial reciente**: lista de las últimas 3 ciudades que buscaste estando logueado, con su temperatura.
- **Menú / Flyout (☰)**: el menú lateral que se abre desde el botón "Menú" en la barra superior, disponible solo con sesión iniciada.
- **Panel principal (Dashboard)**: la pantalla central de la app una vez logueado, con el clima del destino, la sugerencia de vestimenta, la búsqueda de otras ciudades y el historial reciente.
