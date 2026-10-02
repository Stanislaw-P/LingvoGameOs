namespace LingvoGameOs.Shared.Services
{
    // HMAC-подпись идентификатора игровой сессии.
    // Web подписывает sessionId при запуске игры и передаёт подпись в iframe;
    // WebApi сверяет подпись при начислении баллов.
    // Секрет задаётся переменной окружения GAMES_API_SECRET и должен совпадать
    // у обоих приложений.
    public static class GameSessionHmac
    {
        public static string Compute(int gameSessionId, string secret)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA256(
                System.Text.Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(
                System.Text.Encoding.UTF8.GetBytes(gameSessionId.ToString()));
            return Convert.ToHexString(hash);
        }

        // Сверяет подпись в constant-time
        public static bool Verify(int gameSessionId, string token, string secret)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var expected = Compute(gameSessionId, secret);
            return System.Security.Cryptography.CryptographicOperations
                .FixedTimeEquals(
                    System.Text.Encoding.UTF8.GetBytes(expected),
                    System.Text.Encoding.UTF8.GetBytes(token));
        }
    }
}
