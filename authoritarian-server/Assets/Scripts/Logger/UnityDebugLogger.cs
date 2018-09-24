using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityDebugLogger : ILogger {

    public void LogMessage(object message) {
        Debug.Log(message);
    }

    public void LogWarning(object warning) {
        Debug.LogWarning(warning);
    }

    public void LogError(object error) {
        Debug.LogError(error);
    }
}
