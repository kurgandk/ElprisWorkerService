using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElprisWorkerService.Models
{
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
