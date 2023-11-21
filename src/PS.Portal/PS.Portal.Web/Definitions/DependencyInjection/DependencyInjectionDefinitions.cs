using PS.Portal.DAL.Interfaces;
using PS.Portal.DAL.Repositories;
using PS.Portal.Web.Definitions.Base;

namespace PS.Portal.Web.Definitions.DependencyInjection
{
    public class DependencyInjectionDefinitions : AppDefinitions
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMovie, MovieRepository>();
        }
    }
}
