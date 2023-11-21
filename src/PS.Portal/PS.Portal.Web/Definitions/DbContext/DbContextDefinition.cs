using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PS.Portal.DAL.Data;
using PS.Portal.Web.Definitions.Base;

namespace PS.Portal.Web.Definitions.DbContext
{
    public class DbContextDefinition : AppDefinitions
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>(x => x.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationContext>();
        }
    }
}
