using movilTravelCompanion.Views;

namespace movilTravelCompanion;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Rutas para paginas a las que se navega con Shell.Current.GoToAsync(...)
		// pero que no aparecen en el Shell como ShellContent/tab (ver AppShell.xaml).
		Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
		Routing.RegisterRoute(nameof(RegistrationSuccessPage), typeof(RegistrationSuccessPage));
		Routing.RegisterRoute(nameof(TravelDestinationPage), typeof(TravelDestinationPage));
		Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
	}
}
