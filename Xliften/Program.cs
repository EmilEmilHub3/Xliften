using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Xliften.Data;
using Xliften.Endpoints;
using Xliften.Seeding;
using Xliften.Services;
using Xliften.Services.ServiceInterfaces;

namespace Xliften
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Tilføj controllers, hvis du bruger dem andre steder
            builder.Services.AddControllers();

            // 🔹 Registrér MongoContext som singleton
            builder.Services.AddSingleton<MongoContext>();

            // 🔹 Registrér din video-service
            builder.Services.AddSingleton<IGridFsVideoService, GridFsVideoService>();

            var app = builder.Build();

            // 🔹 Kør seeding én gang ved opstart (genbruger MongoContext)
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MongoContext>();
                VideoSeeder.SeedAsync(context).GetAwaiter().GetResult();
            }

            // (Valgfrit) Static files som din lærer viser, hvis du har mappen "StaticFiles"
            /*
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
                RequestPath = "/StaticFiles",
                EnableDefaultFiles = true
            });
            */

            // Map controllers, hvis du har dem
            app.MapControllers();

            // 🔹 Dine video-endpoints (de filer du har sendt)
            app.MapVideoEndpoints();

            app.Run();
        }
    }
}
