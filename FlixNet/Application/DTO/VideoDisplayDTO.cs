namespace FlixNet.Application.DTO
{
    /// <summary>
    /// A class to display videoinformation in the dropdown list in the UI
    /// </summary>
    public class VideoDisplayDTO
    {
        public string Id { get; set; } 
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
