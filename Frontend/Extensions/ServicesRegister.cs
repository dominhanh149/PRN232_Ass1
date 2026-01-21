//using Assignmen_PRN232__.Data;
using Assignmen_PRN232__.Models;
using Frontend.Services;
using Frontend.Services.IServices;

namespace UsersApp.Extensions
{
    public static class ServicesRegister
    {
        public static void RegisterCustomServices(this IServiceCollection services)
        {
            //Tag Services
            services.AddScoped<ITagService, TagService>();
        }
    }
}
