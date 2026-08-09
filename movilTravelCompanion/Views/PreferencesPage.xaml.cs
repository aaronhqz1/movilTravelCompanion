using movilTravelCompanion.ViewModels;

namespace movilTravelCompanion.Views;

public partial class PreferencesPage : ContentPage
{
    private readonly PreferencesViewModel _viewModel;

    public PreferencesPage(PreferencesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    // Ver comentario en DashboardPage.xaml.cs: el icono de hamburguesa automatico
    // no aparece en paginas alcanzadas por push, asi que este boton abre el Flyout a mano.
    private void OnMenuClicked(object? sender, EventArgs e)
    {
        Shell.Current.FlyoutIsPresented = true;
    }
}
