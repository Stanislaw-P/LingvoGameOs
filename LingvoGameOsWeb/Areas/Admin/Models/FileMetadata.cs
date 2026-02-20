namespace LingvoGameOs.Areas.Admin.Models
{
    public class FileMetadata
    {
        public string Key { get; set; } = null!;
        public string? FileUrl { get; set; }
        public long Size { get; set; }
    }
}
