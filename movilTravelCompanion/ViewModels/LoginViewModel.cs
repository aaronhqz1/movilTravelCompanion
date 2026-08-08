using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using movilTravelCompanion.Core.Services;

namespace movilTravelCompanion.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly ISessionStore _sessionStore;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public LoginViewModel(IAuthService authService, ISessionStore sessionStore)
    {
        _authService = authService;
        _sessionStore = sessionStore;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var user = await _authService.LoginAsync(Username, Password);
            Debug.WriteLine($"Login exitoso para el usuario '{user.Username}'.");

            _sessionStore.SaveUser(user);

            // Ruta absoluta ("//"): reemplaza toda la pila de Shell para que, tras loguearse,
            // el boton "atras" no vuelva a la pantalla de login.
            // Siguiendo el flujo real de App.jsx: tras login SIEMPRE se pasa por
            // TravelDestination antes de llegar al Dashboard.
            await Shell.Current.GoToAsync("//" + nameof(Views.TravelDestinationPage));
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
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.RegisterPage));
    }
}
