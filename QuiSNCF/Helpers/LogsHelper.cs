public static class LogsHelper
{
    private const string Reset = "\u001b[0m";
    private const string Green = "\u001b[32m";
    private const string Yellow = "\u001b[33m";
    private const string Red = "\u001b[31m";
    private const string Cyan = "\u001b[36m";

    public static void Api(this ILogger logger, string message)
    {
        logger.LogInformation($"{Cyan}[API]{Reset} {message}");
    }

    public static void Success(this ILogger logger, string message)
    {
        logger.LogInformation($"{Green}[INFO - {DateTime.Today}]{Reset} {message}");
    }

    public static void WarningColored(this ILogger logger, string message)
    {
        logger.LogWarning($"{Yellow}[WARNING]{Reset} {message}");
    }

    public static void ErrorColored(this ILogger logger, string message)
    {
        logger.LogError($"{Red}[ERROR]{Reset} {message}");
    }
}