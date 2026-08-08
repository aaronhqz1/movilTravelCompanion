using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using movilTravelCompanion.Core.Models;
using movilTravelCompanion.Core.Services;

namespace movilTravelCompanion.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IWeatherApiService _weatherApiService;
    private readonly IHistoryService _historyService;
    private readonly ISessionStore _sessionStore;

    private User? _currentUser;

    [ObservableProperty]
    private string welcomeMessage = string.Empty;

    [ObservableProperty]
    private string destinationCity = string.Empty;

    [ObservableProperty]
    private string destinationTemperature = string.Empty;

    [ObservableProperty]
    private string destinationDetails = string.Empty;

    [ObservableProperty]
    private string searchCityQuery = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public ObservableCollection<HistoryEntry> RecentHistory { get; } = [];

    public DashboardViewModel(IWeatherApiService weatherApiService, IHistoryService historyService, ISessionStore sessionStore)
    {
        _weatherApiService = weatherApiService;
        _historyService = historyService;
        _sessionStore = sessionStore;
    }

    // Se llama desde DashboardPage.OnAppearing() (el equivalente en MAUI a un
    // useEffect(() => {...}, []) que corre cada vez que la pantalla se muestra),
    // en vez de desde el constructor, porque necesita await y el constructor no puede ser async.
    public async Task LoadAsync()
    {
        _currentUser = _sessionStore.GetUser();
        if (_currentUser is null)
        {
            // HomePage es el ShellContent real (la unica ruta valida para navegacion
            // absoluta "//"); LoginPage es una ruta global y no admite quedar como
            // unica pagina en la pila.
            await Shell.Current.GoToAsync("//" + nameof(Views.HomePage));
            return;
        }

        WelcomeMessage = $"Hola, {_currentUser.Username}";
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            if (_currentUser.DestinationLatitude is double lat && _currentUser.DestinationLongitude is double lon)
            {
                var weather = await _weatherApiService.GetWeatherByCoordinatesAsync(lat, lon);
                DestinationCity = _currentUser.TravelDestination ?? string.Empty;
                ApplyDestinationWeather(weather);
            }

            await ReloadHistoryAsync();
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
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchCityQuery) || _currentUser is null)
        {
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var weather = await _weatherApiService.SearchWeatherAsync(SearchCityQuery);
            await _historyService.AddAsync(_currentUser.UserId, weather);
            SearchCityQuery = string.Empty;
            await ReloadHistoryAsync();
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
    private async Task LogoutAsync()
    {
        _sessionStore.ClearUser();
        await Shell.Current.GoToAsync("//" + nameof(Views.HomePage));
    }

    private async Task ReloadHistoryAsync()
    {
        if (_currentUser is null)
        {
            return;
        }

        var recent = await _historyService.GetRecentAsync(_currentUser.UserId);
        RecentHistory.Clear();
        foreach (var entry in recent)
        {
            RecentHistory.Add(entry);
        }
    }

    private void ApplyDestinationWeather(WeatherData weather)
    {
        DestinationTemperature = $"{weather.Temperature:0.#} °C";
        DestinationDetails = $"Humedad {weather.Humidity:0.#}% · Viento {weather.WindSpeed:0.#} km/h";
    }
}
