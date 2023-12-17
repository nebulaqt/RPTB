using System.Runtime.InteropServices;

namespace RPTB.Utilities;

public static class SystemInfo
{
    public static string GetOperatingSystemInfo()
    {
        var versionString = Environment.OSVersion.ToString();
        var description = RuntimeInformation.OSDescription;
        var architecture = RuntimeInformation.OSArchitecture.ToString();
        var systemInfoDescription =
            $"OS Version: {versionString}, Description: {description}, Architecture: {architecture}";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows specific information
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Linux specific information
            var linuxVersion = GetLinuxVersion();
            systemInfoDescription += $", Linux Version: {linuxVersion}";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // macOS specific information
            var macVersion = GetMacVersion();
            systemInfoDescription += $", macOS Version: {macVersion}";
        }

        return systemInfoDescription;
    }

    public static string GetRuntimeInfo()
    {
        var frameworkDescription = RuntimeInformation.FrameworkDescription;
        var processArchitecture = RuntimeInformation.ProcessArchitecture.ToString();
        var runtimeInfoDescription = $"Framework: {frameworkDescription}, Process Architecture: {processArchitecture}";

        return runtimeInfoDescription;
    }

    private static string GetLinuxVersion()
    {
        // Replace this with your logic to retrieve Linux version information
        return "Unknown Linux Version";
    }

    private static string GetMacVersion()
    {
        // Replace this with your logic to retrieve macOS version information
        return "Unknown macOS Version";
    }
}