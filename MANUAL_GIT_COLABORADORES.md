# Manual de Git — movilTravelCompanion

Guía para trabajar con el repositorio sin experiencia previa en Git. Seguí los pasos en orden y no deberías tener problemas.

---

## 1. Conceptos básicos (en criollo)

No hace falta entender Git a fondo para usarlo bien. Con estas 4 ideas alcanza:

| Término | Qué es, en simple |
|---|---|
| **Rama (branch)** | Una copia independiente del código donde podés trabajar sin afectar el trabajo de los demás. |
| **Commit** | Un "guardado" de tus cambios, con un mensaje explicando qué hiciste. Como guardar una versión de un documento, pero con una nota de qué cambió. |
| **Push** | Subir tus commits (guardados localmente en tu compu) al repositorio en GitHub, para que los demás los vean. |
| **Pull** | Traer a tu compu los cambios que otros ya subieron a GitHub. |
| **Pull Request (PR)** | Un pedido formal de "quiero que mi rama se combine con esta otra rama". Alguien lo revisa antes de aprobarlo — es el paso seguro para juntar el trabajo de varias personas. |

---

## 2. Estructura de ramas de este proyecto

```
main
 └── UAT
      └── QA
           ├── Aaron
           └── Amanda
```

- **`main`** — Versión final y estable. Nadie trabaja acá directamente.
- **`UAT`** — Versión en pruebas de aceptación, un escalón antes de `main`.
- **`QA`** — Versión donde se juntan los aportes de todos los colaboradores para probarse en conjunto.
- **`Aaron`** / **`Amanda`** — Tu rama personal de desarrollo. Acá es donde trabajás día a día, y desde acá subís tu trabajo hacia `QA` cuando está listo.

**Regla de oro: trabajá siempre en tu propia rama (`Aaron` o `Amanda`), nunca directo en `QA`, `UAT` ni `main`.**

---

## 3. Configuración inicial (una sola vez)

Si todavía no tenés el proyecto clonado en tu computadora:

```powershell
git clone https://github.com/aaronhqz1/movilTravelCompanion.git
cd movilTravelCompanion
```

Configurá tu nombre y correo (así tus commits quedan identificados):

```powershell
git config --global user.name "Tu Nombre"
git config --global user.email "tu-correo@ejemplo.com"
```

Cambiate a tu rama personal:

```powershell
git checkout Aaron
```
*(reemplazá `Aaron` por tu propia rama, por ejemplo `Amanda`)*

---

## 4. Rutina diaria de trabajo

### Paso 1 — Antes de empezar a programar, actualizate

Siempre, **antes de tocar código**, traé lo último de `QA` a tu rama, para no trabajar sobre una versión vieja:

```powershell
git checkout Aaron
git pull origin Aaron
git fetch origin
git merge origin/QA
```

Si aparece un mensaje de **conflicto** (`CONFLICT`), no te preocupes ni intentes resolverlo solo — avisale a Aaron (el administrador del repo) antes de seguir.

### Paso 2 — Programá tranquilo

Hacé tus cambios en el código como siempre.

### Paso 3 — Guardá tu trabajo en commits

Guardá seguido, en pasos chicos, con mensajes claros:

```powershell
git add .
git commit -m "Descripción corta de lo que hice"
```

Ejemplos de buenos mensajes:
- `"Agregar validación de email en el formulario de registro"`
- `"Corregir error al cargar el historial de clima"`

Evitá mensajes como `"cambios"` o `"arreglos"` — no ayudan a nadie a entender qué pasó después.

### Paso 4 — Subí tu trabajo a tu rama en GitHub

```powershell
git push origin Aaron
```

Con esto tu trabajo ya está respaldado en GitHub, en tu propia rama. Todavía **no** se mezcló con el de nadie más.

---

## 5. Cómo subir tu trabajo a QA (cuando está listo para compartirse)

Cuando tengas una parte de trabajo terminada y probada, y quieras que se sume al trabajo conjunto en `QA`, hacelo con un **Pull Request** — es más seguro que mezclar ramas a mano porque queda un registro claro y alguien puede revisarlo antes de aprobarlo.

1. Andá a **https://github.com/aaronhqz1/movilTravelCompanion**
2. GitHub suele mostrar un botón amarillo: **"Compare & pull request"** apenas hacés push. Si no aparece, andá a la pestaña **Pull requests → New pull request**.
3. Configurá:
   - **base:** `QA` (a dónde querés llevar tus cambios)
   - **compare:** `Aaron` (tu rama, de dónde vienen los cambios)
4. Ponele un título corto explicando qué agregaste, y en la descripción un resumen de los cambios.
5. Hacé clic en **"Create pull request"**.
6. Avisale al administrador (o revisor designado) para que lo revise.
7. Una vez aprobado, se hace clic en **"Merge pull request"** — recién ahí tu trabajo pasa a formar parte de `QA`.

**No hace falta usar la terminal para este paso** — todo se hace desde la página de GitHub, con botones.

---

## 6. Reglas de oro

✅ **Sí:**
- Trabajar siempre en tu propia rama (`Aaron` o `Amanda`).
- Actualizarte desde `QA` antes de empezar a programar cada día.
- Commits chicos y frecuentes, con mensajes claros.
- Usar Pull Request para llevar tu trabajo a `QA`.
- Avisar si algo no entendés o si aparece un conflicto — mejor preguntar que forzar algo.

🚫 **No:**
- No hacer `push` directo a `QA`, `UAT` ni `main`.
- No usar `git push --force` (podés borrar el trabajo de otra persona sin darte cuenta).
- No resolver conflictos de merge por tu cuenta si no estás seguro — pedí ayuda.

---

## 7. Si algo sale mal

- **Mensaje de "conflict" al hacer `merge` o `pull`:** parate ahí, no sigas escribiendo comandos al azar. Avisale al administrador del repo con una captura de pantalla del mensaje.
- **"Estoy perdido, no sé en qué rama estoy":** corré `git status` — te dice en qué rama estás parado y si tenés cambios sin guardar.
- **"Quiero ver el historial de lo que se hizo":** `git log --oneline -10` te muestra los últimos 10 commits.

Ante la duda, siempre es mejor preguntar antes de ejecutar un comando que no entendés del todo.

---

## 8. Para el administrador del repo (Aaron)

Para evitar accidentes mientras el equipo se acostumbra a este flujo, conviene proteger las ramas compartidas desde GitHub:

1. Andá a **Settings → Branches** en el repositorio.
2. Agregá una regla de protección (**"Add branch protection rule"**) para `main`, `UAT` y `QA`.
3. Activá al menos:
   - **"Require a pull request before merging"** — nadie puede pushear directo, solo por PR.
   - **"Require approvals"** (opcional, 1 aprobación) si querés revisar antes de que se mezcle algo.
4. Guardá los cambios.

Con esto, aunque alguien se equivoque y intente hacer `git push origin QA` directo, GitHub lo va a rechazar y va a forzar el camino seguro del Pull Request.
