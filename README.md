# RepoCanvas

Tablero visual e interactivo para explorar la información pública de un
repositorio de GitHub. Es una aplicación estática de **Blazor WebAssembly**:
la interfaz, la composición del tablero y el cliente de GitHub están escritos
en C# y se ejecutan en el navegador, sin servidor propio.

## Funcionalidades

- Carga los datos de un repositorio público con el formato `usuario/repositorio`.
- Presenta un resumen del proyecto, lenguajes, issues abiertos y commits recientes.
- Dibuja una red radial con hasta 30 forks públicos recientes; cada nodo abre
  el fork correspondiente en GitHub. La vista ampliada incluye una lista con
  actividad, estrellas y enlaces directos.
- Compara hasta seis ramas públicas, con sus diez commits más recientes y enlaces
  a los commits y al historial de cada rama. Prioriza la rama predeterminada y
  ordena las demás por fecha del commit más reciente.
- Expande los paneles de forks, issues, actividad y ramas para ocupar el lienzo
  completo. La vista de issues muestra etiquetas, autor, comentarios y texto;
  los enlaces permiten continuar directamente en GitHub.
- Grafica la actividad reciente como un árbol de commits por rama, con enlaces a
  cada commit y a los commits padres disponibles.
- Permite arrastrar tarjetas, desplazar el lienzo y acercar o alejar la vista.
- Muestra una vista previa de las dos ramas principales; la vista ampliada
  permite revisar todas las ramas cargadas y su actividad.
- Permite dibujar trazos y añadir notas en el lienzo. Las anotaciones se guardan
  localmente en el navegador por repositorio y pueden deshacerse o eliminarse.
- Muestra las consultas anónimas realizadas durante la sesión y el límite
  restante que devuelve la API de GitHub.
- Permite conectar un token fine-grained de GitHub para aumentar el límite de
  solicitudes. El token se mantiene solo en memoria; no se guarda en el
  navegador ni se envía a un servidor de RepoCanvas.

La visualización es de solo lectura y consulta exclusivamente repositorios,
forks, ramas y commits públicos. Los forks del canvas son los más recientes
devueltos por la primera página de la API; no representan todos los forks de
repositorios grandes. Se consultan hasta 100 ramas y se muestran hasta seis,
priorizando la rama predeterminada. El tablero carga hasta 100 issues, 30
commits de la rama predeterminada, 10 commits por cada rama seleccionada y 30
forks para el diagrama. Estas vistas no reemplazan el historial completo de
GitHub.

La app consulta directamente la API de GitHub desde el navegador y funciona
sin token. Las consultas anónimas están sujetas al límite compartido por IP;
la conexión opcional usa un token fine-grained, idealmente limitado a
**Public repositories (read-only)**, para acceder al límite autenticado de la
cuenta (normalmente hasta 5.000 solicitudes por hora). Los límites secundarios
de GitHub siguen aplicando. El contador anónimo solo refleja las solicitudes
hechas en la pestaña actual y durante la sesión en curso.

Para conectar una cuenta, crea un token personal fine-grained en GitHub y
selecciona únicamente acceso de lectura a repositorios públicos. El token se
envía desde el navegador directamente a `api.github.com`, nunca a un backend,
y se borra al desconectar o recargar la página. RepoCanvas valida que cada
repositorio consultado sea público antes de pedir sus issues, forks, ramas o
commits; no se debe usar un token con permisos de escritura.

## Arquitectura

- `src/GPWB/Pages`: interfaz y coordinación de la carga del repositorio.
- `src/GPWB/Components`: lienzo y tarjetas, aislados de la API de GitHub.
- `src/GPWB/Services`: contrato y cliente HTTP para obtener datos.
- `src/GPWB/Models`: modelos de datos del proyecto.

El servicio se registra detrás de `IGitHubProjectService`, de modo que la
interfaz y el lienzo no dependen directamente de HTTP ni de la API externa.
La posición del tablero es interactiva; los trazos y notas se guardan en el
almacenamiento local del navegador y no se envían a GitHub.

## Desarrollo local

Requisitos: .NET 10 SDK.

```powershell
dotnet run --project src\GPWB\GPWB.csproj
```

## GitHub Pages

El flujo `.github/workflows/deploy-pages.yml` publica la aplicación cuando hay
cambios en `main` o al iniciarlo manualmente. En el repositorio, configura
**Settings → Pages → Build and deployment → Source: GitHub Actions**. El `base`
relativo permite servir la app desde el subdirectorio del proyecto en GitHub
Pages.

Para compilar la versión estática localmente:

```powershell
dotnet publish src\GPWB\GPWB.csproj --configuration Release
```

## Licencia

Este proyecto está licenciado bajo la **GNU General Public License v3.0**.
Consulta el archivo [LICENSE](LICENSE) para más detalles.
