using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using movilTravelCompanion.Core.Services;

namespace movilTravelCompanion.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string homeCity = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Las contraseñas no coinciden.";
            return;
        }

        IsLoading = true;

        try
        {
            // homeCity es opcional al registrarse (se puede configurar despues).
            var homeCityOrNull = string.IsNullOrWhiteSpace(HomeCity) ? null : HomeCity;
            var userId = await _authService.RegisterAsync(Username, Password, homeCityOrNull);
            Debug.WriteLine($"Registro exitoso, userId={userId}.");

            await Shell.Current.GoToAsync(nameof(Views.RegistrationSuccessPage), new Dictionary<string, object>
            {
                ["Username"] = Username
            });
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
        await Shell.Current.GoToAsync("..");
    }
}
