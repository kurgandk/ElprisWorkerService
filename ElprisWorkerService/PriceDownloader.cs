using System.Text;
using System.Text.Json;


namespace ElprisWorkerService
{
    internal class PriceDownloader
    {
        /// <summary>
        /// Downloader dagens priser
        /// Dokumentet skal efterfølgende gemmes i cachen (JsonCache).
        /// Dokumentet kan bruges som input til PriceParser.
        /// 
        /// </summary>
        /// <returns>Json dokument med priser.</returns>
        public static async Task<string> GetTodaysPricesJsonAsync(string url)
        {
            HttpClient client = new();
            var response = await client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

    }
}
