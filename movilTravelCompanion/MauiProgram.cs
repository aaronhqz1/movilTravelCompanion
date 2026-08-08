using Microsoft.Extensions.Logging;
using movilTravelCompanion.Core.Configuration;
using movilTravelCompanion.Core.Services;
using movilTravelCompanion.Services;
using movilTravelCompanion.ViewModels;
using movilTravelCompanion.Views;

namespace movilTravelCompanion;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton(_ => new HttpClient { BaseAddress = new Uri(ApiConfig.BaseUrl) });
		builder.Services.AddSingleton<IWeatherApiService, WeatherApiService>();
		builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddSingleton<IHistoryService, HistoryService>();
		builder.Services.AddSingleton<ISessionStore, PreferencesSessionStore>();

		builder.Services.AddTransient<AppShell>();

		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<HomePage>();

		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<LoginPage>();

		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<RegisterPage>();

		builder.Services.AddTransient<RegistrationSuccessViewModel>();
		builder.Services.AddTransient<RegistrationSuccessPage>();

		builder.Services.AddTransient<TravelDestinationViewModel>();
		builder.Services.AddTransient<TravelDestinationPage>();

		builder.Services.AddTransient<DashboardViewModel>();
		builder.Services.AddTransient<DashboardPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
