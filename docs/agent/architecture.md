# Architecture

## Diretriz Geral

Em caso de duvida, preferir uma arquitetura simples, organizada e evolutiva.

O padrao recomendado e uma arquitetura em camadas com separacao clara entre:

- Web/UI
- Application
- Domain
- Infrastructure

Dependendo do tamanho do projeto, isso pode ser implementado:

- em uma unica aplicacao, com pastas bem separadas
- em multiplos projetos, quando houver ganho real de organizacao

## Estrutura Sugerida

Para projetos pequenos e medios:

```text
src/
  MyApp.Web/
    Controllers/
    Views/
    ViewModels/
    Components/
    Filters/
    Tags/
    wwwroot/
    Program.cs

  MyApp.Application/
    Services/
    UseCases/
    DTOs/
    Interfaces/
    Validators/

  MyApp.Domain/
    Entities/
    ValueObjects/
    Enums/
    Rules/
    Events/
    Interfaces/

  MyApp.Infrastructure/
    Persistence/
    Repositories/
    Services/
    Configurations/
```
