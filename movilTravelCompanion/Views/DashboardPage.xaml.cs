using movilTravelCompanion.ViewModels;

namespace movilTravelCompanion.Views;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel viewModel)
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

    // El icono de hamburguesa automatico de Shell solo aparece en la pagina raiz
    // de una seccion (sin pila de navegacion detras). Como se llega aca por push
    // desde Home, Shell muestra la flecha de "atras" en vez del icono de menu,
    // asi que este boton abre el Flyout a mano.
    private void OnMenuClicked(object? sender, EventArgs e)
    {
        Shell.Current.FlyoutIsPresented = true;
    }
}
