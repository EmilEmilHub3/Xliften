
using Xliften.Services.ServiceInterfaces;

namespace Xliften
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSingleton<GridFsVideoService>();

            var app = builder.Build();

            app.MapControllers();
            app.Run();

        }
    }
}
