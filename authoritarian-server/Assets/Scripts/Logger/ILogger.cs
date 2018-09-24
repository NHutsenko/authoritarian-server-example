public interface ILogger {
    void LogMessage(object message);
    void LogWarning(object warning);
    void LogError(object error);
}