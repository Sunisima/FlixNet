using FlixNet.Application.DTO;
using FlixNet.Application.Services.ServiceInterfaces;
using FlixNet.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Xabe.FFmpeg;

namespace FlixNet.Application.Services
{
    /// <summary>
    /// A service to upload videos to MongoDB GridFS and get video metadata for the UI
    /// </summary>
    public class VideoService : IVideoService
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly DatabaseInfo _databaseInfo;

        public VideoService(IOptions<DatabaseInfo> databaseInfo) {  //DI of DatabaseInfo
            // Gets the the DatabaseInfor values from IOptions<DatabaseInfo>
            _databaseInfo = databaseInfo.Value;
            //Creates a MongoDB client using the injected connection string
            var mongoClient = new MongoClient(databaseInfo.Value.ConnectionString);
            // Opens the MongoDB database to be used later for storing and retrieving data
            _mongoDatabase = mongoClient.GetDatabase(databaseInfo.Value.DatabaseName);
        }

        // Gets video metadata from MongoDB
        public async Task<ICollection<VideoDisplayDTO>> GetVideoDisplayInfoAsync()
        {
            List<VideoDisplayDTO> videoes = new List<VideoDisplayDTO>();
            
            videoes.Add(new VideoDisplayDTO
            {
                Id = "1",
                Title = "Hello",
                Duration = TimeSpan.FromMinutes(19)
            });

            videoes.Add(new VideoDisplayDTO
            {
                Id = "2",
                Title = "I'm here!",
                Duration = TimeSpan.FromMinutes(31) + TimeSpan.FromSeconds(58)
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

                // Reads duration from the video file in the VideoFiles folder
                var info = await FFmpeg.GetMediaInfo(videoPath);
                // Saves the duration from the video file
                var duration = info.VideoStreams.First().Duration;

                // Creates VideoModelInfo objects for each video
                var videoInfo = new VideoInfoModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = Path.GetFileNameWithoutExtension(videoPath),
                    Duration = duration, // Saves the duration of the video in the VideoInfoModel object
                    GridFsId = gridFsId.ToString()
                };

                // Inserts the Video objects into a "videoInfo" collection in MongoDB
                var videoInfoCollection = _mongoDatabase.GetCollection<VideoInfoModel>(_databaseInfo.VideoInfoCollectionName);

                await videoInfoCollection.InsertOneAsync(videoInfo);

            }
        }
    }
}
