using System.Threading.Tasks;

public static class GitCommands
{
  
    public static Task<CommandLineOutput> GetCurrentBranchName()
    {
        return GitCommandLine.AsyncGitCommand("rev-parse --abbrev-ref HEAD");
    }

    public static Task<CommandLineOutput> GetCurrentCommitShortHash()
    {
        return GitCommandLine.AsyncGitCommand("rev-parse --short HEAD");
    }
    
    public static string GetGitPath()
    {
        return GitCommandLine.RunBlockingGitCommand("rev-parse --show-toplevel").Output;
    }

    public static bool CheckGitValid()
    {
        CommandLineOutput cmdOutput = GitCommandLine.RunBlockingGitCommand("--help");
        return cmdOutput.ExitCode == 0;
    }

    public static bool CheckGitLFSValid()
    {
        CommandLineOutput cmdOutput = GitCommandLine.RunBlockingGitCommand("--help", lfs:true);
        return cmdOutput.ExitCode == 0;
    }
    
    public static Task<CommandLineOutput> GetCommitsBetweenRevisions(string fromRev, string toRev)
    {
        // log is the commits between command
        string gitLog = "log";

        // revision, author, commit message
        string prettyFormat = "--pretty=format:\"%h - %an: %s\"";

        // Ensure if fromRev is empty that we supplant main, this is for a new a build
        // so likely to be a result of a PR
        string parsedFromRevision = string.IsNullOrEmpty(fromRev) ? "main" : fromRev;
        string revisions = $"{parsedFromRevision}..{toRev}";

        // Flesh out command
        string fullCommand = $"{gitLog} {prettyFormat} {revisions}";

        return GitCommandLine.AsyncGitCommand(fullCommand);
    }
    
    #region LFSCommands
    public static Task<CommandLineOutput> QueryLFSLocks()
    {
        return GitCommandLine.AsyncGitCommand("locks --json", lfs:true);
    }

    public static Task<CommandLineOutput> LFSLockFile(string path)
    {
        return GitCommandLine.AsyncGitCommand($"lock \"{path}\"", lfs:true);
    }
    
    public static Task<CommandLineOutput> LFSUnLockFile(string id)
    {
        return GitCommandLine.AsyncGitCommand($"unlock --id={id}", lfs:true);
    }
    
    public static Task<CommandLineOutput> LFSForceUnLockFile(string id)
    {
        return GitCommandLine.AsyncGitCommand($"unlock --id={id} --force", lfs:true);
    }
    #endregion
}