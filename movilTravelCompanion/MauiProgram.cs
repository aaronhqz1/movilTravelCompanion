using Microsoft.Extensions.Logging;
using movilTravelCompanion.Core.Configuration;
using movilTravelCompanion.Core.Services;
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

		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<LoginPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
