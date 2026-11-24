using RutasJinotega.Domain.Catalogo;

namespace RutasJinotega.Infrastructure.Persistencia.Datos;

/// <summary>
/// Contenido inicial del catálogo. Los lugares marcados con «(Ejemplo)» son ficticios;
/// el resto usa datos y coordenadas verificados con fuentes públicas (OSM y Wikipedia).
/// </summary>
internal static class DatosSemilla
{
    public static readonly IReadOnlyList<MunicipioSemilla> Municipios =
    [
        new("Jinotega", "Cabecera departamental y puerta de entrada a la ruta del café."),
        new("San Rafael del Norte", "Municipio de montaña, tierra de Blanca Aráuz y fray Odorico D'Andrea."),
        new("La Concordia", "Municipio cafetalero al oeste de Jinotega."),
        new("San Sebastián de Yalí", "Municipio de bosques, cascadas y miradores.")
    ];

    public static readonly IReadOnlyList<CategoriaSemilla> Categorias =
    [
        new("Cultura e historia", "⛪", "#8a5a3b"),
        new("Naturaleza", "🌿", "#2fa56f"),
        new("Cascadas", "💧", "#2b7fd1"),
        new("Miradores", "🏔️", "#136f4f"),
        new("Café", "☕", "#8a5a3b"),
        new("Gastronomía", "🍽️", "#ffb04a"),
        new("Comercio local", "🏪", "#8a5a3b")
    ];

