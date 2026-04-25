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
        }
        public void Log(LogActivity activity, string filePath)
        {
            string line = $"[{DateTime.Now}] [{activity}] - {filePath}";
            Console.WriteLine(line);
            File.AppendAllText(logFilePath, line + Environment.NewLine);
        }
    }
}
