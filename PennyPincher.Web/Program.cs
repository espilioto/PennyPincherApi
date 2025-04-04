using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using PennyPincher.Services;
using PennyPincher.Services.Accounts;
using PennyPincher.Services.Categories;
using PennyPincher.Services.Charts;
using PennyPincher.Services.Statements;

namespace PennyPincher.Web
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var logger = LogManager.Setup().LoadConfigurationFromFile().GetCurrentClassLogger();
            logger.Info("init main");

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseNLog();

                // Add services to the container.
                builder.Services.AddControllers();

                builder.Services.AddSwaggerGen();
                builder.Services.AddHealthChecks();

                // entity framework
                var cs = builder.Configuration.GetConnectionString("MariaDbConnectionString");
                builder.Services.AddDbContext<PennyPincherApiDbContext>(o => o.UseMySql(cs, ServerVersion.AutoDetect(cs)));
                builder.Services.AddDatabaseDeveloperPageExceptionFilter();

                builder.Services.AddIdentityCore<IdentityUser>(o =>
                {
                    o.User.RequireUniqueEmail = true;
                    o.Password.RequireDigit = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequiredLength = 0;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequiredUniqueChars = 0;
                }).AddEntityFrameworkStores<PennyPincherApiDbContext>();

                builder.Services.AddAutoMapper(typeof(AutomapperProfiles));
                builder.Services.AddScoped<IStatementsService, StatementsService>();
                builder.Services.AddScoped<ICategoriesService, CategoriesService>();
                builder.Services.AddScoped<IAccountService, AccountService>();
                builder.Services.AddScoped<IChartDataService, ChartDataService>();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                    app.UseDeveloperExceptionPage();
                    app.UseMigrationsEndPoint();
                }
                else
                    app.UseExceptionHandler("/Error");

                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<PennyPincherApiDbContext>();
                    await context.Database.MigrateAsync();
                }

                app.UseAuthorization();
                app.MapControllers();
                app.MapHealthChecks("/healthz");

                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, "Stopped program because of exception");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}