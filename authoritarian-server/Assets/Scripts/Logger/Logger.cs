public class Logger {

    private static readonly ILogger _logger = new UnityDebugLogger();

    public static void LogMessage(object message) {
        _logger.LogMessage(message);
    }

    public static void LogWarning(object warning) {
        _logger.LogWarning(warning);
    }

    public static void LogError(object error) {
        _logger.LogError(error);
    }
}
