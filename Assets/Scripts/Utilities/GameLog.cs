using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameLog
{
    public static void AssertMessage(Object context, string format, params string[] args)
    {
        Message(LogType.Assert, context, format, args);
    }
    public static void AssertMessage(string format, params string[] args)
    {
        Message(LogType.Assert, null, format, args);
    }
    public static void NormalMessage(Object context, string format, params string[] args)
    {
        Message(LogType.Log, context, format, args);
    }
    public static void NormalMessage(string format, params string[] args)
    {
        Message(LogType.Log, null, format, args);
    }
    public static void WarningMessage(Object context, string format, params string[] args)
    {
        Message(LogType.Warning, context, format, args);
    }
    public static void WarningMessage(string format, params string[] args)
    {
        Message(LogType.Warning, null, format, args);
    }
    public static void ErrorMessage(Object context, string format, params string[] args)
    {
        Message(LogType.Error, context, format, args);
    }
    public static void ErrorMessage(string format, params string[] args)
    {
        Message(LogType.Error, null, format, args);
    }
    public static void ErrorMessage(Object context, string message)
    {
        Message(LogType.Error, context, message);
    }
    public static void ErrorMessage(string message)
    {
        Message(LogType.Error, null, message);
    }
    public static void ExceptionMessage(string message)
    {
        Message(LogType.Exception, null, message);
    }
    public static void ExceptionMessage(Object context, string format, params string[] args)
    {
        Message(LogType.Exception, context, format, args);
    }

    public static void Message(LogType logType, Object context, string format, params string[] args)
    {
#if UNITY_EDITOR
        Debug.LogFormat(logType, LogOption.None, context, format, args);
#endif
    }
    public static void Message(LogType logType, Object context, string message)
    {
#if UNITY_EDITOR
        Debug.LogFormat(logType, LogOption.None, context, message, string.Empty);
#endif
    }
}
