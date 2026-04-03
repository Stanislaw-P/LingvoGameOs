using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db.Models
{
    public class ApiKey
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string KeyHash { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? LastUsedAt { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual User User { get; set; } = null!;
    }
}
