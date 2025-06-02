using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LingvoGameOs.Db.Models
{
    public class Image
    {
        public Guid Id { get; set; }
        public string URL { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}
