using System.Collections;
using System.Collections.Generic;

public static class Log
{
    public static void Info(object message)
    {
        Message(message, MessageLevel.Info);
    }
    public static void Info(object message, params object[] tags)
    {
        Message(message, MessageLevel.Info, tags);
    }
    public static void Debug(object message)
    {
        Message(message, MessageLevel.Debug);
    }
    public static void Debug(object message, params object[] tags)
    {
        Message(message, MessageLevel.Debug, tags);
    }
    public static void Warning(object message)
    {
        Message(message, MessageLevel.Warning);
    }
    public static void Warning(object message, params object[] tags)
    {
        Message(message, MessageLevel.Warning, tags);
    }
    public static void Error(object message)
    {
        Message(message, MessageLevel.Error);
    }
    public static void Error(object message, params object[] tags)
    {
        Message(message, MessageLevel.Error, tags);
    }
    public static void Fatal(object message)
    {
        Message(message, MessageLevel.Fatal);
    }
    public static void Fatal(object message, params object[] tags)
    {
        Message(message, MessageLevel.Fatal, tags);
    }
    public static void Message(object message, MessageLevel level, params object[] tags)
    {
        LogLover.Message(message, level, tags);
    }
}
public enum MessageLevel
{
    Info,
    Debug,
    Warning,
    Error,
    Fatal
}
