public static class Log {
  public enum LogType {
    Game,
    System
  }

  public static List<string> gameLogs = new List<string>();
  public static List<string> systemLogs = new List<string>();

  public static List<string> logs {
    get {
      List<string> allLogs = new List<string>();
      allLogs.AddRange(gameLogs);
      allLogs.AddRange(systemLogs);
      return allLogs;
    }
  }

  public static void Push(string message) {
    Push(message, LogType.Game);
  }

  public static void PushSys(string message) {
    Push(message, LogType.System);
  }

  public static void Push(string message, LogType type) {
    if (type == LogType.Game) {
      gameLogs.Add(message);
    } else {
      systemLogs.Add(message);
    }
  }

  public static void LogError(string error) {
    Push($"[ERROR] {error}", LogType.System);
  }
}
