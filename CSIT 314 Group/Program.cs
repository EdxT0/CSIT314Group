using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace CSIT_314_Group
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();


            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(options =>
                            {
                                options.Cookie.HttpOnly = true;
                                options.Cookie.SameSite = SameSiteMode.Lax;
                                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                            });

            builder.Services.AddAuthorization();

            builder.Services.AddSingleton<DbConnectionFactory>();

            builder.Services.AddSingleton<UserProfile>();
            builder.Services.AddScoped<UserAccount>();
            builder.Services.AddScoped<FundraiserActivity>();
            builder.Services.AddScoped<UserFundraiserRepository>();
            builder.Services.AddScoped<FundraiserDonationsRepository>();
            builder.Services.AddScoped<Category>();
            builder.Services.AddScoped<FavouriteRepository>(); 

            builder.Services.AddScoped<DatabaseInitialiser>();
            builder.Services.AddScoped<PasswordHasher<UserAccount>>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var databaseInitialiser = scope.ServiceProvider.GetRequiredService<DatabaseInitialiser>();
                databaseInitialiser.InitialiseDatabase();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            // app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
