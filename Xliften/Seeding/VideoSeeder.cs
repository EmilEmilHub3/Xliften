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
    /// Læser videofiler fra en 'Videos'-mappe i output-kataloget.
    /// </summary>
    public static class VideoSeeder
    {
        // Seeder bruger MongoContext, så vi genbruger samme connection + bucket
        public static async Task SeedAsync(MongoContext context)
        {
            // Brug den fælles bucket fra context
            IGridFSBucket bucket = context.VideosBucket;

            // Dine 3 film (filnavne skal svare til filerne i Videos-mappen)
            var movieNames = new[]
            {
                "video1.mp4",
                "video2.mp4",
                "video3.mp4"
            };

            // currentDirectory peger typisk på bin/Debug/net8.0
            string currentDirectory = Directory.GetCurrentDirectory();

            // Her definerer vi vores "video layer" / mappe
            string videosFolder = Path.Combine(currentDirectory, "Videos");
            Console.WriteLine("currentDirectory: " + currentDirectory);
            Console.WriteLine("videosFolder: " + videosFolder);

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

                // Fuld sti: bin/.../Videos/videoX.mp4
                string fullPath = Path.Combine(videosFolder, movieName);

                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"[Seed] Filen '{fullPath}' findes ikke – tjek at den ligger i projektets 'Videos'-mappe og har 'Copy to Output Directory' slået til.");
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
