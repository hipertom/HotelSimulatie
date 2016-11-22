using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelSim
{
    public class ConfigJsonModel
    {
        // config shizzle
        // HTE te ont houden tijden
        public int HtesPerSecond { get; set; }
        public int QueueDeathHTE { get; set; }
        public int StairDistanceHTE { get; set; }
        public int RoomCleaningHTE { get; set; }
        public int MovieDurationHTE { get; set; }
        public int EatHTE { get; set; }
        // niet in options scherm
        // staat nu nog altijd op 1
        public double ElevatorDistanceHTE { get; set; }


        public ConfigJsonModel()
        {
            ElevatorDistanceHTE = 1;
        }

        /// <summary>
        /// Leest het config json bestand uit op basis van een gegeven pad
        /// </summary>
        /// <param name="path">Pad naar het bestand op de computer (standaard app-data)</param>
        /// <returns>returns bool op basis van try/ catch, bij false wordt de exception naar console geschreven</returns>
        public bool WriteConfigJson(string path)
        {
            try
            {
                string json = JsonConvert.SerializeObject(this);
                System.IO.File.WriteAllText(path, json);
                Console.WriteLine("Config opgeslagen op: " + path);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        /// <summary>
        /// Vul het hudige object met data uit het bestand
        /// </summary>
        /// <param name="path">string pad naar het bestand</param>
        public void ReadConfigJson(string path)
        {
            try
            {
                StreamReader r = new StreamReader(path);
                string json = r.ReadToEnd();
                r.Close();
                JsonConvert.PopulateObject(json, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
