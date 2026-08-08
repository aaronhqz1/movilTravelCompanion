using movilTravelCompanion.ViewModels;

namespace movilTravelCompanion.Views;

public partial class TravelDestinationPage : ContentPage
{
    public TravelDestinationPage(TravelDestinationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
