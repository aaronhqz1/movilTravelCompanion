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
            // sesion se perdio por algun motivo, volvemos al login en vez de crashear.
            await Shell.Current.GoToAsync("//" + nameof(Views.LoginPage));
            return;
        }

        user.TravelDestination = _searchResult.City;
        user.DestinationLatitude = _searchResult.Latitude;
        user.DestinationLongitude = _searchResult.Longitude;
        _sessionStore.SaveUser(user);

        // Ruta absoluta ("//") en vez de push: reemplaza toda la pila de navegacion,
        // asi el boton "atras" no vuelve a Login ni a TravelDestination una vez en el Dashboard.
        await Shell.Current.GoToAsync("//" + nameof(Views.DashboardPage));
    }
}
