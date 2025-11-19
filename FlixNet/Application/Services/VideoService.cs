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
        private readonly IMongoCollection<VideoInfoModel> _videoInfoCollection;

        public VideoService(IOptions<DatabaseInfo> databaseInfo) 
        {
            //Creates a MongoDB client using the injected connection string
            var mongoClient = new MongoClient(databaseInfo.Value.ConnectionString);
            // Opens the MongoDB database to be used later for storing and retrieving data
            _mongoDatabase = mongoClient.GetDatabase(databaseInfo.Value.DatabaseName);
            //Gets the videoInfo collection from the database to be used later
            _videoInfoCollection = _mongoDatabase.GetCollection<VideoInfoModel>(databaseInfo.Value.VideoInfoCollectionName);
        }

        // Gets video metadata from MongoDB
        public async Task<ICollection<VideoDisplayDTO>> GetVideoDisplayInfoAsync()
        {
            List<VideoDisplayDTO> videoes = new List<VideoDisplayDTO>();
            
            _videoInfoCollection.Find(_ => true).ToList().ForEach(video =>
            {
                videoes.Add(new VideoDisplayDTO
                {
                    Id = video.Id,
                    Title = video.Title,
                    Duration = video.Duration
                });
            });

            return await Task.FromResult(videoes as ICollection<VideoDisplayDTO>);
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
            foreach (string videoPath in files)
            {
                // Reads the file contents as bytes
                byte[] readText = await File.ReadAllBytesAsync(videoPath);

                // Uploads videos to the GridFS bucket using only the filename
                var gridFsId = await gridFsBucket.UploadFromBytesAsync(Path.GetFileName(videoPath), readText);


                // Creates VideoModelInfo objects for each video
                var videoInfo = new VideoInfoModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = Path.GetFileNameWithoutExtension(videoPath),
                    Duration = duration, // Saves the duration of the video in the VideoInfoModel object
                    GridFsId = gridFsId.ToString()
                };

                // Inserts the Video objects into a "videoInfo" collection in MongoDB
                await _videoInfoCollection.InsertOneAsync(videoInfo);

            }
        }
    }
}
