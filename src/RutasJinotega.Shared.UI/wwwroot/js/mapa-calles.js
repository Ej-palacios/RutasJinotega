const CSS_URL = "https://unpkg.com/leaflet@1.9.4/dist/leaflet.css";
const JS_URL = "https://unpkg.com/leaflet@1.9.4/dist/leaflet.js";
const ESTADO = new WeakMap();
let promesaLeaflet = null;

function cargarCss() {
    if (document.querySelector(`link[href="${CSS_URL}"]`)) {
        return;
    }

    const enlace = document.createElement("link");
    enlace.rel = "stylesheet";
    enlace.href = CSS_URL;
    enlace.crossOrigin = "";
    document.head.appendChild(enlace);
}

function cargarScript() {
    return new Promise((resolver, rechazar) => {
        if (window.L) {
            resolver(window.L);
            return;
        }

        const existente = document.querySelector(`script[src="${JS_URL}"]`);
        if (existente) {
            existente.addEventListener("load", () => resolver(window.L));
            existente.addEventListener("error", () => rechazar(new Error("No se pudo cargar Leaflet.")));
            return;
        }

        const script = document.createElement("script");
        script.src = JS_URL;
        script.async = true;
        script.crossOrigin = "";
        script.addEventListener("load", () => resolver(window.L));
        script.addEventListener("error", () => rechazar(new Error("No se pudo cargar Leaflet.")));
        document.head.appendChild(script);
    });
}

async function asegurarLeaflet() {
    if (window.L) {
        return window.L;
    }

    if (!promesaLeaflet) {
        promesaLeaflet = (async () => {
            cargarCss();
            await cargarScript();
            if (!window.L) {
                throw new Error("Leaflet no está disponible.");
            }
            return window.L;
        })().catch((error) => {
            promesaLeaflet = null;
            throw error;
        });
    }

    return promesaLeaflet;
}

function escapar(texto) {
    return String(texto ?? "").replace(/[&<>"']/g, (caracter) => ({
        "&": "&amp;",
        "<": "&lt;",
        ">": "&gt;",
        "\"": "&quot;",
        "'": "&#39;"
    })[caracter]);
}

export async function crear(elemento, opciones, referencia) {
    const L = await asegurarLeaflet();
    destruir(elemento);

    const mapa = L.map(elemento, {
        zoomControl: true,
        attributionControl: true,
        scrollWheelZoom: true
    });

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 19,
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(mapa);

    const capa = L.layerGroup().addTo(mapa);
    const estado = {
        L,
        mapa,
        capa,
        referencia,
        marcadores: new Map(),
        ajustado: false
    };

    ESTADO.set(elemento, estado);
    actualizar(elemento, opciones, referencia);

    if (opciones?.centro && !opciones?.puntos?.length) {
        mapa.setView([opciones.centro.lat, opciones.centro.lon], opciones.zoom ?? 12);
    }

    setTimeout(() => mapa.invalidateSize(), 0);
    return true;
}

export function actualizar(elemento, opciones, referencia) {
    const estado = ESTADO.get(elemento);
    if (!estado) {
        return false;
    }

    const { L, mapa, capa } = estado;
    capa.clearLayers();
    estado.marcadores.clear();

    const puntos = opciones?.puntos ?? [];
    const limites = [];

    for (const punto of puntos) {
        const activo = punto.activo === true;
        const icono = L.divIcon({
            className: `marcador-mapa${activo ? " activo" : ""}`,
            html: `<span>${escapar(punto.icono || "📍")}</span>`,
            iconSize: [34, 34],
            iconAnchor: [17, 17],
            tooltipAnchor: [0, -18]
        });

        const marcador = L.marker([punto.lat, punto.lon], {
            icon: icono,
            title: punto.titulo ?? "",
            alt: punto.titulo ?? "",
            keyboard: true
        }).addTo(capa);

        if (punto.titulo) {
            marcador.bindTooltip(escapar(punto.titulo), { direction: "top", offset: [0, -16] });
        }

        marcador.on("click", () => {
            referencia?.invokeMethodAsync(opciones?.metodoSeleccion ?? "Seleccionar", punto.id)
                .catch(() => { });
        });

        estado.marcadores.set(punto.id, marcador);
        limites.push([punto.lat, punto.lon]);
    }

    const linea = opciones?.linea ?? [];
    if (linea.length > 1) {
        L.polyline(linea.map((p) => [p.lat, p.lon]), {
            color: "#136f4f",
            weight: 4,
            opacity: 0.78,
            dashArray: "8 8",
            lineCap: "round"
        }).addTo(capa);

        for (const punto of linea) {
            limites.push([punto.lat, punto.lon]);
        }
    }

    if (!estado.ajustado && limites.length > 0) {
        mapa.fitBounds(limites, {
            padding: [36, 36],
            maxZoom: opciones?.zoomMaximo ?? 16
        });
        estado.ajustado = true;
    }

    return true;
}

export function enfocar(elemento, lat, lon, zoom = 16) {
    const estado = ESTADO.get(elemento);
    if (!estado) {
        return;
    }

    const destino = Math.max(estado.mapa.getZoom(), zoom);
    estado.mapa.flyTo([lat, lon], destino, { duration: 0.45 });
}

export function destruir(elemento) {
    const estado = ESTADO.get(elemento);
    if (!estado) {
        return;
    }

    estado.mapa.remove();
    ESTADO.delete(elemento);
}
