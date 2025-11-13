using FlixNet.Application.Services;
using FlixNet.Application.Services.ServiceInterfaces;
using FlixNet.Infrastructure.Endpoints;
using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;

namespace FlixNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            

            var builder = WebApplication.CreateBuilder(args);

            //When IVideoRepository is being used, it will use MongoVideoRepository
            builder.Services.AddScoped<IVideoService, VideoService>();


            // Creates a Singleton for our DB connection to MongoDB to be used everytime a connection is needed.
            builder.Services.AddSingleton<IMongoDatabase>(sp =>
            {
                var mongoClient = new MongoClient("mongodb://mongohost:27017");
                var mongoDatabase = mongoClient.GetDatabase("FlixNetMovieVault");
                return mongoDatabase;
            });

         
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
    }
}
