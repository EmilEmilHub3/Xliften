using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Xliften.Data
{
    /// <summary>
    /// Samlet context til MongoDB-connection, database og GridFS-bucket.
    /// Deler samme forbindelse på tværs af services og seeding.
    /// </summary>
    public class MongoContext
    {
        public IMongoClient Client { get; }
        public IMongoDatabase Database { get; }
        public IGridFSBucket VideosBucket { get; }

        public MongoContext(IConfiguration configuration)
        {
            // Læs connection string og database-navn fra appsettings.json
            Client = new MongoClient(configuration["MongoSettings:ConnectionString"]);
            Database = Client.GetDatabase(configuration["MongoSettings:DatabaseName"]);

            // GridFS-bucket til videoer – samme navn som i resten af projektet
            VideosBucket = new GridFSBucket(Database, new GridFSBucketOptions
            {
                BucketName = "videos"
            });
        }
    }
}