    public static readonly IReadOnlyList<LugarSemilla> Lugares =
    [
        new(
            Municipio: "Jinotega",
            Nombre: "Catedral de San Juan Bautista",
            Descripcion: "Sede episcopal y corazón del centro histórico de Jinotega, frente al Parque Central.",
            Historia: "El templo actual se reconstruyó entre 1952 y 1958, bajo el impulso del párroco Valencia y luego de Rubén Baltodano Alfaro. Reúne arte religioso traído de España; en 1954 se instaló un altar barroco importado de Alemania y, a mediados de los sesenta, un reloj mecánico de la casa Friedrich E. Korfhage (Melle, Osnabrück) en la torre derecha. Es sede episcopal de la Diócesis de Jinotega, erigida el 18 de junio de 1982.",
            Latitud: 13.0919136,
            Longitud: -86.0023176,
            Dificultad: DificultadAcceso.Facil,
            EsPocoConocido: false,
            Calificacion: 4.6,
            TotalResenas: 18,
            Direccion: "Frente al Parque Central, Jinotega",
            Horario: "Abierta durante el día; misas por la mañana y la tarde",
            Categorias: ["Cultura e historia"]),

        new(
            Municipio: "Jinotega",
            Nombre: "Lago y presa de Apanás",
            Descripcion: "Espejo de agua rodeado de montaña, ideal para el atardecer y el avistamiento de aves.",
            Historia: null,
            Latitud: 13.1500,
            Longitud: -85.9300,
            Dificultad: DificultadAcceso.Moderada,
            EsPocoConocido: false,
            Calificacion: 4.4,
            TotalResenas: 11,
            Direccion: null,
            Horario: null,
            Categorias: ["Naturaleza", "Miradores"]),

        new(
            Municipio: "Jinotega",
            Nombre: "Reserva Natural Datanlí–El Diablo (coordenadas aproximadas)",
            Descripcion: "Área protegida con bosque nuboso y senderos para caminatas.",
            Historia: null,
            Latitud: 13.0100,
            Longitud: -85.9200,
            Dificultad: DificultadAcceso.Dificil,
            EsPocoConocido: true,
            Calificacion: 4.7,
            TotalResenas: 6,
            Direccion: null,
            Horario: null,
            Categorias: ["Naturaleza"]),

        new(
            Municipio: "Jinotega",
            Nombre: "(Ejemplo) Cafetería del centro",
            Descripcion: "Café de altura y desayunos típicos en el centro de Jinotega.",
            Historia: null,
            Latitud: 13.0895,
            Longitud: -86.0005,
            Dificultad: DificultadAcceso.Facil,
            EsPocoConocido: false,
            Calificacion: 4.3,
            TotalResenas: 9,
            Direccion: null,
            Horario: null,
            Categorias: ["Café", "Gastronomía"]),

        new(
            Municipio: "San Rafael del Norte",
            Nombre: "Iglesia de San Rafael del Norte",
            Descripcion: "Iglesia parroquial de San Rafael Arcángel, referente cultural e histórico del municipio.",
            Historia: "La parroquia está dedicada a San Rafael Arcángel y fue declarada Monumento Artístico Nacional. Es conocida como «una catedral sin obispo» por su tamaño, sus pinturas murales y sus vitrales. Fray Odorico D'Andrea impulsó su renovación. En la antigua iglesia se casaron Augusto C. Sandino y la telegrafista Blanca Aráuz el 18 de mayo de 1927.",
            Latitud: 13.2135062,
            Longitud: -86.1102975,
            Dificultad: DificultadAcceso.Facil,
            EsPocoConocido: false,
            Calificacion: 4.5,
            TotalResenas: 7,
            Direccion: "1a Avenida SE, San Rafael del Norte",
            Horario: "Abierta durante el día",
            Categorias: ["Cultura e historia"]),

        new(
            Municipio: "La Concordia",
            Nombre: "(Ejemplo) Ruta del café de La Concordia",
            Descripcion: "Recorrido entre fincas cafetaleras con degustación.",
            Historia: null,
            Latitud: 13.2300,
            Longitud: -86.1400,
            Dificultad: DificultadAcceso.Moderada,
            EsPocoConocido: true,
            Calificacion: 4.8,
            TotalResenas: 5,
            Direccion: null,
            Horario: null,
            Categorias: ["Café", "Naturaleza"]),

        new(
            Municipio: "San Sebastián de Yalí",
            Nombre: "(Ejemplo) Mirador de Yalí",
            Descripcion: "Vista amplia de las montañas, perfecto para fotografía.",
            Historia: null,
            Latitud: 13.2000,
            Longitud: -85.9500,
            Dificultad: DificultadAcceso.Moderada,
            EsPocoConocido: true,
            Calificacion: 4.6,
            TotalResenas: 4,
            Direccion: null,
            Horario: null,
            Categorias: ["Miradores"]),

        new(
            Municipio: "San Sebastián de Yalí",
            Nombre: "(Ejemplo) Cascada escondida de Yalí",
            Descripcion: "Cascada poco visitada; se llega por camino de tierra y última parte a pie.",
            Historia: null,
            Latitud: 13.2200,
            Longitud: -85.9700,
            Dificultad: DificultadAcceso.Dificil,
            EsPocoConocido: true,
            Calificacion: 4.9,
            TotalResenas: 3,
            Direccion: null,
            Horario: null,
            Categorias: ["Cascadas", "Naturaleza"]),

        new(
            Municipio: "San Rafael del Norte",
            Nombre: "(Ejemplo) Mini pulpería La Esquina",
            Descripcion: "Pulpería de barrio; punto de partida para comprar agua y víveres.",
            Historia: null,
            Latitud: 13.2128,
            Longitud: -86.1112,
            Dificultad: DificultadAcceso.Facil,
            EsPocoConocido: false,
            Calificacion: 4.2,
            TotalResenas: 3,
            Direccion: null,
            Horario: null,
            Categorias: ["Comercio local"]),

        new(
            Municipio: "San Rafael del Norte",
            Nombre: "(Ejemplo) Mini pulpería El Camino",
            Descripcion: "Pulpería junto al camino hacia el Tepeyac.",
            Historia: null,
            Latitud: 13.2140,
            Longitud: -86.1108,
            Dificultad: DificultadAcceso.Facil,
            EsPocoConocido: false,
            Calificacion: 4.1,
            TotalResenas: 2,
            Direccion: null,
            Horario: null,
            Categorias: ["Comercio local"]),

        new(
            Municipio: "San Rafael del Norte",
            Nombre: "(Ejemplo) Mini pulpería Doña Rosa",
            Descripcion: "Última pulpería antes de la subida al Tepeyac.",
            Historia: null,
            Latitud: 13.2152,
            Longitud: -86.1098,
            Dificultad: DificultadAcceso.Facil,
            EsPocoConocido: false,
            Calificacion: 4.4,
            TotalResenas: 4,
            Direccion: null,
            Horario: null,
            Categorias: ["Comercio local"]),

        new(
            Municipio: "San Rafael del Norte",
            Nombre: "El Tepeyac",
            Descripcion: "Centro de retiro y santuario en lo alto de San Rafael del Norte, destino de peregrinación.",
            Historia: "El centro de retiro El Tepeyac fue construido en 1966 por fray Odorico D'Andrea, junto con la ermita del mismo nombre. Es propuesto como Santuario Nacional y alberga el Vía Crucis más conocido de Nicaragua. En la capilla «Virgen del Tepeyac» se resguarda el cuerpo incorrupto del padre Odorico, fallecido en 1990, y el sitio recibe peregrinos durante todo el año, especialmente el 12 de diciembre, fiesta de Nuestra Señora de Guadalupe.",
            Latitud: 13.2189532,
            Longitud: -86.1080840,
            Dificultad: DificultadAcceso.Facil,
            EsPocoConocido: false,
            Calificacion: 4.6,
            TotalResenas: 5,
            Direccion: "Camino al Tepeyac, San Rafael del Norte",
            Horario: "Abierto todos los días; misas y Vía Crucis en fechas especiales",
            Categorias: ["Cultura e historia"])
    ];
}

internal sealed record MunicipioSemilla(string Nombre, string? Descripcion);

internal sealed record CategoriaSemilla(string Nombre, string Icono, string Color);

internal sealed record LugarSemilla(
    string Municipio,
    string Nombre,
    string Descripcion,
    string? Historia,
    double Latitud,
    double Longitud,
    DificultadAcceso Dificultad,
    bool EsPocoConocido,
    double Calificacion,
    int TotalResenas,
    string? Direccion,
    string? Horario,
    IReadOnlyList<string> Categorias);
