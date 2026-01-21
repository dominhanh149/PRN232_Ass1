//using Assignmen_PRN232__.Data;
using Assignmen_PRN232__.Models;
using Assignmen_PRN232__.Repositories;
using Assignmen_PRN232__.Repositories.IRepositories;
using Assignmen_PRN232_1.Services;
using Assignmen_PRN232_1.Services.IServices;

namespace UsersApp.Extensions
{
    public static class ServicesRegister
    {
        public static void RegisterCustomServices(this IServiceCollection services)
        {
            //services.RegisterMapsterConfiguration();

            services.AddScoped<IUnitOfWork, UnitOfWork<AppDbContext>>();

            // Tag Services
            services.AddTransient<ITagRepository, TagRepository>();
            services.AddScoped<ITagService, TagService>();

            // Category Services
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();

            // NewsArticle Services
            services.AddTransient<INewsArticleRepository, NewsArticleRepository>();
            services.AddScoped<INewsArticleService, NewsArticleService>();
        }
    }
}
