namespace LingvoGameOs.Db.Helpers
{
    public interface IGameBase
	{
        int Id { get; }
        string? CoverImagePath { get; set; }
        List<string>? ImagesPaths { get; set; }
        public string? GameFilePath { get; set; }
    }
}
