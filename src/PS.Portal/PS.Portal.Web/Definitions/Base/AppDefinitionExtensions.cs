using PS.Portal.Web.Definitions.Base.Interfaces;

namespace PS.Portal.Web.Definitions.Base
{
    public static class AppDefinitionExtensions
    {

        public static void AddDefinitions(this IServiceCollection source, WebApplicationBuilder builder, params Type[] entryPointAssembly)
        {
            var definitions = new List<IAppDefinitions>();

            foreach (var entryPoin in entryPointAssembly)
            {
                var types = entryPoin
                    .Assembly
                    .ExportedTypes
                    .Where(x => !x.IsAbstract && typeof(IAppDefinitions).IsAssignableFrom(x));

                var instances = types.Select(Activator.CreateInstance).Cast<IAppDefinitions>();

                definitions.AddRange(instances);
            }

            definitions.ForEach(app => app.ConfigureServices(source, builder.Configuration));
            source.AddSingleton(definitions as IReadOnlyCollection<IAppDefinitions>);
        }

        public static void UseDefinitions(this WebApplication source)
        {
            var definitions = source.Services.GetRequiredService<IReadOnlyCollection<IAppDefinitions>>();
            var environment = source.Services.GetRequiredService<IWebHostEnvironment>();

            foreach (var endpoint in definitions)
            {
                endpoint.ConfigureApplication(source, environment);
            }
        }

    }
}
