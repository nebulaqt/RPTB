namespace RPTB.Remote;

public static class CheckDomain
{
    private const string BaseUrl = "http://127.0.0.1:5000/domainstatus/{0}";
    private const string ErrorMessage = "Error occurred: {0}";
    private const string StatusMessage = "Status for {0} is {1}";
    private static readonly HttpClient HttpClient = new();

    public static async Task EvaluateDomainStatusAsync(string domain)
    {
        try
        {
            var requestUrl = BuildRequestUrl(domain);

            var response = await HttpClient.GetAsync(requestUrl);
            var domainStatus = await GetDomainStatusAsync(response);

            PrintStatus(domain, domainStatus);
        }
        catch (Exception ex)
        {
            PrintError(ex);
        }
    }

    private static string BuildRequestUrl(string domain)
    {
        return BaseUrl.Replace("{0}", domain);
    }

    private static async Task<string?> GetDomainStatusAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return null;

        var responseBody = await response.Content.ReadAsStringAsync();

        return responseBody is "Online" or "Offline" ? responseBody : "Offline";
    }

    private static void PrintStatus(string domain, string? domainStatus)
    {
        Console.WriteLine(StatusMessage, domain, domainStatus);
    }

    private static void PrintError(Exception ex)
    {
        Console.WriteLine(ErrorMessage, ex.Message);
    }
}