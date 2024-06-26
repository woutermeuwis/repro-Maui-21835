﻿using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace Maui_Repro_21835;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCompatibility()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.ConfigureMauiHandlers(handlers =>
			{
#if IOS
				handlers.AddHandler<CustomTabbedPage, CustomTabbedPageRenderer>();
#elif ANDROID
				handlers.AddCompatibilityRenderer<CustomTabbedPage, CustomTabbedPageRenderer>();
#endif

			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}