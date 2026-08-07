namespace movilTravelCompanion.Core.Configuration;

public static class ApiConfig
{
    // 10.0.2.2 es la IP especial que el emulador de Android usa para
    // redirigir al localhost de la PC anfitriona (donde corre `npm start`).
    //
    // Para probar en un dispositivo físico, cambia esta URL temporalmente a
    // la IP LAN de la PC (obtenida con `ipconfig`), por ejemplo:
    // "http://192.168.1.50:3000". El celular y la PC deben estar en la misma
    // red WiFi.
    public const string BaseUrl = "http://10.0.2.2:3000";
}
