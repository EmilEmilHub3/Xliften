using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Xliften.Data;
using Xliften.Models;
using Xliften.Services.ServiceInterfaces;

namespace Xliften.Services
{
    /// <summary>
    /// Service til at læse video-filer fra MongoDB GridFS.
    /// </summary>
    public class GridFsVideoService : IGridFsVideoService
    {
        private readonly IGridFSBucket _bucket;

        // ÆNDRING: nu får den MongoContext i stedet for IConfiguration
        public GridFsVideoService(MongoContext context)
        {
            // Brug den fælles bucket fra context
            _bucket = context.VideosBucket;
        }

        /// <summary>
        /// Finder og henter en videofil fra GridFS ud fra dens ObjectId.
        /// Returnerer både stream og contentType fra metadata (hvis sat).
        /// </summary>
        public async Task<(Stream Stream, string ContentType)> GetVideoByIdAsync(string fileId)
        {
            var objectId = ObjectId.Parse(fileId);

            var filter = Builders<GridFSFileInfo>.Filter.Eq(f => f.Id, objectId);
            var fileInfo = await _bucket.Find(filter).FirstOrDefaultAsync();

            if (fileInfo == null)
                throw new FileNotFoundException($"No file with id {fileId}");

            var memoryStream = new MemoryStream();
            await _bucket.DownloadToStreamAsync(objectId, memoryStream);
            memoryStream.Position = 0;

            var contentType = "application/octet-stream";
            if (fileInfo.Metadata != null && fileInfo.Metadata.Contains("contentType"))
                contentType = fileInfo.Metadata["contentType"].AsString;

            return (memoryStream, contentType);
        }

        /// <summary>
        /// Henter id + titel (filnavn) for alle videoer i GridFS.
        /// Bruges til at vise en liste i frontend.
        /// </summary>
        public async Task<IReadOnlyList<VideoInfo>> GetAllVideosAsync()
        {
            var filter = Builders<GridFSFileInfo>.Filter.Empty;
            var cursor = await _bucket.FindAsync(filter);
            var files = await cursor.ToListAsync();

            var result = new List<VideoInfo>();

            foreach (var file in files)
            {
                result.Add(new VideoInfo
                {
                    Id = file.Id.ToString(),
                    Title = file.Filename
                });
            }

            return result;
        }
    }
}
