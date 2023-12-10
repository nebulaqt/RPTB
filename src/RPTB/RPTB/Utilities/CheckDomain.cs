using System.Net;

namespace RPTB.Utilities;

public static class CheckDomain
{
    public static async Task CheckWebsiteStatus(string domain)
    {
        var url = $"http://localhost:5000/domainstatus/{domain}";
        using var client = new HttpClient();
        try
        {
            var response = await client.GetAsync(url);
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
        }
    }
}