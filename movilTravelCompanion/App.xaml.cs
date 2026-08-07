using movilTravelCompanion.Views;

namespace movilTravelCompanion;

public partial class App : Application
{
	private readonly LoginPage _loginPage;

	// TODO: reemplazar por AppShell cuando se arme la navegación completa.
	// Por ahora, LoginPage se muestra directamente para poder probarla.
	public App(LoginPage loginPage)
	{
		InitializeComponent();
		_loginPage = loginPage;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(_loginPage);
	}
}