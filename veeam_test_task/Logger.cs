namespace veeam_test_task
{
    public enum LogActivity
    {
        CREATE, UPDATE, DELETE
    }
    public class Logger
    {
        private string logPath { get; set; }
        private const string logFileName = "log.txt";
        public string LogFileName { get { return logFileName; } }
        private string logFilePath => Path.Combine(logPath, logFileName);
        public Logger(string log_path)
        {
            logPath = log_path;
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
            if (File.Exists(logFilePath))
                File.Delete(logFilePath);
        }
        public void Log(LogActivity activity, string filePath)
        {
            Log(activity, filePath, DateTime.Now, "");
        }
        public void Log(LogActivity activity, string filePath, DateTime timestamp, string exceptionMessage = "")
        {
            string line = string.IsNullOrEmpty(exceptionMessage) ? $"[{timestamp}] [{activity}] - {filePath}" : $"[{timestamp}] [{activity}] Failed - {filePath}{Environment.NewLine} Error: {exceptionMessage}";
            Console.WriteLine(line);
            File.AppendAllText(logFilePath, line + Environment.NewLine);
        }
    }
}
