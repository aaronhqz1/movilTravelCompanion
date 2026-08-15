using movilTravelCompanion.ViewModels;

namespace movilTravelCompanion.Views;

public partial class TravelDestinationPage : ContentPage
{
    public TravelDestinationPage(TravelDestinationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Ver comentario en DashboardPage.xaml.cs: el icono de hamburguesa automatico
    // no aparece en paginas alcanzadas por push, asi que este boton abre el Flyout a mano.
    private void OnMenuClicked(object? sender, EventArgs e)
    {
        Shell.Current.FlyoutIsPresented = true;
    }
}
