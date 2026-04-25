using System.CommandLine;

namespace veeam_test_task
{
    class Program
    {
        static int Main(string[] args)
        {
            var sourceDirectory = new Option<string>("-s", "--source-directory")
            {
                Description = "Source directory.",
                Required = true,
            };
            var replicaDirectory = new Option<string>("-r", "--replica-directory")
            {
                Description = "Replica directory",
                Required = true,
            };
            var syncInterval = new Option<int>("-i", "--sync-interval")
            {
                Description = "Synchronization interval (seconds)",
                DefaultValueFactory = _ => 30
            };
            var logDirectory = new Option<string>("-l", "--log-directory")
            {
                Description = "Log destination",
                Required = true,
            };
            var rootCommand = new RootCommand("Folder synchronization task")
            {
                Options = { sourceDirectory, replicaDirectory, syncInterval, logDirectory },
            };

            rootCommand.SetAction(parseResult =>
            {
                var sourceDir = parseResult.GetValue(sourceDirectory);
                var replicaDir = parseResult.GetValue(replicaDirectory);
                var interval = parseResult.GetValue(syncInterval);
                var logDest = parseResult.GetValue(logDirectory);

                Console.WriteLine($"Source Directory: {sourceDir}\nReplica Directory: {replicaDir}\nInterval: {interval}\nLog Directory: {logDest}");
                var logger = new Logger(logDest!);
                logger.Log(LogActivity.CREATE, sourceDir!);
            });
            return rootCommand.Parse(args).Invoke();
        }
    }
}