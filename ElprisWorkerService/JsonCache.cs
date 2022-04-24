using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElprisWorkerService
{
    /// <summary>
    ///  Meget simpel cache som opbevarer præcis én json fil i cache hver dag.
    ///  Hvis du får brug for at se, hvor cache-filen ligger (så du evt kan slette den), kan du kalde
    ///  metode GetFilename 
    /// 
    /// </summary>
    internal class JsonCache
    {
        /// <summary>
        /// Gemmer et json dokument i cache
        /// </summary>
        /// <param name="json">det dokument der skal gemmes</param>
        public static void Put(string json)
        {
            Console.WriteLine("Writing cache");

            string filename = GetFilename();
            File.WriteAllText(filename, json);
        }

        public static string GetFilename()
        {
            return $"elpris{DateOnly.FromDateTime(DateTime.Now):MM-dd-yy}.json";
        }

        /// <summary>
        /// Henter dagens json dokument fra cachen
        /// </summary>
        /// <returns>json elementet hvis det findes. null hvis der ikke er et dokument i cachen</returns>
        public static string? Get()
        {
            var filename = GetFilename();
            if (!File.Exists(filename))
            {
                Console.WriteLine("No cache");
                return null;
            }
            Console.WriteLine("Reading cache");

            return File.ReadAllText(filename);
        }
    }
}
