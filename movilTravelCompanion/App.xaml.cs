using Microsoft.Extensions.DependencyInjection;

namespace movilTravelCompanion;

public partial class App : Application
{
	private readonly IServiceProvider _serviceProvider;

	public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();
		_serviceProvider = serviceProvider;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// Resolvemos AppShell recién acá (no como parámetro del constructor)
		// para que Application.Resources ya esté cargado por InitializeComponent
		// antes de que el XAML de las páginas busque StaticResources como "Headline".
		var appShell = _serviceProvider.GetRequiredService<AppShell>();
		return new Window(appShell);
	}
}