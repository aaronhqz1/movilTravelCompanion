# movilTravelCompanion.Controls.WeatherCardView

## Ubicación
`movilTravelCompanion/Controls/WeatherCardView.xaml`, `movilTravelCompanion/Controls/WeatherCardView.xaml.cs`

## Propósito
`ContentView` reutilizable que muestra un resultado de clima (ciudad, temperatura y detalles opcionales de humedad/viento). Reemplaza el componente React `WeatherCard.jsx` de la app original, evitando que `HomePage`, `TravelDestinationPage` y `DashboardPage` dupliquen el mismo bloque de `Label`s.

## Tipo
ContentView reutilizable, con `BindableProperty`s propias (equivalente MAUI a un componente React con `props`).

## Responsabilidades
- Mostrar ciudad, temperatura y (opcionalmente) detalles adicionales, todo parametrizable desde el consumidor vía `BindableProperty`.
- Ocultar la línea de detalles cuando `ShowDetails` es `false` (comportamiento por defecto).

## Dependencias
Ninguna inyectada — es un control de UI puro sin servicios ni ViewModel propio.

## Miembros públicos clave

### Propiedades observables / Bindable
| Nombre | Tipo | Descripción |
|---|---|---|
| `CityText` | `string` | Nombre de la ciudad a mostrar. Default `string.Empty`. |
| `TemperatureText` | `string` | Texto de temperatura ya formateado (p. ej. `"18.3 °C"`), calculado por el consumidor. Default `string.Empty`. |
| `DetailsText` | `string` | Texto de detalles adicionales (p. ej. `"Humedad 60% · Viento 12 km/h"`). Default `string.Empty`. |
| `ShowDetails` | `bool` | Controla la visibilidad de la línea de `DetailsText`. Default `false` — pensado porque `TravelDestinationPage` no muestra humedad/viento, solo `Dashboard` y `Home`. |

### Comandos / Métodos
Ninguno — no expone comandos, solo las `BindableProperty`s de arriba.

## Flujo y lógica relevante
El XAML interno (`WeatherCardView.xaml`) usa `x:Name="Root"` y `BindingContext="{x:Reference Root}"` en el `VerticalStackLayout` contenedor, de forma que los `Label`s internos bindean (`{Binding CityText}`, etc.) contra las propiedades del propio control, no contra un `BindingContext` externo — así el control es autocontenido y el consumidor solo necesita setear atributos XAML (`CityText="{Binding ...}"`), sin preocuparse por el `BindingContext` interno.

## Data binding (si es Page/ViewModel)
No tiene ViewModel propio. Es consumido embebido dentro de las páginas que sí lo tienen:
- `HomePage.xaml`: bindea `CityText`/`TemperatureText`/`DetailsText` a `ResultCity`/`ResultTemperature`/`ResultDetails` de `HomeViewModel`, con `IsVisible="{Binding HasResult}"` y `ShowDetails="True"`.
- `TravelDestinationPage.xaml`: bindea a `ResultCity`/`ResultTemperature` de `TravelDestinationViewModel`, sin `DetailsText` ni `ShowDetails` (quedan en sus defaults vacío/`false`).
- `DashboardPage.xaml`: bindea a `DestinationCity`/`DestinationTemperature`/`DestinationDetails` de `DashboardViewModel`, con `ShowDetails="True"`.

## Relaciones
- Usado por `HomePage.xaml`, `TravelDestinationPage.xaml` y `DashboardPage.xaml` (ver arriba).
- No depende de ninguna clase de `Core` ni de otros Controls.

## Notas de diseño
Ver CONTEXT.md, "Mapeo de componentes React → MAUI": `WeatherCardView` es el equivalente directo de `WeatherCard.jsx`. También se menciona en "Estado actual": se creó específicamente porque "antes cada página duplicaba el mismo bloque de `Label`s" — es la extracción de un componente compartido a partir de una duplicación real detectada durante el desarrollo, no un diseño anticipado desde el principio.
