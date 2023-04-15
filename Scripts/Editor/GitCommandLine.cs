using System.Diagnostics;
using System.Threading.Tasks;

public static class GitCommandLine
{
    private static string GIT_COMMAND = "git";
    private static string GIT_LFS_COMMAND = "git-lfs";
    
    // ReSharper disable Unity.PerformanceAnalysis
    public static Task<CommandLineOutput> AsyncGitCommand(string command, bool lfs = false)
    {
        Process process = ProcessWithStartInfo(command, lfs);
        Task<CommandLineOutput> task = SetupProcessWithTask(process);
        StartProcess(process);
        return task;
    }

    public static CommandLineOutput RunBlockingGitCommand(string command, bool lfs = false)
    {
        Process process = ProcessWithStartInfo(command, lfs);
        return ExecuteBlockingProcess(process);
    }

    private static Process ProcessWithStartInfo(string command, bool lfs)
    {
        // Set up our processInfo to run the git command and log to output and errorOutput.
        ProcessStartInfo startInfo = new ProcessStartInfo(lfs ? GIT_LFS_COMMAND : GIT_COMMAND, @command)
        {
            CreateNoWindow = true,          // We want no visible pop-ups
            UseShellExecute = false,        // Allows us to redirect input, output and error streams
            RedirectStandardOutput = true,  // Allows us to read the output stream
            RedirectStandardError = true,    // Allows us to read the error stream
        };

        return new Process { StartInfo = startInfo };
    }

    private static Task<CommandLineOutput> SetupProcessWithTask(Process process)
    {
        TaskCompletionSource<CommandLineOutput> gitTask = new TaskCompletionSource<CommandLineOutput>();
        process.EnableRaisingEvents = true; // Required for the exited event to fire
        process.Exited += (sender, args) =>
        {   
            gitTask.SetResult(new CommandLineOutput(process));
            process.Close();
        };

        return gitTask.Task;
    }

    private static void StartProcess(Process process)
    {
        process.Start();
    }

    private static CommandLineOutput ExecuteBlockingProcess(Process process)
    {
        process.Start();
        process.WaitForExit();
        
        CommandLineOutput commandLineOutput = new CommandLineOutput(process);
        process.Close();
        return commandLineOutput;
    }
}