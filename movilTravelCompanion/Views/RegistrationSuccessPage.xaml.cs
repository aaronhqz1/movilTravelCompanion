using movilTravelCompanion.ViewModels;

namespace movilTravelCompanion.Views;

public partial class RegistrationSuccessPage : ContentPage
{
    public RegistrationSuccessPage(RegistrationSuccessViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
