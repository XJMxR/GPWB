# Plan de Profesionalizacion: RepoCanvas (GPWB) 
> **Objetivo**: Transformar el proyecto de un nivel *aficionado/hobby* a un nivel *profesional/empresarial* listo para produccion, mantenimiento a largo plazo y escalabilidad.

---

## Estado Actual vs. Objetivo

| Area | Actual (Aficionado) | Objetivo (Profesional) |
|------|---------------------|------------------------|
| Estructura | Proyecto unico (monolito) | Multi-proyecto (Core, Web, Tests, Shared) |
| Testing | Ninguno | Unit + Integration + E2E + Contract |
| CI/CD | Solo deploy a GitHub Pages | Pipeline completo: Build -> Test -> Analyze -> Deploy |
| Calidad Codigo | Solo nullable enabled | Analyzers, StyleCop, SonarCloud, Coverage gates |
| Seguridad | Token en memoria | Security scanning, dependency audit, CSP |
| Observabilidad | Ninguna | Logging estructurado, metricas, error tracking |
| Documentacion | README basico | ADRs, API docs, runbooks, contrib guide |
| Versionado | Manual | SemVer automatizado, changelog, releases |
| Dependencias | Manual | Dependabot/Renovate, policy de actualizaciones |
| Arquitectura | Componentes acoplados | Clean Architecture, CQRS, Domain Events |

---

## Fases del Plan

### FASE 1: Fundamentos (Semanas 1-2)

#### 1.1 Estructura Multi-Proyecto
```
GPWB.sln
|-- src/
|   |-- GPWB.Core/
|   |-- GPWB.Application/
|   |-- GPWB.Infrastructure/
|   |-- GPWB.Web/
|   |-- GPWB.Shared/
|-- tests/
|   |-- GPWB.Core.Tests/
|   |-- GPWB.Application.Tests/
|   |-- GPWB.Infrastructure.Tests/
|   |-- GPWB.Web.Tests/
|-- .github/workflows/
|-- Directory.Build.props
|-- Directory.Packages.props
|-- nuget.config
|-- global.json
|-- README.md
```

#### 1.2 Archivos de Configuracion Base

**Directory.Build.props**:
```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <CentralPackageVersionsFile>$(MSBuildThisFileDirectory)Directory.Packages.props</CentralPackageVersionsFile>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <AnalysisLevel>latest-all</AnalysisLevel>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>
</Project>
```

**Directory.Packages.props**: Versiones centralizadas para testing, analyzers, infrastructure, observability.

#### 1.3 Solution File & Global Config
- Crear GPWB.sln en raiz
- global.json fijando SDK .NET 10.x
- nuget.config con fuentes oficiales
- .editorconfig para consistencia cross-IDE

---

### FASE 2: Testing & Calidad (Semanas 2-3)

#### 2.1 Estrategia de Testing (Piramide)

| Nivel | Herramienta | Que testea | Cobertura objetivo |
|-------|-------------|------------|-------------------|
| Unit | xUnit + Moq | Domain logic, Services, Converters | 85%+ |
| Integration | xUnit + WireMock | GitHub API client, HttpClient pipeline | 70%+ |
| Component | bUnit | Blazor Components (render, interactions) | 60%+ |
| E2E | Playwright | Flujos criticos usuario (load repo, auth, canvas) | 5-10 scenarios |
| Contract | Pact / Schemas | GitHub API response schemas | 100% endpoints |

#### 2.2 Quality Gates (SonarCloud)
- Coverage >= 70%
- Duplicated lines < 3%
- Maintainability Rating A
- Reliability Rating A
- Security Rating A
- Zero Blocker/Critical issues

#### 2.3 Analyzers Obligatorios
StyleCop.Analyzers, SonarAnalyzer.CSharp, Meziantou.Analyzer, Roslynator.Analyzers, ErrorProne.NET
---

### FASE 3: CI/CD Pipeline (Semanas 3-4)

#### 3.1 Pipeline CI (.github/workflows/ci.yml)
- Build, restore, analyze en cada PR
- Test con coverage (XPlat Code Coverage)
- SonarCloud scan
- Security scanning: CodeQL + Trivy
- Dependency review action

#### 3.2 Pipeline CD (.github/workflows/cd.yml)
- Deploy automatico a GitHub Pages en merge a main
- Preview deployments en PRs
- Rollback automatico en fallo de health checks

