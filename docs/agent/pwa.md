# PWA

## Visao Geral

A aplicacao deve possuir suporte padrao a PWA seguindo o principio de **progressive enhancement**, sem adicionar complexidade desnecessaria.

O objetivo do PWA nesta stack e melhorar a experiencia do usuario, nao transformar a aplicacao em um sistema offline-first complexo.

## Objetivos

- permitir instalacao da aplicacao no dispositivo
- melhorar desempenho com cache basico
- suportar uso limitado offline quando fizer sentido
- melhorar experiencia em redes instaveis
- fornecer sensacao de app nativo leve
- manter simplicidade arquitetural

## Componentes Obrigatorios

Toda aplicacao deve incluir:

1. `manifest.json`
2. `service worker`
3. registro do `service worker`
4. icones da aplicacao
5. meta tags PWA no layout

## Manifest

Deve existir um arquivo `manifest.json` configurado com:

- `name`
- `short_name`
- `start_url`
- `display`, preferencialmente `standalone`
- `background_color`
- `theme_color`
- `icons` em multiplos tamanhos

O manifest deve ser referenciado no `_Layout.cshtml`.

## Service Worker

A aplicacao deve incluir um `service worker` simples e previsivel.

### Estrategia Padrao

Usar estrategias basicas de cache:

- `cache-first` para assets estaticos
- `network-first` para paginas dinamicas
- fallback simples para offline quando aplicavel

### O service worker nao deve

- implementar logica complexa de sincronizacao offline
- armazenar dados sensiveis
- replicar regras de negocio
- se tornar dificil de manter

## Registro do Service Worker

O registro deve ser feito no layout principal, de forma simples:

- somente em ambiente de producao, ou de forma controlada
- com fallback seguro caso nao seja suportado

## Meta Tags e Integracao

O layout deve conter:

- `<link rel="manifest" href="/manifest.json">`
- `<meta name="theme-color">`
- `<meta name="apple-mobile-web-app-capable">`
- `<meta name="apple-mobile-web-app-status-bar-style">`
- `<link rel="apple-touch-icon">`

## Estrategia de Offline

O suporte offline deve ser limitado e previsivel.

### Regras

- paginas principais podem ser cacheadas
- assets devem estar disponiveis offline
- exibir fallback amigavel quando offline
- nao prometer funcionalidade offline completa sem necessidade real

## UX

A aplicacao deve considerar:

- `loading states` mais rapidos com cache
- mensagens amigaveis quando offline
- comportamento consistente mesmo com falhas de rede
- possibilidade de instalacao com Add to Home Screen

## Integracao com HTMX

Ao usar HTMX:

- requisicoes continuam sendo feitas ao servidor normalmente
- o service worker pode interceptar requisicoes apenas para cache basico
- nao tentar gerenciar estado HTMX no service worker
- evitar complexidade ao combinar HTMX com cache

## Diretriz de Simplicidade

O suporte a PWA deve:

- ser leve
- ser previsivel
- nao introduzir complexidade desnecessaria
- nao dificultar debugging
- nao comprometer a arquitetura server-side

Se houver duvida, preferir uma implementacao mais simples.

## Estrutura de Arquivos

```text
wwwroot/
  manifest.json
  sw.js
  icons/
    icon-192.png
    icon-512.png
```
