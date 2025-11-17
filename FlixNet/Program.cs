using FlixNet.Application.Services;
using FlixNet.Application.Services.ServiceInterfaces;
using FlixNet.Infrastructure.Endpoints;
using Microsoft.Extensions.FileProviders;

namespace FlixNet
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            

            var builder = WebApplication.CreateBuilder(args);

            //When IVideoRepository is being used, it will use VideoService
            builder.Services.AddScoped<IVideoService, VideoService>();


            builder.Services.Configure<DatabaseInfo>(builder.Configuration.GetSection("DatabaseSettings"));


            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            //Used one time pr person to upload the videos to MongoDbB and afterwards comment it out again.
            //Method UploadVideoToDatabaseAsync in VideoService must also be public when doing this, to work.
            //After running one time that method must be set back to private.
            using (var scope = app.Services.CreateScope())
            {
                var videoService = scope.ServiceProvider.GetRequiredService<IVideoService>();
                await videoService.UploadVideoToDatabaseAsync();
            }



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