#### 3.3 Dependabot (.github/dependabot.yml)
- Actualizaciones semanales agrupadas por categoria
- Limites de PRs abiertos
- Labels y commit messages estandarizados

---

### FASE 4: Arquitectura & Clean Code (Semanas 4-6)

#### 4.1 Separacion en Capas (Clean Architecture)

**GPWB.Core (Domain)**
- Entities: Repository, Issue, Commit, Branch, Fork
- ValueObjects: RepositoryId, GitHubToken, RateLimit
- Interfaces: IGitHubApiClient, ICacheProvider, ICanvasStorage
- Events: RepositoryLoadedEvent, TokenConnectedEvent, CanvasAnnotationSavedEvent
- Exceptions: RepositoryNotFoundException, RateLimitExceededException
- Specifications: PublicRepositorySpec

**GPWB.Application (Use Cases)**
- Features: LoadRepository, ConnectGitHub, ManageAnnotations, CanvasState
- Common/Behaviors: LoggingBehavior, ValidationBehavior, CachingBehavior, RetryBehavior
- DTOs y Mapping (AutoMapper)

**GPWB.Infrastructure (External Concerns)**
- GitHub: GitHubApiClient, Models, Mappers, Policies (Retry, Circuit Breaker)
- Cache: MemoryCacheProvider, BrowserStorageCacheProvider
- Storage: LocalStorageCanvasRepository
- Logging: SerilogConfiguration
- Telemetry: OpenTelemetryConfiguration

**GPWB.Web (Presentation - Blazor WASM)**
- Components (dumb): Canvas, Sidebar, Toolbar, Dialogs
- Pages (smart)
- State: RepositoryState, CanvasState, AuthState (Flux/Redux o Mediator)
- Services: RepositoryService, AnnotationService, TokenService

#### 4.2 Patrones Clave
- CQRS + Mediator (MediatR) en Application
- Repository Pattern para abstraer datos
- Result Pattern para manejo errores sin exceptions
- Domain Events para side effects
- Options Pattern para configuracion tipada

#### 4.3 MediatR Pipeline Behaviors
Orden: Logging -> Validation -> Caching -> Retry
---

### FASE 5: Observabilidad & Resiliencia (Semanas 6-7)

#### 5.1 Logging Estructurado (Serilog)
Configuracion en Infrastructure/Logging/SerilogConfiguration.cs con enriquecimiento de contexto, output template estructurado, y sinks para browser console.

#### 5.2 Resiliencia (Polly)
- Retry policy con exponential backoff para transient errors y rate limiting (429)
- Circuit breaker tras 5 fallos, 30s de break
- Timeout policies por request

#### 5.3 Health Checks & Metricas
- Health checks para GitHub API y Browser Storage
- OpenTelemetry tracing con HttpClient instrumentation
- Metricas custom: repos loaded, annotations saved, API calls

#### 5.4 Error Tracking (Sentry)
Integracion Sentry con DSN en configuracion, environment tagging, traces sample rate 10%, stacktrace attachment.

---

### FASE 6: Seguridad & Compliance (Semanas 7-8)

#### 6.1 Content Security Policy (CSP)
Meta tag en index.html con directivas restrictivas: default-src self, script-src self unsafe-inline wasm-unsafe-eval, connect-src self api.github.com, etc.

#### 6.2 Security Headers
Configurar en hosting (GitHub Pages, Cloudflare, Netlify): X-Frame-Options, X-Content-Type-Options, Referrer-Policy, Permissions-Policy.

#### 6.3 Token Handling Seguro
- Ya esta en memoria (no localStorage) - BUENO
- Anadir: Validacion de scopes del token (public_repo read-only)
- Anadir: Auto-expiracion y refresh silencioso
- Anadir: Audit log de uso de token

#### 6.4 Dependency Scanning
- Trivy en CI (ya incluido)
- dotnet list package --vulnerable en CI
- Politica: CVE Critical/High -> bloquear merge

#### 6.5 Secrets Management
- GitHub Secrets para: SonarCloud token, Sentry DSN, etc.
- NUNCA commitear secrets
- Usar dotnet user-secrets en desarrollo local

---

### FASE 7: Documentacion & DX (Semanas 8-9)

