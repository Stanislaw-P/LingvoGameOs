namespace LingvoGameOs.Helpers
{
    public interface ImailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
