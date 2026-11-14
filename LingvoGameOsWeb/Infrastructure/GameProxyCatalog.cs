namespace LingvoGameOs.Infrastructure;

public sealed record GameProxyDefinition(
    string FolderName,
    string Slug,
    string Destination);

public static class GameProxyCatalog
{
    public static readonly GameProxyDefinition[] All =
    [
        new("lingvo-piece-by-piece", "piece-by-piece", "http://game-piece:8080/"),
        new("crossword-ossetia", "crossword", "http://game-crossword:8080/"),
        new("linvgo-puzzles_in_ossetian", "puzzles", "http://game-puzzles:8080/"),
        new("who_faster", "who-faster", "http://game-who-faster:8080/"),
        new("iron-detective", "iron-detective", "http://game-iron-detective:8080/"),
        new("osetinckie-rebusi", "rebusi", "http://game-rebusi:8080/"),
        new("ossetian-mazaika", "mazaika", "http://game-mazaika:8080/")
    ];

    public static string? GetPublicPath(string? folderName)
    {
        var game = All.FirstOrDefault(candidate =>
            string.Equals(candidate.FolderName, folderName, StringComparison.OrdinalIgnoreCase));

        return game is null ? null : $"/games/{game.Slug}";
    }
}
