using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WonderlandChip.Database.DbContexts;
using WonderlandChip.Database.Repositories;
using WonderlandChip.Database.Repositories.Interfaces;
using WonderlandChip.WebAPI.Services;

namespace WonderlandChip.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            string serverConnectionString = builder.Configuration["MSSQL_SERVER"] ?? "localhost";
            string portConnectionString = builder.Configuration["MSSQL_PORT"] ?? "1443";
            string userConnectionString = builder.Configuration["MSSQL_USER"] ?? "SA";
            string passwordConnectionString = builder.Configuration["MSSQL_PASSWORD"] ?? "Password1";
            string initialCatalogConnectionstring = builder.Configuration["MSSQL_DBNAME"] ?? "animal-chipization";
            string connectionString =
                $"Server={serverConnectionString};" +
                $"Initial Catalog={initialCatalogConnectionstring};" +
                $"User ID = {userConnectionString};" +
                $"Password={passwordConnectionString};";
            builder.Services.AddDbContext<ChipizationDbContext>(options =>
            options.UseSqlServer(connectionString));

            builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();

            builder.Services.AddScoped<IAnimalTypeRepository, AnimalTypeRepository>();

            builder.Services.AddScoped<ILocationPointRepository, LocationPointRepository>();

            builder.Services.AddScoped<IAccountRepository, AccountRepository>();

            builder.Services.AddScoped<IAnimalVisitedLocationRepository, AnimalVisitedLocationRepository>();

            builder.Services.AddScoped<AuthenticationService>();

            builder.Services.AddControllers();
            builder.Services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>(nameof(DatabaseHealthCheck));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapControllers();
            app.MapHealthChecks("/health");
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ChipizationDbContext>();
                context.Database.EnsureCreated();
                context.Database.Migrate();
            }
            app.Run();
        }
    }
}