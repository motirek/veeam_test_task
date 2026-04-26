using System.Security.Cryptography;

namespace veeam_test_task
{
    public class SyncEngine
    {
        private string sourceDirectory;
        private string replicaDirectory;
        private Logger logger;

        public SyncEngine(string source_directory, string replica_directory, Logger _logger)
        {
            sourceDirectory = source_directory;
            replicaDirectory = replica_directory;
            logger = _logger;
        }

        public void Sync()
        {
            CreateAndUpdate();
            Delete();
        }
        private void CreateAndUpdate()
        {
            foreach (var sourceDir in Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(sourceDirectory, sourceDir);
                var replicaDir = Path.Combine(replicaDirectory, relativePath);

                if (!Directory.Exists(replicaDir))
                {
                    Directory.CreateDirectory(replicaDir!);
                    logger.Log(LogActivity.CREATE, replicaDir);
                }
            }

            foreach (var sourceFile in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(sourceDirectory, sourceFile);
                var replicaFile = Path.Combine(replicaDirectory, relativePath);

                if (!File.Exists(replicaFile))
                {
                    File.Copy(sourceFile, replicaFile);
                    logger.Log(LogActivity.CREATE, replicaFile);
                }
                else if (!FilesIdentical(sourceFile, replicaFile))
                {
                    try
                    {
                        File.Copy(sourceFile, replicaFile, true);
                        logger.Log(LogActivity.UPDATE, replicaFile);
                    }
                    catch (IOException exception)
                    {
                        logger.Log(LogActivity.UPDATE, replicaFile, exception.Message);
                    }
                }
            }
        }
        private void Delete()
        {
            foreach (var replicaFile in Directory.GetFiles(replicaDirectory, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(replicaDirectory, replicaFile);
                var sourceFile = Path.Combine(sourceDirectory, relativePath);

                if (!File.Exists(sourceFile))
                {
                    File.Delete(replicaFile);
                    logger.Log(LogActivity.DELETE, replicaFile);
                }
            }
            foreach (var replicaDir in Directory.GetDirectories(replicaDirectory, "*", SearchOption.AllDirectories)
                .OrderByDescending(d => d.Split(Path.DirectorySeparatorChar).Length))
            {
                var relativePath = Path.GetRelativePath(replicaDirectory, replicaDir);
                var sourceDir = Path.Combine(sourceDirectory, relativePath);

                if (!Directory.Exists(sourceDir) && Directory.Exists(replicaDir))
                {
                    Directory.Delete(replicaDir!, true);
                    logger.Log(LogActivity.DELETE, replicaDir);
                }
            }
        }
        private bool FilesIdentical(string source, string replica)
        {
            return ComputeMD5(source) == ComputeMD5(replica);
        }
        private string ComputeMD5(string path)
        {

            using var md5 = MD5.Create();
            using var stream = File.OpenRead(path);
            return Convert.ToHexString(md5.ComputeHash(stream));
        }
    }
}
