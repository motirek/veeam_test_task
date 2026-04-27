using veeam_test_task;

namespace tests
{
    public class SyncEngineIntegrationTests : IDisposable
    {
        private string testSourcePath = Path.Combine(Path.GetTempPath(), "test_source");
        private string testReplicaPath = Path.Combine(Path.GetTempPath(), "test_replica");
        private string testLogPath = Path.Combine(Path.GetTempPath(), "test_log");
        private SyncEngine syncEngine;
        private Logger logger;
        public SyncEngineIntegrationTests()
        {
            logger = new Logger(testLogPath);
            syncEngine = new SyncEngine(testSourcePath, testReplicaPath, logger);
        }
        [Fact]
        public void LogCreateOperation()
        {
            File.WriteAllText(Path.Combine(testSourcePath, "test.txt"), "test content");
            syncEngine.Sync();
            var logContent = File.ReadAllLines(Path.Combine(testLogPath, logger.LogFileName));
            Assert.Contains($"[CREATE] - {Path.Combine(testReplicaPath, "test.txt")}", logContent[0]);
        }
        [Fact]
        public void LogUpdateOperation()
        {
            File.WriteAllText(Path.Combine(testSourcePath, "test.txt"), "test content");
            syncEngine.Sync();
            File.WriteAllText(Path.Combine(testSourcePath, "test.txt"), "updated content");
            syncEngine.Sync();
            var logContent = File.ReadAllLines(Path.Combine(testLogPath, logger.LogFileName));
            Assert.Contains($"[UPDATE] - {Path.Combine(testReplicaPath, "test.txt")}", logContent[1]);
        }
        [Fact]
        public void LogDeleteOperation()
        {
            File.WriteAllText(Path.Combine(testSourcePath, "test.txt"), "test content");
            syncEngine.Sync();
            File.Delete(Path.Combine(testSourcePath, "test.txt"));
            syncEngine.Sync();
            var logContent = File.ReadAllLines(Path.Combine(testLogPath, logger.LogFileName));
            Assert.Contains($"[DELETE] - {Path.Combine(testReplicaPath, "test.txt")}", logContent[1]);
        }
        [Fact]
        public void HandleLockedFile()
        {
            File.WriteAllText(Path.Combine(testSourcePath, "locked.txt"), "initial content");
            syncEngine.Sync();
            using var stream = File.Open(Path.Combine(testReplicaPath, "locked.txt"), FileMode.Open, FileAccess.Read, FileShare.None);
            File.WriteAllText(Path.Combine(testSourcePath, "locked.txt"), "updated content");
            syncEngine.Sync();
            var logContent = File.ReadAllLines(Path.Combine(testLogPath, logger.LogFileName));

            Assert.Contains(logContent, line => line.Contains($"[UPDATE] Failed - {Path.Combine(testReplicaPath, "locked.txt")}"));
            Assert.Contains(logContent, line => line.Contains("Error: "));
        }
        public void Dispose()
        {
            Directory.Delete(testSourcePath, true);
            Directory.Delete(testReplicaPath, true);
            Directory.Delete(testLogPath, true);
        }
    }
}
