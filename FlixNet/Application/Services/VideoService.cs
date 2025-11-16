using FlixNet.Application.DTO;
using FlixNet.Application.Services.ServiceInterfaces;
using FlixNet.Domain;
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
        private readonly DatabaseInfo _databaseInfo;

        public VideoService(IOptions<DatabaseInfo> databaseInfo) //DI of DatabaseInfo
        {
            //Creates a MongoDB client using the injected connection string
            var mongoClient = new MongoClient(_databaseInfo.ConnectionString);
            // Gets the the DatabaseInfor values from IOptions<DatabaseInfo>
            _databaseInfo = databaseInfo.Value;
            // Opens the MongoDB database to be used later for storing and retrieving data
            _mongoDatabase = mongoClient.GetDatabase(_databaseInfo.DatabaseName); 
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
        public async Task UploadVideoToDatabaseAsync()
        {
            //Findes the path to our VideoFiles
            string currentDirectory = Directory.GetCurrentDirectory();
            string VideoFilesDirectory = Path.Combine(currentDirectory, "VideoFiles");

            Console.WriteLine("currentDirectory: " + currentDirectory);

            // Iterates through all of the files in VideoFiles and adds the .mp4 file paths to the list
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


            //Iterates through the list of mp4. file paths and adds them to MongoDB via GridFS
            foreach (string videos in files)
            {
                // Reads the file contents as bytes
                byte[] readText = await File.ReadAllBytesAsync(videos);

                // Uploads videos to the GridFS bucket using only the filename
                var gridFsId = await gridFsBucket.UploadFromBytesAsync(Path.GetFileName(videos), readText);

                // Creates VideoModelInfo objects for each video
                var videoInfo = new VideoInfoModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = Path.GetFileNameWithoutExtension(videos),
                    Duration = TimeSpan.Zero, //must later be filled out with correct timespan of each video!!!!
                    GridFsId = gridFsId.ToString()
                };

                // Inserts the VideoInfoModel objects into a "videoInfo" collection in MongoDB
                var videoInfoCollection = _mongoDatabase.GetCollection<VideoInfoModel>(_databaseInfo.VideoInfoCollectionName);

                await videoInfoCollection.InsertOneAsync(videoInfo);

            }
        }
    }
}
