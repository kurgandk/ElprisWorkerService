using ElprisWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElprisWorkerService
{
    internal class GetElpris
    {

        public static async Task<Record> ReturnRecordAsync()
        {
            Record returnRecord=new Record();

            HttpClient client = new HttpClient();
            string url = "https://api.energidataservice.dk/datastore_search_sql?sql=SELECT * from \"elspotprices\" WHERE \"PriceArea\"='DK1' ORDER BY \"HourDK\" DESC LIMIT 35";

            string? json = JsonCache.Get();
            if (json == null)
            {
                json = await PriceDownloader.GetTodaysPricesJsonAsync(url);
                JsonCache.Put(json);
            }
            var result = JsonSerializer.Deserialize<Rootobject>(json);

            int index = Array.FindIndex(result.result.records, HourIsNow);

            // find seneste pris
            returnRecord = result.result.records[index];
            float EURDKK = 7.45f;
            // hvis DK spotpris mangler, så regn en ud.
            returnRecord.SpotPriceDKK=spotprisDkk(returnRecord.SpotPriceEUR,returnRecord.SpotPriceDKK,EURDKK);

            return returnRecord;

            // udregn dansk spotpris hvis den er tom
            float spotprisDkk(float spotprisEur, float? spotprisDk, float eurdkkkurs)
            {
                return (spotprisDk ?? (spotprisEur * eurdkkkurs)) / 1000f;
            }

            bool HourIsNow(Record thisrecord)
            {
                DateTime now = DateTime.Now;
                TimeOnly time = new(now.Hour, 00);
                DateTime thisHour = DateOnly.FromDateTime(now).ToDateTime(time);

                if (thisrecord.HourDK == thisHour) { return true; }
                else { return false; }
            }

        }


    }


}
