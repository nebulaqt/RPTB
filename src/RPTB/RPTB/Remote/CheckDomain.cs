using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RPTB.Remote
{
    public static class CheckDomain
    {
        private const string BaseUrl = "http://127.0.0.1:5000/domainstatus/{0}";
        private const string ErrorMessage = "Error occurred: {0}";
        private const string StatusMessage = "Status for {0} is {1}";

        // HttpClient is designed to be reused, so it's better to create it once and reuse it.
        private static readonly HttpClient HttpClient = new HttpClient();

        // It's advisable to use Task<string> instead of Task<string?> to avoid nullable reference types.
        public static async Task EvaluateDomainStatusAsync(string domain)
        {
            try
            {
                var requestUrl = string.Format(BaseUrl, domain); // Using string interpolation is more efficient than Replace.

                var response = await HttpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode(); // Ensure the response is successful before processing.

                var domainStatus = await response.Content.ReadAsStringAsync();
                PrintStatus(domain, domainStatus);
            }
            catch (HttpRequestException ex) // Catching specific exceptions for better error handling.
            {
                PrintError(ex);
            }
            catch (Exception ex)
            {
                PrintError(new Exception(string.Format(ErrorMessage, ex.Message)));
            }
        }

        private static void PrintStatus(string domain, string domainStatus)
        {
            Console.WriteLine(StatusMessage, domain, domainStatus);
        }

        private static void PrintError(Exception ex)
        {
            Console.WriteLine(ErrorMessage, ex.Message);
        }
    }
}
