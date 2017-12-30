using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferWeatherInformation
{
    [Serializable]
    public class Weather
    {
        [JsonProperty("Temperature")]
        public string Temperature { get; set; }

        [JsonProperty("Condition")]
        public string Condition { get; set; }

        [JsonProperty("Humidity")]
        public string Humidity { get; set; }

        [JsonProperty("WinSpeed")]
        public string WinSpeed { get; set; }

        [JsonProperty("Town")]
        public string Town { get; set; }

        [JsonProperty("TFCond")]
        public string TFCond { get; set; }

        [JsonProperty("TFHigh")]
        public string TFHigh { get; set; }

        [JsonProperty("TFLow")]
        public string TFLow { get; set; }
    }
}
