# Book Highlights

Aplicação mobile-first para salvar e manter destaques de livros físicos com OCR.

## Stack Tecnológica

- **Backend**: ASP.NET Core 8 MVC
- **Frontend**: HTMX, Alpine.js, Tailwind CSS, daisyUI
- **Banco de Dados**: SQLite (via Entity Framework Core)
- **Diferencial UX**: Three.js para animações 3D
- **PWA**: Service Worker para instalação e cache offline

## Funcionalidades

### Principais
- ✅ Cadastro rápido de livros
- ✅ Captura de trechos via câmera do dispositivo
- ✅ Upload de imagens dos trechos capturados
- ✅ Armazenamento local com SQLite
- ✅ Interface mobile-first responsiva
- ✅ PWA instalável no dispositivo
- ✅ Animações 3D com Three.js

### Em Desenvolvimento
- 🔄 OCR automático para extração de texto das imagens
- 🔄 Busca full-text nos destaques
- 🔄 Exportação de destaques (PDF, Markdown)
- 🔄 Sincronização em nuvem

## Estrutura do Projeto

```
BookHighlights/
├── Controllers/          # Controllers MVC
│   ├── BooksController.cs
│   └── HighlightsController.cs
├── Domain/
│   └── Entities/         # Entidades do domínio
│       ├── Book.cs
│       └── Highlight.cs
├── Infrastructure/
│   ├── Persistence/      # DbContext EF Core
│   └── Repositories/     # Repositórios
├── Application/
│   ├── Services/         # Serviços de negócio
│   └── DTOs/             # Data Transfer Objects
├── Views/
│   ├── Books/            # Views de livros
│   └── Shared/           # Layouts e partials
├── wwwroot/
│   ├── css/              # Estilos Tailwind
│   ├── js/               # JavaScript
│   ├── uploads/          # Imagens dos destaques
│   ├── icons/            # Ícones PWA
│   ├── manifest.json     # Manifest PWA
│   └── sw.js             # Service Worker
└── Data/
    └── bookhighlights.db # Banco SQLite (gerado automaticamente)
```

## Como Rodar

### Pré-requisitos
- .NET 8 SDK
- Node.js (para build do Tailwind CSS)

### Passos

1. **Instalar dependências do frontend**
```bash
cd BookHighlights
npm install
```

2. **Build do CSS**
```bash
npm run build:css
```

3. **Rodar a aplicação**
```bash
dotnet run
```

4. **Acessar**
Abra o navegador em `http://localhost:5000`

## Uso no Mobile

1. Acesse a aplicação pelo navegador do seu dispositivo
2. Toque em "Adicionar à Tela Inicial" ou "Instalar aplicativo"
3. A aplicação funcionará como um app nativo
4. Use a câmera para capturar trechos dos seus livros

## Diferenciais UX com Three.js

A aplicação inclui animações 3D sutis usando Three.js:
- Livros flutuantes na tela inicial
- Transições suaves entre páginas
- Feedback visual interativo

## Próximos Passos - OCR

Para implementar OCR automático:

1. **Client-side (recomendado)**: Usar Tesseract.js
```javascript
import Tesseract from 'tesseract.js';

Tesseract.recognize(
  imageFile,
  'por', // Português
  { logger: m => console.log(m) }
).then(({ data: { text } }) => {
  console.log(text);
});
```

2. **Server-side**: Usar biblioteca como Tesseract ou Azure Computer Vision

## Licença

MIT
