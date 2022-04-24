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

    public class Rootobject
    {
        public string help { get; set; }
        public bool success { get; set; }
        public Result result { get; set; }
    }

    public class Result
    {
        public Record[] records { get; set; }
        public Field[] fields { get; set; }
        public string sql { get; set; }
    }

    public class Record
    {
        public float SpotPriceEUR { get; set; }
        public DateTime HourUTC { get; set; }
        public DateTime HourDK { get; set; }
        public string _full_text { get; set; }
        public int _id { get; set; }
        public string PriceArea { get; set; }
        public float? SpotPriceDKK { get; set; }
    }

    public class Field
    {
        public string type { get; set; }
        public string id { get; set; }
    }

}
