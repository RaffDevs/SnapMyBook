async function createWorkerModule() {
  const { createWorker } = await import("https://unpkg.com/tesseract.js@5.1.1/dist/tesseract.min.js");
  return createWorker("por");
}

function shouldReduceMotion() {
  return window.matchMedia("(prefers-reduced-motion: reduce)").matches;
}

function hashCode(input) {
  return Array.from(input).reduce((hash, char) => ((hash << 5) - hash) + char.charCodeAt(0), 0);
}

function titleLines(title) {
  const words = (title || "Livro").split(/\s+/).filter(Boolean);
  if (words.length <= 2) {
    return [words.join(" ")];
  }

  const midpoint = Math.ceil(words.length / 2);
  return [words.slice(0, midpoint).join(" "), words.slice(midpoint).join(" ")];
}

function makeCanvasTexture(THREE, canvas) {
  const texture = new THREE.CanvasTexture(canvas);
  texture.colorSpace = THREE.SRGBColorSpace;
  texture.needsUpdate = true;
  return texture;
}

function paletteForBook(book) {
  const palettes = [
    ["#0c7ff2", "#39a0ff", "#dceeff"],
    ["#0f1724", "#26354a", "#dde6f2"],
    ["#0f766e", "#14b8a6", "#daf7f3"],
    ["#8b5cf6", "#c084fc", "#f2e8ff"],
    ["#f97316", "#fdba74", "#fff1df"]
  ];

  return palettes[Math.abs(hashCode(book.title || "livro")) % palettes.length];
}

function createBookFaceCanvas(book, width = 512, height = 768) {
  const canvas = document.createElement("canvas");
  canvas.width = width;
  canvas.height = height;
  const context = canvas.getContext("2d");
  const palette = paletteForBook(book);

  const gradient = context.createLinearGradient(0, 0, width, height);
  gradient.addColorStop(0, palette[0]);
  gradient.addColorStop(1, palette[1]);
  context.fillStyle = gradient;
  context.fillRect(0, 0, width, height);

  context.fillStyle = "rgba(255,255,255,0.12)";
  context.fillRect(width * 0.08, height * 0.06, width * 0.84, height * 0.88);

  context.fillStyle = "#ffffff";
  context.font = "700 52px 'SF Pro Display', sans-serif";
  context.textBaseline = "top";
  titleLines(book.title).slice(0, 3).forEach((line, index) => {
    context.fillText(line, width * 0.12, height * (0.18 + index * 0.1), width * 0.72);
  });

  context.fillStyle = "rgba(255,255,255,0.82)";
  context.font = "500 26px 'SF Pro Display', sans-serif";
  context.fillText(book.author || "Autor indefinido", width * 0.12, height * 0.76, width * 0.72);

  context.strokeStyle = "rgba(255,255,255,0.28)";
  context.lineWidth = 6;
  context.strokeRect(width * 0.08, height * 0.06, width * 0.84, height * 0.88);

  return canvas;
}

function createSpineCanvas(book, width = 160, height = 768) {
  const canvas = document.createElement("canvas");
  canvas.width = width;
  canvas.height = height;
  const context = canvas.getContext("2d");
  const hue = Math.abs(hashCode(book.title || "livro")) % 360;
  const gradient = context.createLinearGradient(0, 0, width, height);
  gradient.addColorStop(0, `hsl(${hue} 80% 40%)`);
  gradient.addColorStop(1, `hsl(${(hue + 35) % 360} 70% 28%)`);
  context.fillStyle = gradient;
  context.fillRect(0, 0, width, height);

  context.save();
  context.translate(width * 0.58, height * 0.86);
  context.rotate(-Math.PI / 2);
  context.fillStyle = "#ffffff";
  context.font = "700 52px 'SF Pro Display', sans-serif";
  context.fillText(book.title || "Livro", 0, 0, height * 0.7);
  context.font = "500 24px 'SF Pro Display', sans-serif";
  context.fillStyle = "rgba(255,255,255,0.76)";
  context.fillText(book.author || "", 0, 64, height * 0.7);
  context.restore();

  return canvas;
}

function createPageCanvas(THREE, width = 96, height = 512) {
  const canvas = document.createElement("canvas");
  canvas.width = width;
  canvas.height = height;
  const context = canvas.getContext("2d");
  context.fillStyle = "#f8f2e8";
  context.fillRect(0, 0, width, height);

  for (let y = 10; y < height; y += 14) {
    context.fillStyle = y % 28 === 0 ? "rgba(169,148,121,0.12)" : "rgba(169,148,121,0.07)";
    context.fillRect(0, y, width, 6);
  }

  return makeCanvasTexture(THREE, canvas);
}

