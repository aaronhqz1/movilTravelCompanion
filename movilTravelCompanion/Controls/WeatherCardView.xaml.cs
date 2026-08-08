namespace movilTravelCompanion.Controls;

// ContentView reutilizable para mostrar un resultado de clima (ciudad + temperatura
// + detalles opcionales). Equivalente MAUI a extraer un componente de React: en vez
// de <WeatherCard city={...} temp={...} /> con props, acá se usan BindableProperty,
// que son props que ADEMÁS se pueden bindear con {Binding ...} como cualquier
// propiedad de un control nativo (Entry.Text, Label.IsVisible, etc.).
public partial class WeatherCardView : ContentView
{
    public static readonly BindableProperty CityTextProperty =
        BindableProperty.Create(nameof(CityText), typeof(string), typeof(WeatherCardView), string.Empty);

    public static readonly BindableProperty TemperatureTextProperty =
        BindableProperty.Create(nameof(TemperatureText), typeof(string), typeof(WeatherCardView), string.Empty);

    public static readonly BindableProperty DetailsTextProperty =
        BindableProperty.Create(nameof(DetailsText), typeof(string), typeof(WeatherCardView), string.Empty);

    public static readonly BindableProperty ShowDetailsProperty =
        BindableProperty.Create(nameof(ShowDetails), typeof(bool), typeof(WeatherCardView), false);

    public string CityText
    {
        get => (string)GetValue(CityTextProperty);
        set => SetValue(CityTextProperty, value);
    }

    public string TemperatureText
    {
        get => (string)GetValue(TemperatureTextProperty);
        set => SetValue(TemperatureTextProperty, value);
    }

    public string DetailsText
    {
        get => (string)GetValue(DetailsTextProperty);
        set => SetValue(DetailsTextProperty, value);
    }

    // Default false: TravelDestinationPage no muestra humedad/viento, solo Dashboard y Home.
    public bool ShowDetails
    {
        get => (bool)GetValue(ShowDetailsProperty);
        set => SetValue(ShowDetailsProperty, value);
    }

    public WeatherCardView()
    {
        InitializeComponent();
    }
}
