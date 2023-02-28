using Data;
using Microsoft.AspNetCore.Identity;
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
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks();

            var cs = builder.Configuration.GetConnectionString("MariaDbConnectionString");
            builder.Services.AddDbContext<PennyPincherApiDbContext>(options =>
                options.UseMySql(cs, ServerVersion.AutoDetect(cs)));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //you can pass options like SignIn.RequireConfirmedAccount or Password.RequireNonAlphanumeric
            builder.Services.AddIdentityCore<IdentityUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
            })
                .AddEntityFrameworkStores<PennyPincherApiDbContext>();

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