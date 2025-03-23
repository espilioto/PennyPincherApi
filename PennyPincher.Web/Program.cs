using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using PennyPincher.Services;
using PennyPincher.Services.Accounts;
using PennyPincher.Services.Categories;
using PennyPincher.Services.Statements;
using PennyPincher.Web;

namespace PennyPincher
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

                //builder.Logging.ClearProviders();
                builder.Host.UseNLog();

                // Add services to the container.
                builder.Services.AddControllers();

                builder.Services.AddApiVersioning(o =>
                {
                    o.AssumeDefaultVersionWhenUnspecified = true;
                    o.DefaultApiVersion = new ApiVersion(1, 0);
                    o.ReportApiVersions = true;
                    o.ApiVersionReader = ApiVersionReader.Combine(
                        new QueryStringApiVersionReader("api-version"),
                        new HeaderApiVersionReader("X-Version"),
                        new MediaTypeApiVersionReader("ver"));
                });

                builder.Services.AddVersionedApiExplorer(setup =>
                {
                    setup.GroupNameFormat = "'v'VVV";
                    setup.SubstituteApiVersionInUrl = true;
                });

                builder.Services.AddSwaggerGen();
                builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
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
                builder.Services.AddScoped<IAccountService, AccountServiceV2>();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                    app.UseSwagger();
                    app.UseSwaggerUI(o =>
                    {
                        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                        {
                            o.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                        }
                    });
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