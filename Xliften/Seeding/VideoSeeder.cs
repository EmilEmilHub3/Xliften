using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Xliften.Data;

namespace Xliften.Seeding
{
    /// <summary>
    /// Engangs-seeder til at lægge dine test-videoer ind i MongoDB GridFS.
    /// </summary>
    public static class VideoSeeder
    {
        // ÆNDRING: tager MongoContext i stedet for IConfiguration
        public static async Task SeedAsync(MongoContext context)
        {
            // Brug den fælles bucket fra context
            IGridFSBucket bucket = context.VideosBucket;

            // Dine 3 film (filnavne skal svare til de rigtige filer i output-mappen)
            var movieNames = new[]
            {
                "video1.mp4",
                "video2.mp4",
                "video3.mp4"
            };

            string currentDirectory = Directory.GetCurrentDirectory();
            Console.WriteLine("currentDirectory: " + currentDirectory);

            foreach (var movieName in movieNames)
            {
                // Tjek om filmen allerede ligger i GridFS
                var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, movieName);
                using (var cursor = await bucket.FindAsync(filter))
                {
                    var existing = (await cursor.ToListAsync()).FirstOrDefault();
                    if (existing != null)
                    {
                        Console.WriteLine($"[Seed] Skipper '{movieName}' – findes allerede i GridFS med id {existing.Id}");
                        continue;
                    }
                }

                string fullPath = Path.Combine(currentDirectory, movieName);

                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"[Seed] Filen '{fullPath}' findes ikke – tjek sti og 'Copy to Output Directory'.");
                    continue;
                }

                Console.WriteLine($"[Seed] Indlæser og uploader '{fullPath}'...");

                byte[] bytes = await File.ReadAllBytesAsync(fullPath);
                await bucket.UploadFromBytesAsync(movieName, bytes);

                Console.WriteLine($"[Seed] Upload færdig for '{movieName}'.");
            }
        }
    }
}
