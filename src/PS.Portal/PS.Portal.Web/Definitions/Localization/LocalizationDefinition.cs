using PS.Portal.Web.Definitions.Base;
using System.Globalization;

namespace PS.Portal.Web.Definitions.Localization
{
    public class LocalizationDefinition : AppDefinitions
    {
        private RequestLocalizationOptions localizationOptions = null!;

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddLocalization();

            localizationOptions = new RequestLocalizationOptions();

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("es-ES")
            };

            localizationOptions.SupportedCultures = supportedCultures;
            localizationOptions.SupportedUICultures = supportedCultures;
            localizationOptions.SetDefaultCulture("en-US");
            localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
        }

        public override void ConfigureApplication(WebApplication app, IWebHostEnvironment environment)
        {
            app.UseRequestLocalization(localizationOptions);
        }
    }

}
