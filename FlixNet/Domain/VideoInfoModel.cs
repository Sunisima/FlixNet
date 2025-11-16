namespace FlixNet.Domain
{
    /// <summary>
    /// Modelclass for storing metadata in the database MongoDB
    /// </summary>
    public class VideoInfoModel
    {
        public string Id { get; set; } //The ID to be used in th UI
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }
        public string GridFsId { get; set; } // The ID to get the correct video file from GridFS
    }
}
