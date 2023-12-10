using System.Runtime.InteropServices;

namespace RPTB.Utilities;

public static class SystemInfo
{
    public static string GetOperatingSystemInfo()
    {
        var versionString = Environment.OSVersion.ToString();
        var description = RuntimeInformation.OSDescription;
        var architecture = RuntimeInformation.OSArchitecture.ToString();
        var systemInfoDescription = $"OS Version: {versionString}, Description: {description}, Architecture: {architecture}";

        return systemInfoDescription;
    }
    public static string GetRuntimeInfo()
    {
        var frameworkDescription = RuntimeInformation.FrameworkDescription;
        var processArchitecture = RuntimeInformation.ProcessArchitecture.ToString();
        var runtimeInfoDescription = $"Framework: {frameworkDescription}, Process Architecture: {processArchitecture}";

        return runtimeInfoDescription;
    }
}