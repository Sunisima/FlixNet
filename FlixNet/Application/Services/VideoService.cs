using FlixNet.Application.DTO;
using FlixNet.Application.Services.ServiceInterfaces;
using FlixNet.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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
        const string VIDEO_BUCKET_NAME = "videos";

        public VideoService(IOptions<DatabaseInfo> databaseInfo)
        {
            //Creates a MongoDB client using the injected connection string
            var mongoClient = new MongoClient(databaseInfo.Value.ConnectionString);
            // Opens the MongoDB database to be used later for storing and retrieving data
            _mongoDatabase = mongoClient.GetDatabase(databaseInfo.Value.DatabaseName);

            // Gets access to the videoInfoCollectionName in MongoDB
            _videoInfoCollection = _mongoDatabase.GetCollection<VideoInfoModel>(databaseInfo.Value.VideoInfoCollectionName);
        }

        /// <summary>
        /// Gets all video information from mongoDB and maps it to VideoDisplayDTO-objects to be used in the UI (the dropdown menu)
        /// </summary>
        /// <returns> A collection of VideoDisplayDTO with metadata for each video </returns>
        public async Task<ICollection<VideoDisplayDTO>> GetVideoDisplayInfoAsync()
        {
            // Executes a query to get all documents and then deserializes them into VideoInfoModel-objects 
            var getAllInfoFromVideos = await _videoInfoCollection.Find(_ => true).ToListAsync();

            // Maps the VideoInfoModel-objects to VideoDisplayDTO-objects
            var result = getAllInfoFromVideos.Select(v => new VideoDisplayDTO
            {
                Id = v.Id,
                Title = v.Title,
                Duration = v.Duration
            }).ToList();

            return result;
        }

        /// <summary>
        /// Gets a video stream from MongoDB GridFS by the provided video id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<Stream> GetVideoStreamByIdAsync(string id)
        {
            //Creates a GridFS bucket named "videos"
            IGridFSBucket gridFsBucket = new GridFSBucket(_mongoDatabase, new GridFSBucketOptions() { BucketName = VIDEO_BUCKET_NAME });
            
            //Finds the video info by the provided id
            VideoInfoModel videoInfo = await _videoInfoCollection.Find(v => v.Id == id).FirstOrDefaultAsync();
            if (videoInfo == null)
            {
                throw new FileNotFoundException($"Video with id {id} not found.");
            }
            //Parses the GridFS Id from string to ObjectId
            var gridFsObjectId = ObjectId.Parse(videoInfo.GridFsId);

            //Downloads the video stream from GridFS using the ObjectId
            var videoStream = await gridFsBucket.OpenDownloadStreamAsync(gridFsObjectId);
            return videoStream;
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
            IGridFSBucket gridFsBucket = new GridFSBucket(_mongoDatabase, new GridFSBucketOptions() { BucketName = VIDEO_BUCKET_NAME });

            // Hardcoded durations taken from the video files
            var videoDurations = new Dictionary<string, string>
            {
                { "Funniest-Cat-Videoes-Ever", "00:20:10" },
                { "How-To-Learn-Programming", "00:04:45" },
                { "Most_Popular_Funny_Cats", "00:16:44" }
            };


            //Iterates through the list of mp4. file paths and adds them to MongoDB via GridFS
            foreach (string videoPath in files)
            {
                // Gets the FileName of each video
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(videoPath);

                // Gets the duration from the dictionary or falls back to 00:00:00
                string durationFromDictionary = videoDurations.ContainsKey(fileNameWithoutExt) ? videoDurations[fileNameWithoutExt] : "00:00:00";

                TimeSpan duration = TimeSpan.Parse(durationFromDictionary);

                // Reads the file contents as bytes
                byte[] readText = await File.ReadAllBytesAsync(videoPath);

                // Uploads videos to the GridFS bucket using only the filename
                var gridFsId = await gridFsBucket.UploadFromBytesAsync(Path.GetFileName(videoPath), readText);

                // Creates VideoModelInfo objects for each video
                var videoInfo = new VideoInfoModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = fileNameWithoutExt,
                    Duration = duration,
                    GridFsId = gridFsId.ToString()
                };

                // Inserts the Video objects into a "videoInfo" collection in MongoDB
                await _videoInfoCollection.InsertOneAsync(videoInfo);

            }
        }
    }
}