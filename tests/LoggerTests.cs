using veeam_test_task;

namespace tests
{
    public class LoggerTests : IDisposable
    {
        private string testLogPath = Path.Combine(Path.GetTempPath(), "test_log");
        private Logger logger;
        public LoggerTests()
        {
            logger = new Logger(testLogPath);
        }
        [Fact]
        public void WriteCreateActivity()
        {
            var timestamp = DateTime.Now;
            logger.Log(LogActivity.CREATE, "test.txt", timestamp);
            var logContent = File.ReadAllLines(Path.Combine(testLogPath, logger.LogFileName));
            Assert.Equal($"[{timestamp}] [CREATE] - test.txt", logContent[0]);
        }
        [Fact]
        public void WriteUpdateActivity()
        {
            var timestamp = DateTime.Now;
            logger.Log(LogActivity.UPDATE, "test.txt", timestamp);
            var logContent = File.ReadAllLines(Path.Combine(testLogPath, logger.LogFileName));
            Assert.Equal($"[{timestamp}] [UPDATE] - test.txt", logContent[0]);
        }
        [Fact]
        public void WriteDeleteActivity()
        {
            var timestamp = DateTime.Now;
            logger.Log(LogActivity.DELETE, "test.txt", timestamp);
            var logContent = File.ReadAllLines(Path.Combine(testLogPath, logger.LogFileName));
            Assert.Equal($"[{timestamp}] [DELETE] - test.txt", logContent[0]);
        }
        [Fact]
        public void AppendsMultipleEntrie()
        {
            var timestamp = DateTime.Now;
            logger.Log(LogActivity.DELETE, "test.txt", timestamp);
            logger.Log(LogActivity.UPDATE, "test1.txt", timestamp);
            var logContent = File.ReadAllLines(Path.Combine(testLogPath, logger.LogFileName));
            Assert.Equal(2, logContent.Length);
            Assert.Equal($"[{timestamp}] [DELETE] - test.txt", logContent[0]);
            Assert.Equal($"[{timestamp}] [UPDATE] - test1.txt", logContent[1]);
        }
        [Fact]
        public void WriteLogWithExceptionMessage()
        {
            var timestamp = DateTime.Now;
            logger.Log(LogActivity.UPDATE, "test.txt", timestamp, "File is locked");
            var logContent = File.ReadAllLines(Path.Combine(testLogPath, logger.LogFileName));
            Assert.Equal($"[{timestamp}] [UPDATE] Failed - test.txt", logContent[0]);
            Assert.Equal($" Error: File is locked", logContent[1]);

        }
        [Fact]
        public void ClearLogOnStart()
        {
            var timestamp = DateTime.Now;
            logger.Log(LogActivity.CREATE, "test.txt", timestamp);
            var logContent = File.ReadAllLines(Path.Combine(testLogPath, logger.LogFileName));
            Assert.Single(logContent);
            logger = new Logger(testLogPath);
            Assert.False(File.Exists(Path.Combine(testLogPath, logger.LogFileName)));
        }
        public void Dispose()
        {
            //Directory.Delete(testLogPath, true);
        }
    }
}
