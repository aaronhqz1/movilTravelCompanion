using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using movilTravelCompanion.Core.Models;
using movilTravelCompanion.Core.Services;

namespace movilTravelCompanion.ViewModels;

public partial class TravelDestinationViewModel : ObservableObject
{
    private readonly IWeatherApiService _weatherApiService;
    private readonly ISessionStore _sessionStore;

    private WeatherData? _searchResult;

    [ObservableProperty]
    private string cityQuery = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    // Se llenan recien cuando la busqueda del clima da resultado; hasta entonces
    // quedan vacias y el boton "Confirmar destino" esta deshabilitado (ver HasResult).
    [ObservableProperty]
    private string resultCity = string.Empty;

    [ObservableProperty]
    private string resultTemperature = string.Empty;

    [ObservableProperty]
    private bool hasResult;

    public TravelDestinationViewModel(IWeatherApiService weatherApiService, ISessionStore sessionStore)
    {
        _weatherApiService = weatherApiService;
        _sessionStore = sessionStore;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(CityQuery))
        {
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;
        HasResult = false;

        try
        {
            _searchResult = await _weatherApiService.SearchWeatherAsync(CityQuery);
            ResultCity = _searchResult.City ?? CityQuery;
            ResultTemperature = $"{_searchResult.Temperature:0.#} °C";
            HasResult = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        if (_searchResult is null)
        {
            return;
        }

        var user = _sessionStore.GetUser();
        if (user is null)
        {
            // No deberia pasar (se llega aca solo despues de loguearse), pero si la
            // sesion se perdio por algun motivo, volvemos a Home en vez de crashear.
            // HomePage es el ShellContent real; LoginPage es ruta global y no admite
            // navegacion absoluta "//" como unica pagina en la pila.
            await Shell.Current.GoToAsync("//" + nameof(Views.HomePage));
            return;
        }

        user.TravelDestination = _searchResult.City;
        user.DestinationLatitude = _searchResult.Latitude;
        user.DestinationLongitude = _searchResult.Longitude;
        _sessionStore.SaveUser(user);

        // Push relativo (no "//"): DashboardPage es una ruta global (Routing.RegisterRoute
        // en AppShell.xaml.cs), no un ShellContent, y Shell no permite navegacion absoluta
        // a una ruta global si queda como unica pagina en la pila ("Global routes currently
        // cannot be the only page on the stack").
        await Shell.Current.GoToAsync(nameof(Views.DashboardPage));

        // Sin esto, cada vez que se llega aca desde el Flyout ("Cambiar Destino" con
        // sesion ya iniciada) se apilan TravelDestination+Dashboard nuevos sin sacar
        // los anteriores, y la pila crece sin limite. Se deja solo Home (raiz) + este
        // Dashboard; efecto secundario: el boton "atras" ahora vuelve directo a Home
        // en vez de a TravelDestination/Login.
        ShellNavigationHelper.TrimNavigationStack();
    }
}