#### 7.1 Architecture Decision Records (ADRs)
docs/adr/ con decisiones: 0001-use-blazor-wasm, 0002-clean-architecture, 0003-cqrs-mediator, 0004-github-api-direct, 0005-no-backend, 0006-local-storage-annotations

#### 7.2 Documentacion Tecnica
| Documento | Ubicacion | Audiencia |
|-----------|-----------|-----------|
| Architecture Overview | docs/architecture.md | Devs, Architects |
| API Contracts | docs/api/ (OpenAPI) | Consumers |
| Component Library | Storybook / docs/components/ | UI Devs |
| Runbooks | docs/runbooks/ | On-call, SRE |
| Contributing Guide | CONTRIBUTING.md | Contributors |
| Code of Conduct | CODE_OF_CONDUCT.md | Community |

#### 7.3 Developer Experience
- Dev Container (.devcontainer/devcontainer.json) - VS Code Codespaces ready
- Pre-commit hooks (Husky + lint-staged) - Formato, tests rapidos
- Makefile / justfile - Comandos comunes (make test, make build, make lint)
- VS Code Settings (.vscode/settings.json) - Format on save, rulers, etc.

---

### FASE 8: Release & Versioning (Semanas 9-10)

#### 8.1 Semantic Release Automatizado
GitHub Action con semantic-release: analiza conventional commits, genera version, changelog, GitHub Release, publica a NuGet si aplica.

#### 8.2 Versionado
- GitVersion o semantic-release para versionado automatico
- Changelog generado automaticamente (CHANGELOG.md)
- Git tags: v1.2.3
- GitHub Releases con assets (binarios, SBOM)

#### 8.3 SBOM (Software Bill of Materials)
Generacion con anchore/sbom-action en formato SPDX, subida como artifact y a GitHub Dependency Graph.
---

### FASE 9: Performance & Accessibility (Semanas 10-11)

#### 9.1 Performance Budgets
```json
{ "budgets": [
  { "metric": "total-bundle-size", "threshold": "2MB" },
  { "metric": "wasm-size", "threshold": "1.5MB" },
  { "metric": "css-size", "threshold": "100KB" },
  { "metric": "js-size", "threshold": "50KB" },
  { "metric": "fcp", "threshold": "1500ms" },
  { "metric": "tti", "threshold": "3000ms" },
  { "metric": "cls", "threshold": "0.1" }
]}
```

#### 9.2 Performance Testing en CI
- Lighthouse CI en PRs
- Bundle size tracking
- WASM size monitoring

#### 9.3 Accessibility (a11y)
- axe-core en tests E2E (Playwright)
- CI check automatizado
- WCAG 2.1 AA compliance
- Keyboard navigation testing
- Screen reader testing (NVDA/VoiceOver)

---

### FASE 10: Escalabilidad Futura (Opcional, Continuo)

| Mejora | Esfuerzo | Valor |
|--------|----------|-------|
| PWA (Service Worker, Offline) | Medio | Alto |
| Backend opcional (BFF para rate limits) | Alto | Medio |
| Multi-repo dashboard | Alto | Alto |
| Plugin system (extensibilidad) | Muy Alto | Medio |
| Real-time (SignalR/WebSockets) | Alto | Bajo |
| i18n (localizacion) | Medio | Medio |

---

## Checklist de Validacion por Fase

### FASE 1 Completada Cuando:
- [ ] GPWB.sln existe y compila
- [ ] 5+ proyectos en solucion
- [ ] Directory.Build.props y Directory.Packages.props funcionando
- [ ] global.json, nuget.config, .editorconfig en raiz
- [ ] Build pasa en CI

### FASE 2 Completada Cuando:
- [ ] Tests unitarios > 85% cobertura en Core/Application
- [ ] Tests de integracion para GitHub API client
- [ ] Tests de componentes con bUnit
- [ ] Quality Gate SonarCloud pasando
- [ ] Analyzers sin warnings en build

### FASE 3 Completada Cuando:
- [ ] CI pipeline corre en cada PR
- [ ] Security scanning (CodeQL + Trivy) pasando
- [ ] Dependabot configurado y creando PRs
- [ ] Deploy a staging en PRs
- [ ] Deploy a production en merge a main

