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
        private string logFilePath => Path.Combine(logPath, logFileName);
        public Logger(string log_path)
        {
            logPath = log_path;
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
            if (File.Exists(logFilePath))
                File.Delete(logFilePath);
        }
        public void Log(LogActivity activity, string filePath, string exceptionMessage = "")
        {
            string line = string.IsNullOrEmpty(exceptionMessage) ? $"[{DateTime.Now}] [{activity}] - {filePath}" : $"[{DateTime.Now}] [{activity}] Failed - {filePath}\n Error: {exceptionMessage}";
            Console.WriteLine(line);
            File.AppendAllText(logFilePath, line + Environment.NewLine);
        }
    }
}
