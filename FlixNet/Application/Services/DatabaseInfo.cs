namespace FlixNet.Application.Services
{
    /// <summary>
    /// A class to hold database information from appSettings.json
    /// </summary>
    public class DatabaseInfo
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string VideoInfoCollectionName { get; set; }
    }
}
