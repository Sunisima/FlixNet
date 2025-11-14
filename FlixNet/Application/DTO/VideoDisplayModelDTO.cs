namespace FlixNet.Application.DTO
{
    /// <summary>
    /// A class to display videoinformation in the dropdown list
    /// </summary>
    public class VideoDisplayModelDTO
    {
        public string Id { get; set; } 
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
