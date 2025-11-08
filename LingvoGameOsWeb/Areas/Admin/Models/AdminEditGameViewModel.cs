using LingvoGameOs.Db.Models;
using LingvoGameOs.Models;
using System.ComponentModel.DataAnnotations;

namespace LingvoGameOs.Areas.Admin.Models
{
    public class AdminEditGameViewModel : EditGameViewModel
    {       
        public string? GameFolderName { get; set; }
        public int Port { get; set; }
    }
}
