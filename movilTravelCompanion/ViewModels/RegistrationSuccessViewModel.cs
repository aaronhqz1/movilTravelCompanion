using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace movilTravelCompanion.ViewModels;

[QueryProperty(nameof(Username), "Username")]
public partial class RegistrationSuccessViewModel : ObservableObject
{
    [ObservableProperty]
    private string username = string.Empty;

    [RelayCommand]
    private async Task ContinueAsync()
    {
        // Push relativo (no "//"): LoginPage es una ruta global (Routing.RegisterRoute
        // en AppShell.xaml.cs), no el ShellContent (que ahora es HomePage), y Shell no
        // permite navegacion absoluta a una ruta global si queda como unica pagina en
        // la pila. El usuario recien registrado debe loguearse antes de continuar.
        await Shell.Current.GoToAsync(nameof(Views.LoginPage));
    }
}
