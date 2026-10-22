# RepoCanvas

Tablero visual e interactivo para explorar la información pública de un
repositorio de GitHub. Es una aplicación estática de **Blazor WebAssembly**:
la interfaz, la composición del tablero y el cliente de GitHub están escritos
en C# y se ejecutan en el navegador, sin servidor propio.

## Funcionalidades

- Carga los datos de un repositorio público con el formato `usuario/repositorio`.
- Presenta un resumen del proyecto, lenguajes, issues abiertos y commits recientes.
- Permite arrastrar tarjetas, desplazar el lienzo y acercar o alejar la vista.
- Consulta la API pública de GitHub directamente desde el navegador. No requiere
  un token ni guarda credenciales.

Las consultas anónimas están sujetas al límite de solicitudes de GitHub. No se
envían datos a un backend.

## Arquitectura

- `src/GPWB/Pages`: interfaz y coordinación de la carga del repositorio.
- `src/GPWB/Components`: lienzo y tarjetas, aislados de la API de GitHub.
- `src/GPWB/Services`: contrato y cliente HTTP para obtener datos.
- `src/GPWB/Models`: modelos de datos del proyecto.

El servicio se registra detrás de `IGitHubProjectService`, de modo que la
interfaz y el lienzo no dependen directamente de HTTP ni de la API externa.

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