### FASE 4 Completada Cuando:
- [ ] Clean Architecture implementada
- [ ] CQRS + MediatR funcionando
- [ ] Domain events para side effects
- [ ] Result pattern para manejo errores
- [ ] Zero acoplamiento Web <-> Infrastructure

### FASE 5 Completada Cuando:
- [ ] Logging estructurado en toda la app
- [ ] Resiliencia (retry, circuit breaker) en HTTP calls
- [ ] Health checks expuestos
- [ ] OpenTelemetry traces visibles
- [ ] Error tracking (Sentry) recibiendo eventos

### FASE 6 Completada Cuando:
- [ ] CSP header configurado
- [ ] Dependency scanning bloquea CVEs criticos
- [ ] Token handling auditado
- [ ] Secrets management correcto

### FASE 7 Completada Cuando:
- [ ] ADRs documentando decisiones clave
- [ ] CONTRIBUTING.md, CODE_OF_CONDUCT.md
- [ ] Dev container funcionando
- [ ] Pre-commit hooks activos

### FASE 8 Completada Cuando:
- [ ] Semantic release automatizado
- [ ] Changelog generado automaticamente
- [ ] SBOM generado en cada release
- [ ] Versionado SemVer estricto

### FASE 9 Completada Cuando:
- [ ] Performance budgets en CI
- [ ] Lighthouse CI > 90 en todas las metricas
- [ ] Accesibilidad WCAG 2.1 AA verificada
- [ ] Bundle size < 2MB inicial

---

## Priorizacion Recomendada (MVP Profesional)

Si el tiempo es limitado, enfocate en este orden:

1. **Tests + CI** (Fases 1-3) - Fundacion de confianza
2. **Clean Architecture** (Fase 4) - Mantenibilidad
3. **Observabilidad + Resiliencia** (Fase 5) - Operabilidad
4. **Seguridad** (Fase 6) - Compliance
5. **Documentacion + DX** (Fase 7) - Onboarding
6. **Release Automation** (Fase 8) - Delivery
7. **Performance + a11y** (Fase 9) - Calidad UX

---

## Estimacion de Esfuerzo

| Fase | Semanas | Complejidad | Riesgo |
|------|---------|-------------|--------|
| 1. Fundamentos | 2 | Media | Bajo |
| 2. Testing | 2 | Alta | Medio |
| 3. CI/CD | 1-2 | Media | Bajo |
| 4. Arquitectura | 3 | Muy Alta | Alto |
| 5. Observabilidad | 2 | Media | Bajo |
| 6. Seguridad | 2 | Media | Medio |
| 7. Documentacion | 2 | Baja | Bajo |
| 8. Release | 1-2 | Media | Bajo |
| 9. Perf/a11y | 2 | Media | Medio |
| TOTAL | 17-20 | | |

---

## Primeros Pasos Inmediatos (Esta Semana)

```bash
# 1. Crear solution file
dotnet new sln -n GPWB

# 2. Crear estructura de carpetas
mkdir -p src/GPWB.Core src/GPWB.Application src/GPWB.Infrastructure src/GPWB.Shared
mkdir -p tests/GPWB.Core.Tests tests/GPWB.Application.Tests tests/GPWB.Infrastructure.Tests tests/GPWB.Web.Tests

# 3. Mover codigo existente a GPWB.Web (refactor progresivo, no big bang)

# 4. Anadir Directory.Build.props y Directory.Packages.props

# 5. Configurar CI basico (build + test)

# 6. Anadir primer test unitario (ej: GitHubProjectService)
```

---

## Referencias y Recursos

- Clean Architecture .NET: https://github.com/jasontaylordev/CleanArchitecture
- Blazor WASM Best Practices: https://learn.microsoft.com/aspnet/core/blazor/
- MediatR Pipeline Behaviors: https://github.com/jbogard/MediatR/wiki/Behaviors
- Polly Resilience: https://github.com/App-vNext/Polly
- Semantic Release: https://semantic-release.gitbook.io/
- SonarCloud for .NET: https://docs.sonarcloud.io/
- OWASP Blazor Security: https://cheatsheetseries.owasp.org/cheatsheets/Blazor_Security_Cheat_Sheet.html

---

> Nota: Este plan es iterativo. Cada fase entrega valor incremental. No intentes hacerlo todo a la vez. Prioriza segun las necesidades reales del proyecto y del equipo.
