using FlixNet.Application.DTO;
using FlixNet.Application.Services.ServiceInterfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace FlixNet.Application.Services
{
    /// <summary>
    /// A service to upload videos to MongoDB GridFS and get video metadata for the UI
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
        /// <param name="databaseInfo"> Creates an object of the DatabaseInfo</param>
        /// <returns></returns>
        private async Task UploadVideoToDatabaseAsync()
        {
            //Findes the path to our VideoFiles
            string currentDirectory = Directory.GetCurrentDirectory();
            string VideoFilesDirectory = Path.Combine(currentDirectory, "VideoFiles");

            Console.WriteLine("currentDirectory: " + currentDirectory);

            // Iterates through all of the files in VideoFiles and adds .mp4 files to the list
            List<string> files = new List<string>();
            Directory.GetFiles(VideoFilesDirectory).ToList().ForEach(file =>
            {
                if (Path.GetExtension(file).Equals(".mp4"))
                {
                    files.Add(file);
                }
            });

            //Creates a GridFS bucket named "videos"
            IGridFSBucket gridFsBucket = new GridFSBucket(_mongoDatabase, new GridFSBucketOptions() { BucketName = "videos" });

            //Iterates through the list of mp4. files and adds them to MongoDB via GridFS
            foreach (string videos in files)
            {
                // Reads the file contents as bytes
                byte[] readText = await File.ReadAllBytesAsync(videos);

                // Uploads videos to the GridFS bucket using only the filename
                await gridFsBucket.UploadFromBytesAsync(Path.GetFileName(videos), readText);
            }
        }
    }
}
