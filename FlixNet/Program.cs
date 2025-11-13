using FlixNet.Application.Services;
using FlixNet.Application.Services.ServiceInterfaces;
using FlixNet.Infrastructure.Endpoints;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Threading.Tasks;

namespace FlixNet
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            

            var builder = WebApplication.CreateBuilder(args);

            //When IVideoRepository is being used, it will use MongoVideoRepository
            builder.Services.AddScoped<IVideoService, VideoService>();


            var test = builder.Services.Configure<DatabaseInfo>(builder.Configuration.GetSection("DatabaseSettings"));

            await HandleAsync();

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // // Makes our HTML/CSS files in the StaticFiles folder available in the browser
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
          Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
                RequestPath = "/StaticFiles",
                EnableDefaultFiles = true
            });

            app.MapVideoEndpoints();


            app.Run();
        }

        private static async Task HandleAsync(IOptions<DatabaseInfo> databaseInfo)
        {

            //Connect to the database collection
            var mongoClient = new MongoClient(databaseInfo.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseInfo.Value.DatabaseName);


            //Stream insert movie into database
            var movieName = "bladerunner.mp4";

            string currentDirectory = Directory.GetCurrentDirectory();
            string fullPath = Path.Combine(currentDirectory, movieName);

            Console.WriteLine("currentDirectory: " + currentDirectory);


            IGridFSBucket bucket = new GridFSBucket(mongoDatabase, new GridFSBucketOptions() { BucketName = "videos" });
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, movieName);

            // Calling the ReadAllBytes() function
            byte[] readText = File.ReadAllBytes(fullPath);

            // inserting
            bucket.UploadFromBytes(movieName, readText);

            //Test if the data has been stored
            using (var cursor = bucket.Find(filter))
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
    }
}
