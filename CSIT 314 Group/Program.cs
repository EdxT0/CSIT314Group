
using CSIT_314_Group.Data;

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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
