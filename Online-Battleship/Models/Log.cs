using System;
using System.Collections.Generic;
using System.Text;

namespace Online_Battleship.Models
{
    public enum LogType
    {
        System,
        Chat,
        Shot
    }

    public class Log
    {
        public string Message { get; set; }
        public LogType Type { get; set; }
        public string SenderUsername { get; set; }
        public DateTime Timestamp { get; set; }

        public Log(string message, LogType type, string senderUsername = "System")
        {
            Message = message;
            Type = type;
            SenderUsername = senderUsername;
            Timestamp = DateTime.Now;
        }

        public string FormattedMessage => Type == LogType.System
            ? $"[{Timestamp:HH:mm}] {Message}"
            : $"[{Timestamp:HH:mm}] {SenderUsername}: {Message}";
    }
}