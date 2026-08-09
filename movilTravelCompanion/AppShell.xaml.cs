using CommunityToolkit.Mvvm.Input;
using movilTravelCompanion.Core.Services;
using movilTravelCompanion.Views;

namespace movilTravelCompanion;

public partial class AppShell : Shell
{
	private readonly ISessionStore _sessionStore;

	public AppShell(ISessionStore sessionStore)
	{
		InitializeComponent();

		_sessionStore = sessionStore;
		BindingContext = this;

		// Rutas para paginas a las que se navega con Shell.Current.GoToAsync(...)
		// pero que no aparecen en el Shell como ShellContent/tab (ver AppShell.xaml).
		Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
		Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
		Routing.RegisterRoute(nameof(RegistrationSuccessPage), typeof(RegistrationSuccessPage));
		Routing.RegisterRoute(nameof(TravelDestinationPage), typeof(TravelDestinationPage));
		Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
		Routing.RegisterRoute(nameof(PreferencesPage), typeof(PreferencesPage));
	}

	[RelayCommand]
	private async Task GoToDashboardAsync()
	{
		FlyoutIsPresented = false;
		await Current.GoToAsync(nameof(DashboardPage));
	}

	[RelayCommand]
	private async Task GoToTravelDestinationAsync()
	{
		FlyoutIsPresented = false;
		await Current.GoToAsync(nameof(TravelDestinationPage));
	}

	[RelayCommand]
	private async Task GoToPreferencesAsync()
	{
		FlyoutIsPresented = false;
		await Current.GoToAsync(nameof(PreferencesPage));
	}

	[RelayCommand]
	private async Task LogoutAsync()
	{
		FlyoutIsPresented = false;
		_sessionStore.ClearUser();
		await Current.GoToAsync("//" + nameof(HomePage));
	}
}
