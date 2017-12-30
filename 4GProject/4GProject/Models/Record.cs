using System;

namespace _4GProject.Models
{
    public class Record
    {
        public string Temperature { get; set; }
        public string Condition { get; set; }
        public string Humidity { get; set; }
        public string WinSpeed { get; set; }
        public string Town { get; set; }
        public string TFCond { get; set; }
        public string TFHigh { get; set; }
        public string TFLow { get; set; }

        public int Zone { get; set; }
        public DateTime Date { get; set; }
    }
}