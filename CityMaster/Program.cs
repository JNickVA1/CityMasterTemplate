using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CityMaster
{
	public class Program
	{
		#region Fields
		// Define the constant for the default culture, US English, using the IETF RFC 4646 guidelines.
		// See: https://en.wikipedia.org/wiki/IETF_language_tag
		private static CultureInfo DefaultCulture;

		//
		public static List<CultureInfo> SupportedCultures = new List<CultureInfo>();
		#endregion Fields

		#region Properties
		/// <summary>
		/// The set of key/value configuration properties.
		/// </summary>
		public static IConfiguration Configuration { get; set; }

		/// <summary>
		/// A variable used to hold runtime environment information.
		/// </summary>
		public static IHostingEnvironment Environment { get; set; }

		/// <summary>
		/// Used to configure logging and to create ILogger instances.
		/// </summary>
		public ILoggerFactory LoggerFactory { get; private set; }
		#endregion Properties

		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			//WebHost.CreateDefaultBuilder(args)
			//	.UseStartup<Startup>();

			WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					Configuration = config.Build();
					//
					// Setup localization configuration.
					InitCultures();

					Environment = hostingContext.HostingEnvironment;
				})
				.ConfigureServices(services =>
				{
					//services.AddDbContext<ApplicationDbContext>(options =>
					//{
					//	options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]);
					//});

					//services.AddIdentity<ApplicationUser, IdentityRole>()
					//	.AddEntityFrameworkStores<ApplicationDbContext>()
					//	.AddDefaultTokenProviders();

					#region snippet1

					services.AddLocalization(options => options.ResourcesPath = "Resources");

					services.AddMvc()
						.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
						.AddDataAnnotationsLocalization();

					#endregion

					// Configure supported cultures and localization options
					services.Configure<RequestLocalizationOptions>(options =>
					{
						// State what the default culture for your application is. This will be used if no specific culture
						// can be determined for a given request.
						options.DefaultRequestCulture = new RequestCulture(DefaultCulture);
						// You must explicitly state which cultures your application supports.
						// These are the cultures the app supports for formatting numbers, dates, etc.
						options.SupportedCultures = SupportedCultures.OrderBy(x => x.EnglishName).ToList();
						// These are the cultures the app supports for UI strings, i.e. we have localized resources for.
						options.SupportedUICultures = SupportedCultures.OrderBy(x => x.EnglishName).ToList();

						// You can change which providers are configured to determine the culture for requests, or even add a custom
						// provider with your own logic. The providers will be asked in order to provide a culture for each request,
						// and the first to provide a non-null result that is in the configured supported cultures list will be used.
						// By default, the following built-in providers are configured:
						// - QueryStringRequestCultureProvider, sets culture via "culture" and "ui-culture" query string values, useful for testing
						// - CookieRequestCultureProvider, sets culture via "ASPNET_CULTURE" cookie
						// - AcceptLanguageHeaderRequestCultureProvider, sets culture via the "Accept-Language" request header
						//
						//options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
						//{
						//  // My custom request culture logic
						//  return new ProviderCultureResult("en");
						//}));
					});
				})
				.Configure(app =>
				{
					//Register Syncfusion license
					Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mjg5NDhAMzEzNjJlMzMyZTMwRnlPc09zQU5qWUVFNTVySkl1a0hmam1Ha0lhczlBOFBJQVd0TFFWNDBGWT0=;Mjg5NDlAMzEzNjJlMzMyZTMwWW9ibHhRNUVNem5sL1l1QmlROHNxWENRaEVyVGoxc1cwSk1qTEtLMFpIMD0=;Mjg5NTBAMzEzNjJlMzMyZTMwUFgvSXEwQzNZdzUzYjVRMUhibkY0VWFZeDhHWFI5djVuRjQvYzdyZklGOD0=;Mjg5NTFAMzEzNjJlMzMyZTMwZTBsd2VCMXFDTmpoR2U0aEVieDlxWjFpbEZJTFBYRGZCYW05V3ZjV0NsTT0=;Mjg5NTJAMzEzNjJlMzMyZTMwbnd3WjkwRWdaNXJMdnEwS1Y1a1kzMHVubFFpaXJSOGJERUF2VEFmV1VjWT0=;Mjg5NTNAMzEzNjJlMzMyZTMwZ2lORE1RTGZweDc5NEp5aWVVRXNrMWlkVFU1VVZNLzFBblQyb1Bmd054Zz0=;Mjg5NTRAMzEzNjJlMzMyZTMwaStFMHVhcEJVYnl0RWhFK1pCaHhGRm00NzVtQjJWNDVLYUpsUWpSTmJldz0=;Mjg5NTVAMzEzNjJlMzMyZTMwa3ljQTNaSGtnUVNoQmExNTcwTU1iS3VwRHIwOWRkbG5jU0VGWlB6emhkST0=;Mjg5NTZAMzEzNjJlMzMyZTMwSDFCYnd3VG4wbGxCMjNITUlCa0J2MU1IWWZRdmlkdHdkTGVPTlNlY1k2Yz0=");

					if (Environment.IsDevelopment())
					{
						app.UseDeveloperExceptionPage();
						app.UseDatabaseErrorPage();
					}
					else
					{
						app.UseExceptionHandler("/Error");
						app.UseHsts();

						//// For more details on creating database during deployment,
						//// see: http://go.microsoft.com/fwlink/?LinkID=615859
						//try
						//{
						//	using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
						//		.CreateScope())
						//	{
						//		serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
						//			.Database.Migrate();
						//	}
						//}
						//catch
						//{
						//}
					}

					#region snippet2

					app.UseRequestLocalization(new RequestLocalizationOptions
					{
						DefaultRequestCulture = new RequestCulture(DefaultCulture),
						// Formatting numbers, dates, etc.
						SupportedCultures = SupportedCultures,
						// UI strings that we have localized.
						SupportedUICultures = SupportedCultures
					});

					//
					app.UseStaticFiles();
					app.UseHttpsRedirection();
					app.UseAuthentication();
					app.UseCookiePolicy();
					app.UseMvc();

					#endregion
				});

		/// <summary>
		/// Constructs the List of supported cultures from the app settings file.
		/// </summary>
		private static void InitCultures()
		{
			// Read the default culture.
			DefaultCulture = new CultureInfo(Configuration["Localization:DefaultCulture"]);

			// Read all supported cultures and add each to the List of SupportedCultures.
			var cultures = Configuration.GetSection("Localization:SupportedCultures").GetChildren();
			foreach (var culture in cultures)
			{
				// Create a new CultureInfo object using the value of this setting.
				var cultureInfo = new CultureInfo(culture.Value);

				// Determine if we have one of the common RTL (Right to Left) languages.
				if (cultureInfo.EnglishName.StartsWith("Arabic"))
				{
					//
					cultureInfo.DateTimeFormat = new DateTimeFormatInfo() { Calendar = new GregorianCalendar() };

					//
					cultureInfo.NumberFormat = new NumberFormatInfo() { NativeDigits = "0 1 2 3 4 5 6 7 8 9".Split(" ") };
				}
				if (cultureInfo.EnglishName.StartsWith("Hebrew"))
				{
					//
					cultureInfo.DateTimeFormat = new DateTimeFormatInfo() { Calendar = new GregorianCalendar() };

					//
					cultureInfo.NumberFormat = new NumberFormatInfo() { NativeDigits = "0 1 2 3 4 5 6 7 8 9".Split(" ") };
				}

				//
				SupportedCultures.Add(cultureInfo);
			}
		}
	}
}
