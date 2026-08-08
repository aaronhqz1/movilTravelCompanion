using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using movilTravelCompanion.Core.Models;
using movilTravelCompanion.Core.Services;

namespace movilTravelCompanion.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IWeatherApiService _weatherApiService;

    [ObservableProperty]
    private string cityQuery = string.Empty;

    [ObservableProperty]
    private string resultCity = string.Empty;

    [ObservableProperty]
    private string resultTemperature = string.Empty;

    [ObservableProperty]
    private string resultDetails = string.Empty;

    [ObservableProperty]
    private bool hasResult;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public HomeViewModel(IWeatherApiService weatherApiService)
    {
        _weatherApiService = weatherApiService;
    }

    // Se llama desde HomePage.OnAppearing() (mismo patron que DashboardViewModel.LoadAsync):
    // necesita await y el constructor no puede ser async.
    public async Task LoadRandomWeatherAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var weather = await _weatherApiService.GetRandomWeatherAsync();
            ApplyWeather(weather);
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
        if (string.IsNullOrWhiteSpace(CityQuery))
        {
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var weather = await _weatherApiService.SearchWeatherAsync(CityQuery);
            ApplyWeather(weather);
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
    private async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.LoginPage));
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.RegisterPage));
    }

    private void ApplyWeather(WeatherData weather)
    {
        ResultCity = weather.City ?? CityQuery;
        ResultTemperature = $"{weather.Temperature:0.#} °C";
        ResultDetails = $"Humedad {weather.Humidity:0.#}% · Viento {weather.WindSpeed:0.#} km/h";
        HasResult = true;
    }
}
