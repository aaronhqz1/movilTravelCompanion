using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using movilTravelCompanion.Core.Models;
using movilTravelCompanion.Core.Services;

namespace movilTravelCompanion.ViewModels;

public partial class PreferencesViewModel : ObservableObject
{
    private readonly IPreferencesService _preferencesService;
    private readonly ISessionStore _sessionStore;

    [ObservableProperty]
    private string selectedClothingStyle = "casual";

    [ObservableProperty]
    private string selectedColdSensitivity = "normal";

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public PreferencesViewModel(IPreferencesService preferencesService, ISessionStore sessionStore)
    {
        _preferencesService = preferencesService;
        _sessionStore = sessionStore;
    }

    // Se llama desde PreferencesPage.OnAppearing(), igual patron que DashboardViewModel.LoadAsync.
    public async Task LoadAsync()
    {
        var user = _sessionStore.GetUser();
        if (user is null)
        {
            await Shell.Current.GoToAsync("//" + nameof(Views.HomePage));
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;
        StatusMessage = string.Empty;

        try
        {
            var preferences = await _preferencesService.GetPreferencesAsync(user.UserId);
            SelectedClothingStyle = preferences.DefaultClothingStyle;
            SelectedColdSensitivity = preferences.ColdSensitivity;
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
    private async Task SaveAsync()
    {
        var user = _sessionStore.GetUser();
        if (user is null)
        {
            await Shell.Current.GoToAsync("//" + nameof(Views.HomePage));
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;
        StatusMessage = string.Empty;

        try
        {
            await _preferencesService.UpdatePreferencesAsync(user.UserId, new UserPreferences
            {
                DefaultClothingStyle = SelectedClothingStyle,
                ColdSensitivity = SelectedColdSensitivity
            });
            StatusMessage = "Preferencias guardadas.";
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
}