function loadImage(url) {
  return new Promise((resolve, reject) => {
    const image = new Image();
    image.crossOrigin = "anonymous";
    image.onload = () => resolve(image);
    image.onerror = reject;
    image.src = url;
  });
}

function storeOpenedBook(book) {
  try {
    sessionStorage.setItem("snapmybook:last-opened-book", JSON.stringify({
      id: String(book.id ?? ""),
      title: book.title ?? "Livro",
      author: book.author ?? "",
      coverUrl: book.coverUrl ?? "",
      ts: Date.now()
    }));
  } catch {
    // Ignore storage failures.
  }
}

function readOpenedBook() {
  try {
    const raw = sessionStorage.getItem("snapmybook:last-opened-book");
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}

function mountBookReaderTransition() {
  const reader = document.querySelector("[data-book-reader]");
  if (!reader) {
    return;
  }

  const book = {
    id: reader.dataset.bookId,
    title: reader.dataset.bookTitle
  };

  const lastOpenedBook = readOpenedBook();
  if (lastOpenedBook?.id === String(book.id)) {
    sessionStorage.removeItem("snapmybook:last-opened-book");
  }

  requestAnimationFrame(() => {
    reader.classList.add("is-entered");
  });

  reader.querySelectorAll(".js-book-close").forEach((link) => {
    link.addEventListener("click", (event) => {
      event.preventDefault();
      storeOpenedBook(book);
      if (shouldReduceMotion()) {
        window.location.href = link.href;
        return;
      }

      reader.classList.remove("is-entered");
      reader.classList.add("is-closing");
      window.setTimeout(() => {
        window.location.href = link.href;
      }, 360);
    });
  });
}

async function buildCoverTextures(THREE, book) {
  const fallbackCanvas = createBookFaceCanvas(book);
  const frontCanvas = document.createElement("canvas");
  frontCanvas.width = fallbackCanvas.width;
  frontCanvas.height = fallbackCanvas.height;
  const context = frontCanvas.getContext("2d");
  context.drawImage(fallbackCanvas, 0, 0);

  if (book.coverUrl) {
    try {
      const image = await loadImage(book.coverUrl);
      context.clearRect(0, 0, frontCanvas.width, frontCanvas.height);
      context.drawImage(image, 0, 0, frontCanvas.width, frontCanvas.height);
      context.fillStyle = "rgba(12,20,34,0.14)";
      context.fillRect(0, 0, frontCanvas.width, frontCanvas.height);
    } catch {
      // Fall back to generated cover.
    }
  }

  const backCanvas = document.createElement("canvas");
  backCanvas.width = frontCanvas.width;
  backCanvas.height = frontCanvas.height;
  const backContext = backCanvas.getContext("2d");
  backContext.drawImage(fallbackCanvas, 0, 0);
  backContext.fillStyle = "rgba(255,255,255,0.08)";
  backContext.fillRect(0, 0, backCanvas.width, backCanvas.height);

  return {
    front: makeCanvasTexture(THREE, frontCanvas),
    back: makeCanvasTexture(THREE, backCanvas),
    spine: makeCanvasTexture(THREE, createSpineCanvas(book))
  };
}

function buildPageMaterials(THREE, pageTexture, spineTexture, backTexture) {
  return [
    new THREE.MeshPhysicalMaterial({ map: pageTexture, roughness: 0.9, metalness: 0.02 }),
    new THREE.MeshPhysicalMaterial({ map: spineTexture, roughness: 0.46, metalness: 0.08, clearcoat: 0.28 }),
    new THREE.MeshPhysicalMaterial({ map: pageTexture, roughness: 0.92, metalness: 0.02 }),
    new THREE.MeshPhysicalMaterial({ map: pageTexture, roughness: 0.92, metalness: 0.02 }),
    new THREE.MeshPhysicalMaterial({ color: "#f6efe4", roughness: 0.95, metalness: 0.02 }),
    new THREE.MeshPhysicalMaterial({ map: backTexture, roughness: 0.6, metalness: 0.05, clearcoat: 0.18 })
  ];
}

function buildCoverMaterials(THREE, frontTexture) {
  const edge = new THREE.MeshPhysicalMaterial({
    color: "#ebe2d4",
    roughness: 0.82,
    metalness: 0.02
  });

  const inside = new THREE.MeshPhysicalMaterial({
    color: "#fffaf2",
    roughness: 0.9,
    metalness: 0.02
  });

  return [
    edge,
    edge,
    edge,
    edge,
    new THREE.MeshPhysicalMaterial({ map: frontTexture, roughness: 0.42, metalness: 0.08, clearcoat: 0.62 }),
    inside
  ];
}

function createBookObject(THREE, book, textures, pageTexture, widthScale, heightScale, depth) {
  const group = new THREE.Group();
  const coverThickness = 0.04;
  const blockDepth = Math.max(depth - coverThickness, 0.12);

  const body = new THREE.Mesh(
    new THREE.BoxGeometry(widthScale, heightScale, blockDepth),
    buildPageMaterials(THREE, pageTexture, textures.spine, textures.back)
  );
  group.add(body);

  const coverPivot = new THREE.Group();
  coverPivot.position.set(-(widthScale / 2), 0, blockDepth / 2);

  const cover = new THREE.Mesh(
    new THREE.BoxGeometry(widthScale, heightScale, coverThickness),
    buildCoverMaterials(THREE, textures.front)
  );
  cover.position.set(widthScale / 2, 0, coverThickness / 2);
  coverPivot.add(cover);
  group.add(coverPivot);

  const hingeShadow = new THREE.Mesh(
    new THREE.PlaneGeometry(widthScale * 0.96, heightScale * 0.96),
    new THREE.MeshBasicMaterial({ color: "#0c1422", transparent: true, opacity: 0.08 })
  );
  hingeShadow.position.set(0.02, 0, (blockDepth / 2) + 0.003);
  group.add(hingeShadow);

  group.userData.body = body;
  group.userData.coverPivot = coverPivot;
  group.userData.cover = cover;
  group.userData.hingeShadow = hingeShadow;
  group.userData.href = `/books/${book.id}`;
  group.userData.book = book;
  body.userData.root = group;
  cover.userData.root = group;
  hingeShadow.userData.root = group;

  return group;
}

async function mountThreeShelf() {
  const container = document.getElementById("library-three");
  if (!container || shouldReduceMotion()) {
    return;
  }

  const books = JSON.parse(container.dataset.books || "[]");
  if (!books.length) {
    container.innerHTML = "<div class='empty-state'><h3>Sua estante 3D aparece quando existir pelo menos um livro.</h3></div>";
    return;
  }

  const THREE = await import("https://unpkg.com/three@0.179.1/build/three.module.js");
  const scene = new THREE.Scene();
  const height = Math.max(container.clientHeight, 620);
  const camera = new THREE.PerspectiveCamera(38, container.clientWidth / height, 0.1, 100);
  camera.position.set(0, -0.15, 10.8);

  const renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
  renderer.setSize(container.clientWidth, height);
  renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
  container.innerHTML = "";
  container.appendChild(renderer.domElement);
  const veil = document.createElement("div");
  veil.className = "library-three__veil";
  container.appendChild(veil);
  const hint = document.createElement("div");
  hint.className = "library-three__hint";
  hint.textContent = "Arraste para explorar. Toque para abrir um livro.";
  container.appendChild(hint);

  scene.fog = new THREE.Fog("#dde7f0", 12, 24);
  scene.add(new THREE.AmbientLight(0xffffff, 1.8));
  const keyLight = new THREE.DirectionalLight(0xffffff, 2.6);
  keyLight.position.set(4, 6, 10);
  scene.add(keyLight);

  const rimLight = new THREE.PointLight("#c6ecff", 14, 30, 2);
  rimLight.position.set(-5, 2, 6);
  scene.add(rimLight);

  const shelfLevels = [
    { centerY: -0.35, topY: -0.21, lipY: -0.18 },
    { centerY: -3.15, topY: -3.01, lipY: -2.98 }
  ];

  function createShelf(level) {
    const shelf = new THREE.Mesh(
      new THREE.BoxGeometry(17.5, 0.28, 2.4),
      new THREE.MeshPhysicalMaterial({
        color: "#d7dee8",
        roughness: 0.5,
        metalness: 0.08,
        clearcoat: 0.18
      })
    );
    shelf.position.set(0, level.centerY, -0.1);
    scene.add(shelf);

    const lip = new THREE.Mesh(
      new THREE.BoxGeometry(17.7, 0.14, 0.5),
      new THREE.MeshPhysicalMaterial({
        color: "#eef3f8",
        roughness: 0.35,
        metalness: 0.02
      })
    );
    lip.position.set(0, level.lipY, 0.95);
    scene.add(lip);
  }

  shelfLevels.forEach(createShelf);

  const group = new THREE.Group();
  const pageTexture = createPageCanvas(THREE);
  const visibleBooks = books.slice(0, 10);
  const books3d = [];
  const booksPerRow = Math.min(5, visibleBooks.length);

  for (const [index, book] of visibleBooks.entries()) {
    const rowIndex = Math.floor(index / booksPerRow);
    const columnIndex = index % booksPerRow;
    const rowBooks = visibleBooks.slice(rowIndex * booksPerRow, rowIndex * booksPerRow + booksPerRow);
    const rowLevel = shelfLevels[rowIndex] ?? shelfLevels[shelfLevels.length - 1];
    const depth = 0.3 + ((Math.abs(hashCode(book.title || String(index))) % 100) / 100) * 0.28;
    const heightScale = 2.2 + ((Math.abs(hashCode(book.author || String(index))) % 100) / 100) * 0.42;
    const widthScale = 1.52 + ((Math.abs(hashCode(`${book.title}-${index}`)) % 100) / 100) * 0.22;
    const textures = await buildCoverTextures(THREE, book);
    const bookObject = createBookObject(THREE, book, textures, pageTexture, widthScale, heightScale, depth);
    const baseX = (columnIndex - (rowBooks.length - 1) / 2) * 1.95;
    const baseY = rowLevel.topY + (heightScale / 2) + 0.02;
    const baseZ = Math.cos(index * 0.55) * 0.18 + (rowIndex === 0 ? -0.05 : 0.08);
    const baseRotationY = -0.32 + columnIndex * 0.12 + (rowIndex === 0 ? -0.08 : 0.05);
    const baseRotationZ = -0.02 + Math.sin(index * 0.75) * 0.02;
    bookObject.position.set(baseX, baseY, baseZ);
    bookObject.rotation.y = baseRotationY;
    bookObject.rotation.z = baseRotationZ;
    bookObject.userData.baseX = baseX;
    bookObject.userData.baseY = baseY;
    bookObject.userData.baseZ = baseZ;
    bookObject.userData.baseRotationY = baseRotationY;
    bookObject.userData.baseRotationZ = baseRotationZ;
    group.add(bookObject);
    books3d.push(bookObject);
  }

  scene.add(group);

  const raycaster = new THREE.Raycaster();
  const pointer = new THREE.Vector2();
  let hovered = null;
  let dragActive = false;
  let dragOriginX = 0;
  let openingBook = null;
  let navigateAt = 0;

  function animate() {
    if (!openingBook) {
      group.rotation.y *= 0.94;
    }

    books3d.forEach((bookObject) => {
      const isHovered = hovered === bookObject && !openingBook;
      const isOpening = openingBook === bookObject;
      const targetRotationY = isOpening ? 0.08 : bookObject.userData.baseRotationY + (isHovered ? 0.08 : 0);
      const targetRotationZ = isOpening ? 0 : bookObject.userData.baseRotationZ;
      const targetY = isOpening ? bookObject.userData.baseY + 0.14 : (isHovered ? bookObject.userData.baseY + 0.04 : bookObject.userData.baseY);
      const targetZ = isOpening ? 2.9 : bookObject.userData.baseZ + (isHovered ? 0.18 : 0);
      const targetX = isOpening ? 0 : bookObject.userData.baseX;
      const targetScale = isOpening ? 1.18 : (isHovered ? 1.06 : 1);
      const targetCoverRotation = isOpening ? -2.3 : 0;
      const targetShadowOpacity = isOpening ? 0.22 : (isHovered ? 0.12 : 0.08);
      const targetBodyRotationY = isOpening ? 0.1 : 0;

      bookObject.rotation.y += (targetRotationY - bookObject.rotation.y) * 0.08;
      bookObject.rotation.z += (targetRotationZ - bookObject.rotation.z) * 0.08;
      bookObject.position.x += (targetX - bookObject.position.x) * 0.08;
      bookObject.position.y += (targetY - bookObject.position.y) * 0.12;
      bookObject.position.z += (targetZ - bookObject.position.z) * 0.12;
      bookObject.scale.lerp(new THREE.Vector3(targetScale, targetScale, targetScale), 0.12);

      bookObject.userData.coverPivot.rotation.y += (targetCoverRotation - bookObject.userData.coverPivot.rotation.y) * 0.1;
      bookObject.userData.body.rotation.y += (targetBodyRotationY - bookObject.userData.body.rotation.y) * 0.08;
      bookObject.userData.hingeShadow.material.opacity += (targetShadowOpacity - bookObject.userData.hingeShadow.material.opacity) * 0.12;
    });

    if (openingBook && navigateAt && performance.now() >= navigateAt) {
      window.location.href = openingBook.userData.href;
      return;
    }

    renderer.render(scene, camera);
    requestAnimationFrame(animate);
  }

  function updatePointer(event) {
    const rect = renderer.domElement.getBoundingClientRect();
    pointer.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
    pointer.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;
    raycaster.setFromCamera(pointer, camera);
    const hit = raycaster.intersectObjects(group.children, true)[0]?.object ?? null;
    hovered = hit?.userData?.root ?? hit ?? null;
  }

  container.addEventListener("pointermove", (event) => {
    if (openingBook) {
      return;
    }
    updatePointer(event);
    if (dragActive) {
      const delta = (event.clientX - dragOriginX) * 0.0028;
      group.rotation.y += delta;
      dragOriginX = event.clientX;
    }
  });

  container.addEventListener("pointerdown", (event) => {
    if (openingBook) {
      return;
    }
    dragActive = true;
    dragOriginX = event.clientX;
    updatePointer(event);
  });

  window.addEventListener("pointerup", () => {
    dragActive = false;
  });

  container.addEventListener("pointerleave", () => {
    hovered = null;
    dragActive = false;
  });

  container.addEventListener("click", () => {
    if (hovered?.userData?.href && !dragActive && !openingBook) {
      openingBook = hovered;
      storeOpenedBook(openingBook.userData.book ?? {});
      navigateAt = performance.now() + 620;
      container.classList.add("is-opening");
      hint.textContent = "Abrindo livro...";
    }
  });

  window.addEventListener("resize", () => {
    const nextHeight = Math.max(container.clientHeight, 620);
    camera.aspect = container.clientWidth / nextHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(container.clientWidth, nextHeight);
  });

  animate();
}

document.addEventListener("alpine:init", () => {
  Alpine.data("highlightCapture", ({ bookId, reviewUrl, token }) => ({
    bookId,
    reviewUrl,
    token,
    previewImage: "",
    ocrState: "idle",
    errorMessage: "",
    stream: null,

    async startCamera() {
      try {
        this.stream = await navigator.mediaDevices.getUserMedia({
          video: { facingMode: { ideal: "environment" } },
          audio: false
        });
        this.$refs.video.srcObject = this.stream;
        this.errorMessage = "";
      } catch {
        this.errorMessage = "Nao foi possivel acessar a camera. Voce ainda pode escolher uma foto.";
      }
    },

    captureFrame() {
      const video = this.$refs.video;
      if (!video.videoWidth || !video.videoHeight) {
        this.errorMessage = "Ative a camera antes de capturar.";
        return;
      }

      const canvas = this.$refs.canvas;
      canvas.width = video.videoWidth;
      canvas.height = video.videoHeight;
      canvas.getContext("2d").drawImage(video, 0, 0);
      this.previewImage = canvas.toDataURL("image/jpeg", 0.92);
      this.errorMessage = "";
    },

    loadFile(event) {
      const file = event.target.files?.[0];
      if (!file) {
        return;
      }

      const reader = new FileReader();
      reader.onload = () => {
        this.previewImage = reader.result?.toString() || "";
      };
      reader.readAsDataURL(file);
    },

    async review() {
      if (!this.previewImage) {
        this.errorMessage = "Capture ou selecione uma imagem primeiro.";
        return;
      }

      this.ocrState = "processing";
      this.errorMessage = "";

      let rawText = "";
      let confidence = "";
      let language = "por";
      let errorMessage = "";

      try {
        const worker = await createWorkerModule();
        const result = await worker.recognize(this.previewImage);
        rawText = result.data.text || "";
        confidence = result.data.confidence?.toString() || "";
        await worker.terminate();
      } catch {
        errorMessage = "OCR indisponivel no momento. Voce ainda pode revisar manualmente.";
      } finally {
        this.ocrState = "idle";
      }

      const form = document.createElement("form");
      form.method = "post";
      form.action = this.reviewUrl;
      form.innerHTML = `
        <input type="hidden" name="__RequestVerificationToken" value="${this.token}" />
        <input type="hidden" name="BookId" value="${this.bookId}" />
        <input type="hidden" name="RawText" value="${escapeHtml(rawText)}" />
        <input type="hidden" name="FinalText" value="${escapeHtml(rawText)}" />
        <input type="hidden" name="Base64Image" value="${escapeHtml(this.previewImage)}" />
        <input type="hidden" name="Confidence" value="${escapeHtml(confidence)}" />
        <input type="hidden" name="Language" value="${language}" />
        <input type="hidden" name="errorMessage" value="${escapeHtml(errorMessage)}" />
      `;
      document.body.appendChild(form);
      form.submit();
    }
  }));
});

function escapeHtml(value) {
  return String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("\"", "&quot;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;");
}

document.addEventListener("DOMContentLoaded", () => {
  mountBookReaderTransition();
  mountThreeShelf().catch(() => {});
});
