using System.Collections.Concurrent;
using System.Diagnostics;
using System;

public static class LogLover
{
    struct LogMessage
    {
        public const string Format = "[{4}][{1}][{2}] - {0}\n{3}";
        public string Message;
        public MessageLevel Level;
        public long Time;
        public string[] Tags;
        public string[] StackTrace;

        public override string ToString()
        {
            return string.Format(Format,
                Message,
                Level,
                string.Join(",", Tags),
                string.Join("\n", StackTrace),
                DateTime.FromFileTimeUtc(Time).ToString("dd.MM.yyyy HH:mm:ss:ffff"));
        }
    }
    readonly static ConcurrentQueue<LogMessage> Logs = new ConcurrentQueue<LogMessage>();

    public static void Message(object message, MessageLevel level, params object[] tags)
    {
        LogMessage log = new LogMessage();
        log.Message = message.ToString();
        log.Level = level;
        log.Time = System.DateTime.UtcNow.ToFileTimeUtc();
        switch (level)
        {
            case MessageLevel.Info:
                log.StackTrace = new string[0];
                break;
            case MessageLevel.Debug:
            case MessageLevel.Warning:
            case MessageLevel.Error:
            case MessageLevel.Fatal:
#if DEBUG
                StackTrace st = new StackTrace(true);
#else
                StackTrace st = new StackTrace();
#endif
                var frames = st.GetFrames();
                log.StackTrace = new string[frames.Length];
                for (int i = 0; i < frames.Length; i++)
                {
                    var frame = frames[i];
                    var sb = new System.Text.StringBuilder();
                    sb.Append(frame.GetMethod().ReflectedType.ToString());
                    sb.Append(":");
                    sb.Append(frame.GetMethod().ToString());
#if DEBUG
                    sb.Append("\n - ");
                    sb.Append(frame.GetFileName());
                    sb.Append(" ");
                    sb.Append(frame.GetFileLineNumber());
#endif
                    log.StackTrace[i] = sb.ToString();
                }
                break;
        }
        if (tags != null)
        {
            log.Tags = new string[tags.Length];
            for (int i = 0; i < tags.Length; i++)
            {
                log.Tags[i] = tags[i].ToString();
            }
        }
        else
        {
            log.Tags = new string[0];
        }
        UnityEngine.Debug.Log(log.ToString());
        //Logs.Enqueue(log);
    }



}