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
        // TODO: una vez exista TravelDestinationPage, navegar ahi en lugar de volver al login.
        await Shell.Current.GoToAsync("//" + nameof(Views.LoginPage));
    }
}
