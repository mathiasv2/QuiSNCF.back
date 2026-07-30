using System.Security.Cryptography;
using System.Text;

namespace QuiSNCF.Service;

public record PlayProof(GameType GameType, DateOnly Date, int Tries, bool Won)
{
    public bool IsForToday => Date == DateOnly.FromDateTime(DateTime.Today);
}

public class PlayProofService
{
    private readonly byte[] _key;

    public PlayProofService(IConfiguration config, ILogger<PlayProofService> logger)
    {
        var secret = config["PlayProof:Secret"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            _key = RandomNumberGenerator.GetBytes(32);
            logger.LogWarning("PlayProof:Secret absent de la config : clé aléatoire générée, les jetons seront invalidés à chaque redémarrage.");
        }
        else
        {
            _key = Encoding.UTF8.GetBytes(secret);
        }
    }

    public string Issue(GameType gameType, int tries, bool won)
    {
        var payload = $"{(int)gameType}|{DateOnly.FromDateTime(DateTime.Today):yyyy-MM-dd}|{tries}|{(won ? 1 : 0)}";
        return $"{Base64UrlEncode(Encoding.UTF8.GetBytes(payload))}.{Sign(payload)}";
    }

    public bool TryValidate(string? token, out PlayProof? proof)
    {
        proof = null;
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var parts = token.Split('.');
        if (parts.Length != 2)
            return false;

        string payload;
        try
        {
            payload = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
        }
        catch (FormatException)
        {
            return false;
        }

        var expected = Sign(payload);
        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(parts[1])))
            return false;

        var fields = payload.Split('|');
        if (fields.Length != 4
            || !int.TryParse(fields[0], out var gameType) || !Enum.IsDefined((GameType)gameType)
            || !DateOnly.TryParseExact(fields[1], "yyyy-MM-dd", out var date)
            || !int.TryParse(fields[2], out var tries)
            || !int.TryParse(fields[3], out var won))
            return false;

        proof = new PlayProof((GameType)gameType, date, tries, won == 1);
        return true;
    }

    public int GetCurrentTries(string? token, GameType gameType)
    {
        if (TryValidate(token, out var proof)
            && proof!.GameType == gameType
            && proof.IsForToday
            && !proof.Won)
            return proof.Tries;
        return 0;
    }

    private string Sign(string payload)
    {
        using var hmac = new HMACSHA256(_key);
        return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
    }

    private static string Base64UrlEncode(byte[] data) =>
        Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] Base64UrlDecode(string input)
    {
        var s = input.Replace('-', '+').Replace('_', '/');
        return Convert.FromBase64String(s.PadRight(s.Length + (4 - s.Length % 4) % 4, '='));
    }
}
