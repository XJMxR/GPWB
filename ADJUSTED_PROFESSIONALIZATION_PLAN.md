# Plan de Profesionalizacion Ajustado: RepoCanvas (GPWB)

> **Objetivo**: Mejorar la estructura y calidad del proyecto manteniendo compatibilidad con GitHub Pages, sin testing, con tokens en memoria, logging basico, versionado manual y dependencias manuales.

---

## Cambios respecto al plan original

| Area | Plan Original | Plan Ajustado |
|------|---------------|---------------|
| Testing | Unit + Integration + E2E + Contract | **NINGUNO** |
| CI/CD | Pipeline completo | **Solo deploy a GitHub Pages** (ya existe) |
| Calidad Codigo | Analyzers, StyleCop, SonarCloud | **Nullable enabled, ImplicitUsings** (ya en csproj) |
| Seguridad | Security scanning, CSP, Sentry | **Token en memoria (ya implementado)** |
| Observabilidad | Serilog, OpenTelemetry, Health checks | **Logging basico con ILogger** |
| Documentacion | ADRs, API docs, runbooks | **NINGUNA** |
| Versionado | SemVer automatizado | **Manual** |
| Dependencias | Dependabot/Renovate | **Manual** |
| Arquitectura | Clean Architecture completa | **Separacion basica en proyectos** |

---

## Estructura Final Objetivo

```
GPWB.sln
|-- src/
|   |-- GPWB.Core/           # Domain: Entities, Interfaces, ValueObjects, Events
|   |-- GPWB.Application/    # Use Cases: Services, DTOs, Mappings
|   |-- GPWB.Infrastructure/ # External: GitHub API Client, Cache, Storage, Logging
|   |-- GPWB.Web/            # Presentation: Blazor WASM (proyecto actual movido)
|   |-- GPWB.Shared/         # Shared: Common types, constants
|-- .github/workflows/       # Solo deploy-pages.yml (ya existe)
|-- Directory.Build.props    # Configuracion centralizada basica
|-- global.json              # SDK .NET 10.x
|-- README.md                # Ya existe
```

---

## Fases Simplificadas

### FASE 1: Estructura Multi-Proyecto y Configuracion Base (Inmediato)

1. **Crear GPWB.sln** en raiz
2. **Crear 5 proyectos** en src/
3. **Mover codigo existente** a GPWB.Web (refactor progresivo)
4. **Directory.Build.props** con configuracion basica
5. **global.json** fijando SDK .NET 10.x
6. **Mantener deploy-pages.yml** sin cambios

### FASE 2: Separacion de Capas (Clean Architecture Ligera)

**GPWB.Core (Domain)**
- Entities: Repository, Issue, Commit, Branch, Fork (mover de Models)
- Interfaces: IGitHubApiClient, ICacheProvider, ICanvasStorage
- Events: RepositoryLoadedEvent, TokenConnectedEvent, CanvasAnnotationSavedEvent

**GPWB.Application (Use Cases)**
- Features: LoadRepository, ConnectGitHub, ManageAnnotations, CanvasState
- DTOs y Mapping

**GPWB.Infrastructure (External Concerns)**
- GitHub: GitHubApiClient (mover de Services), Models, Mappers
- Cache: MemoryCacheProvider
- Storage: LocalStorageCanvasRepository
- Logging: BasicLoggerConfiguration

**GPWB.Web (Presentation - Blazor WASM)**
- Components (actuales en Components/)
- Pages (actuales en Pages/)
- State: RepositoryState, CanvasState, AuthState

**GPWB.Shared**
- Tipos comunes, constantes, extensiones

### FASE 3: Logging Basico

- ILogger en Infrastructure
- Configuracion simple en Program.cs
- Logs en consola del navegador

### FASE 4: Validacion y Build

- Verificar que compila
- Verificar que deploy a GitHub Pages funciona
- Verificar que la app funciona igual

---

## Checklist de Validacion

- [ ] GPWB.sln existe y compila
- [ ] 5 proyectos en solucion
- [ ] Directory.Build.props funcionando
- [ ] global.json en raiz
- [ ] Build pasa localmente
- [ ] Deploy a GitHub Pages funciona
- [ ] App funciona igual que antes
- [ ] Logging basico visible en consola

---

## Primeros Pasos Inmediatos

```bash
# 1. Crear solution file
dotnet new sln -n GPWB

# 2. Crear estructura de carpetas
mkdir -p src/GPWB.Core src/GPWB.Application src/GPWB.Infrastructure src/GPWB.Shared

# 3. Mover proyecto actual a src/GPWB.Web (renombrar carpeta)

# 4. Crear proyectos de classlib para Core, Application, Infrastructure, Shared

# 5. Anadir Directory.Build.props y global.json

# 6. Agregar proyectos a la solucion

# 7. Mover codigo progresivamente
```