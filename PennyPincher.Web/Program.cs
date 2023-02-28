using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;

namespace PennyPincher
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
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
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks();

            // entity framework
            var cs = builder.Configuration.GetConnectionString("MariaDbConnectionString");
            builder.Services.AddDbContext<PennyPincherApiDbContext>(o =>
                o.UseMySql(cs, ServerVersion.AutoDetect(cs)));
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


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                //app.UseSwagger();
                //app.UseSwaggerUI(o =>
                //{
                //    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                //    {
                //        o.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                //            description.GroupName.ToUpperInvariant());
                //    }
                //}); app.UseDeveloperExceptionPage();
                //app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<PennyPincherApiDbContext>();
                await context.Database.MigrateAsync();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHealthChecks("/healthz");

            app.Run();
        }
    }
}