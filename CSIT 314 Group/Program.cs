using CSIT_314_Group.Authentication;
using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
            var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
            var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing.");
            var keyBase64 = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,

                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                Convert.FromBase64String(keyBase64)),

                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role
                    };
                }
                );

            builder.Services.AddAuthorization();

            builder.Services.AddSingleton<DbConnectionFactory>();
            builder.Services.AddScoped<UserAccountRepository>();
            builder.Services.AddScoped<DatabaseInitialiser>();

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

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
