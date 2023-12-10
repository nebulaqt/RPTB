using System.Runtime.InteropServices;

namespace RPTB.Utilities;

public class SystemInfo
{
    public static string GetOperatingSystemInfo()
    {
        var versionString = Environment.OSVersion.ToString();
        var description = RuntimeInformation.OSDescription;
        var architecture = RuntimeInformation.OSArchitecture.ToString();

        return $"OS Version: {versionString}, Description: {description}, Architecture: {architecture}";
    }

    public static string GetRuntimeInfo()
    {
        var frameworkDescription = RuntimeInformation.FrameworkDescription;
        var processArchitecture = RuntimeInformation.ProcessArchitecture.ToString();

        return $"Framework: {frameworkDescription}, Process Architecture: {processArchitecture}";
    }
}