using System.Diagnostics;

public readonly struct CommandLineOutput
{
    public readonly ProcessStartInfo StartInfo;
    public readonly int ExitCode;
    public readonly string Output;
    public readonly string Error;

    public CommandLineOutput(Process completedProcess)
    {
        StartInfo = completedProcess.StartInfo;
        Output = completedProcess.StandardOutput.ReadToEnd().Trim();
        Error = completedProcess.StandardError.ReadToEnd().Trim();
        ExitCode = completedProcess.ExitCode;
    }

    public override string ToString()
    {
        return $"Exit Code={ExitCode} Output={Output} Error={Error}";
    }
}