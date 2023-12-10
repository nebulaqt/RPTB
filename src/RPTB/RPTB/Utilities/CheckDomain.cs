using System.Net;

namespace RPTB.Utilities;

public static class CheckDomain
{
    private const string BaseUrl = "http://localhost:5000/domainstatus/";

    public static async Task CheckAndDisplayWebsiteStatus(string domain)
    {
        var url = BaseUrl + domain;
        using var client = new HttpClient();
        try
        {
            var response = await client.GetAsync(url);
            await DisplayDomainStatus(response, domain);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
        }
    }

    private static async Task DisplayDomainStatus(HttpResponseMessage response, string domain)
    {
        var domainStatus = "Offline"; // Default State
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody is "Online" or "Offline") domainStatus = responseBody;
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine("Request failed with status code: NotFound");
            return; // return early as site was not available
        }

        Console.WriteLine($"Status for {domain} is {domainStatus}");
    }
}