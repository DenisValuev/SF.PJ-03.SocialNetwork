using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SF.PJ_03.SocialNetwork.Configs;
using SF.PJ_03.SocialNetwork.Data;
using SF.PJ_03.SocialNetwork.Data.Repository;
using SF.PJ_03.SocialNetwork.Data.UoW;
using SF.PJ_03.SocialNetwork.Models.Users;

namespace SF.PJ_03.SocialNetwork
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var config = builder.Configuration
                .AddJsonFile("appsettings.json")
                .Build();

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            string connection = config.GetConnectionString("DefaultConnection");

            builder.Services
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection))
                .AddCustomRepository<Friend, FriendsRepository>()
                .AddTransient<IUnitOfWork, UnitOfWork>();


            builder.Services.AddIdentity<User, IdentityRole>(opts =>
            {
                opts.Password.RequiredLength = 5;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            })
                .AddEntityFrameworkStores <ApplicationDbContext >();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
