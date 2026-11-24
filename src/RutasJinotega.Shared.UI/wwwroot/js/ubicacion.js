// Módulo ES usado por ServicioUbicacionWeb. Devuelve { latitud, longitud } o null.
export function obtener() {
    return new Promise((resolve) => {
        if (!navigator.geolocation) {
            resolve(null);
            return;
        }

        navigator.geolocation.getCurrentPosition(
            (posicion) => resolve({
                latitud: posicion.coords.latitude,
                longitud: posicion.coords.longitude
            }),
            () => resolve(null),
            { enableHighAccuracy: true, timeout: 8000, maximumAge: 60000 }
        );
    });
}
