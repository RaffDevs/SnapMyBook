# Stack

## Filosofia

Esta stack deve ser usada com mentalidade **server-render** e **progressive enhancement**.

## Principios Fundamentais

1. O servidor e a fonte principal de renderizacao da UI.
2. HTMX e usado para interacoes incrementais e atualizacao parcial de tela.
3. Alpine.js e usado para adicionar comportamentos no client.
4. A meta e entregar interatividade comparavel a uma SPA sem adotar tecnologias SPA.
5. O dominio e a regra de negocio devem ficar no backend.
6. JavaScript customizado quando necessario.
7. A UI deve ser organizada em componentes e partials reutilizaveis.
8. A estilizacao deve usar Tailwind via npm.
9. Os componentes visuais devem priorizar daisyUI via npm.
10. A arquitetura deve crescer sem acoplar controller, view e regra de negocio.
11. O projeto deve ser facil de entender por outro desenvolvedor.

## Tipo de Aplicacao Alvo

Este agente e adequado principalmente para:

- sistemas administrativos
- CRUDs
- dashboards internos
- portais corporativos
- aplicacoes de backoffice
- sistemas com autenticacao
- fluxos de negocio
- formularios
- listas, filtros, paginacao e busca
- cadastros
- workflows simples e moderados
- areas autenticadas
- aplicacoes orientadas a produtividade
- jogos simples

## Objetivo do Agente

O agente deve ser capaz de:

- criar novos projetos
- evoluir projetos existentes
- organizar a solucao de forma consistente
- implementar funcionalidades completas de ponta a ponta
- criar views, partials, controllers, servicos e modelos
- estruturar layout, navegacao e componentes visuais
- aplicar boas praticas de UX
- implementar formularios e validacoes
- implementar paginas com HTMX
- aplicar Alpine.js para adicionar interatividade local e global
- utilizar Tailwind e daisyUI de forma consistente via npm
- garantir sinergia entre scripts do .NET e `package.json`
- manter padroes arquiteturais claros
- gerar codigo limpo, objetivo e de facil manutencao
- documentar decisoes importantes quando necessario
