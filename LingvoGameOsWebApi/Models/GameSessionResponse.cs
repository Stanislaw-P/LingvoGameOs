namespace LingvoGameOsWebApi.Models
{
    public record GameSessionResponse(bool valid, int gameId, string userId, string userName, int totalUserPoints);
}
