namespace FlixNet.Domain
{
    /// <summary>
    /// A class to display videoinformation in the dropdown list
    /// </summary>
    public class VideoDisplayModel
    {
        public string Id { get; set; } 
        public string Title { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
