using FlixNet.Application.DTO;
using FlixNet.Application.Services.ServiceInterfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace FlixNet.Application.Services
{
    /// <summary>
    /// Class that gets video info and handles upload and streaming through Repository
    /// </summary>
    public class VideoService : IVideoService
    {
        private readonly IMongoDatabase _mongoDatabase;

        public VideoService(IOptions<DatabaseInfo> databaseInfo) 
        {
            var mongoClient = new MongoClient(databaseInfo.Value.ConnectionString);

            _mongoDatabase = mongoClient.GetDatabase(databaseInfo.Value.DatabaseName); //Dependency injection to get IMongoDatabase
        }

        // Gets video metadata from MongoDB
        public async Task<ICollection<VideoDisplayModelDTO>> GetVideoDisplayInfoAsync()
        {
            List<VideoDisplayModelDTO> videoes = new List<VideoDisplayModelDTO>();
            
            videoes.Add(new VideoDisplayModelDTO
            {
                Id = "1",
                Title = "Hello",
                Duration = TimeSpan.FromMinutes(19)
            });

            videoes.Add(new VideoDisplayModelDTO
            {
                Id = "2",
                Title = "I'm here!",
                Duration = TimeSpan.FromMinutes(31) + TimeSpan.FromSeconds(58)
            });

            return await Task.FromResult(videoes as ICollection<VideoDisplayModelDTO>);
        }

        /// <summary>
        /// Imports videos from VideoFiles and uploads them to MongoDB GridFS.
        /// </summary>
        /// <param name="databaseInfo"></param>
        /// <returns></returns>
        private static async Task UploadVideoToDatabaseAsync(IOptions<DatabaseInfo> databaseInfo)
        {

            //Connect to the database collection
            var mongoClient = new MongoClient(databaseInfo.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseInfo.Value.DatabaseName);

            //Findes the path to our VideoFiles
            string currentDirectory = Directory.GetCurrentDirectory();
            string VideFilesDirectory = Path.Combine(currentDirectory, "VideoFiles");

            Console.WriteLine("currentDirectory: " + currentDirectory);


            IGridFSBucket bucket = new GridFSBucket(mongoDatabase, new GridFSBucketOptions() { BucketName = "videos" });
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, movieName);

            // Calling the ReadAllBytes() function
            byte[] readText = await File.ReadAllBytesAsync(fullPath);

            // inserting
            await bucket.UploadFromBytesAsync(movieName, readText);

            //Test if the data has been stored
            using (var cursor = await bucket.FindAsync(filter))
            {
                var fileInfo = (cursor.ToList()).FirstOrDefault();

                try
                {
                    Console.WriteLine($"{nameof(fileInfo.Id)}: {fileInfo.Id}");
                    Console.WriteLine($"{nameof(fileInfo.Filename)}: {fileInfo.Filename}");
                    Console.WriteLine($"{nameof(fileInfo.Length)}: {fileInfo.Length}");
                    Console.WriteLine($"{nameof(fileInfo.ChunkSizeBytes)}: {fileInfo.ChunkSizeBytes}");
                    Console.WriteLine($"{nameof(fileInfo.UploadDateTime)}: {fileInfo.UploadDateTime}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }
        // Method to get streaming data from MongoDb

        // Method to upload videos til MongoDb
    }
}
