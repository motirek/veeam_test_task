using veeam_test_task;

namespace tests
{
    public class SyncEngineTest : IDisposable
    {
        private string testSourcePath = Path.Combine(Path.GetTempPath(), "test_source");
        private string testReplicaPath = Path.Combine(Path.GetTempPath(), "test_replica");
        private string testLogPath = Path.Combine(Path.GetTempPath(), "test_log");
        private SyncEngine syncEngine;
        private Logger logger;
        public SyncEngineTest()
        {
            logger = new Logger(testLogPath);
            syncEngine = new SyncEngine(testSourcePath, testReplicaPath, logger);
        }
        [Fact]
        public void CreateFileInReplica()
        {
            File.WriteAllText(Path.Combine(testSourcePath, "test.txt"), "test content");
            syncEngine.Sync();
            Assert.True(File.Exists(Path.Combine(testReplicaPath, "test.txt")));
            Assert.Equal("test content", File.ReadAllText(Path.Combine(testReplicaPath, "test.txt")));
        }
        [Fact]
        public void UpdateFileInReplica()
        {
            File.WriteAllText(Path.Combine(testSourcePath, "update.txt"), "test content");
            syncEngine.Sync();
            File.WriteAllText(Path.Combine(testSourcePath, "update.txt"), "updated content");
            syncEngine.Sync();
            Assert.Equal("updated content", File.ReadAllText(Path.Combine(testReplicaPath, "update.txt")));
        }
        [Fact]
        public void DeleteFileInReplica()
        {
            File.WriteAllText(Path.Combine(testSourcePath, "delete.txt"), "test content");
            syncEngine.Sync();
            File.Delete(Path.Combine(testSourcePath, "delete.txt"));
            syncEngine.Sync();
            Assert.False(File.Exists(Path.Combine(testReplicaPath, "delete.txt")));
        }
        [Fact]
        public void CreateMissingDirectoryInReplica()
        {
            Directory.CreateDirectory(Path.Combine(testSourcePath, "subdir"));
            syncEngine.Sync();
            Assert.True(Directory.Exists(Path.Combine(testReplicaPath, "subdir")));
        }
        [Fact]
        public void DeleteDirectoryInReplica()
        {
            Directory.CreateDirectory(Path.Combine(testSourcePath, "subdir"));
            syncEngine.Sync();
            Directory.Delete(Path.Combine(testSourcePath, "subdir"));
            syncEngine.Sync();
            Assert.False(Directory.Exists(Path.Combine(testReplicaPath, "subdir")));
        }
        [Fact]
        public void CreateMissingDirectories()
        {
            var missingSourcePath = Path.Combine(testSourcePath, "subdir1", "subdir2");
            var missingReplicaPath = Path.Combine(testReplicaPath, "subdir1", "subdir2");
            var missingLogPath = Path.Combine(testLogPath, "subdir1", "subdir2");
            var logger = new Logger(missingLogPath);
            var syncEngine = new SyncEngine(missingSourcePath, missingReplicaPath, logger);
            syncEngine.Sync();
            Assert.True(Directory.Exists(missingSourcePath));
            Assert.True(Directory.Exists(missingReplicaPath));
            Assert.True(Directory.Exists(missingLogPath));
        }

        public void Dispose()
        {
            Directory.Delete(testSourcePath, true);
            Directory.Delete(testReplicaPath, true);
            Directory.Delete(testLogPath, true);
        }
    }
}
