using LingvoGameOs.Db.Models;

namespace LingvoGameOs.Models
{
    public class ReviewViewModel
    {
        public Review Review { get; set; } = null!;
        public string? AuthorAvatarUrl { get; set; }
    }
}
