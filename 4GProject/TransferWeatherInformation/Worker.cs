using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Threading;

namespace TransferWeatherInformation
{
    class Worker
    {
        private SqlConnection _conn;
        private SqlTransaction _trans;
        private const string ConnectionString = "Server=tcp:4gserverdatc.database.windows.net,1433;Initial Catalog=4G;Persist Security Info=False;User ID=davidtorje;Password=4g@datc1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void Process()
        {
            while (true)
            {
                var zona1 = weatherInformation("timisoara/ro/");
                var zona2 = weatherInformation("iasi/ro/");
                var zona3 = weatherInformation("brasov/ro/");
                var zona4 = weatherInformation("bucuresti/ro/");
                var zona5 = weatherInformation("deva/ro/");
                var zona6 = weatherInformation("constanta/ro/");
                var zona7 = weatherInformation("cluj/ro/");

                OpenConnDB();
                _trans = _conn.BeginTransaction();

                try
                {
                    SaveInfomationInDB(zona1, 1);
                    SaveInfomationInDB(zona2, 2);
                    SaveInfomationInDB(zona3, 3);
                    SaveInfomationInDB(zona4, 4);
                    SaveInfomationInDB(zona5, 5);
                    SaveInfomationInDB(zona6, 6);
                    SaveInfomationInDB(zona7, 7);

                    _trans.Commit();
                }
                catch (Exception ex)
                {
                    _trans.Rollback();
                    Console.WriteLine(ex.ToString());
                }

                CloseConnDB();

                Thread.Sleep(5 * 60 * 1000);
            }
        }

        public void OpenConnDB()
        {
            _conn = new SqlConnection();
            _conn.ConnectionString = ConnectionString;
            _conn.Open();
        }

        public void CloseConnDB()
        {
            _conn.Close();
        }

        public void SaveInfomationInDB(Weather zona, int nrZona)
        {
            SqlCommand command = new SqlCommand("INSERT INTO WeatherInformationTable VALUES " +
                "(@nrZona, @zonaTemperature, @zonaHumidity, @zonaDate, @zonaCondition, " +
                "@zonaWinSpeed, @zonaTown, @zonaTFCond, @zonaTFHigh, @zonaTFLow)", _conn, _trans);
            // Add the parameters.
            command.Parameters.Add(new SqlParameter("nrZona", nrZona));
            command.Parameters.Add(new SqlParameter("zonaTemperature", int.Parse(zona.Temperature)));
            command.Parameters.Add(new SqlParameter("zonaHumidity", int.Parse(zona.Humidity)));
            command.Parameters.Add(new SqlParameter("zonaDate", DateTime.Now));
            command.Parameters.Add(new SqlParameter("zonaCondition", zona.Condition));
            command.Parameters.Add(new SqlParameter("zonaWinSpeed", zona.WinSpeed));
            command.Parameters.Add(new SqlParameter("zonaTown", zona.Town));
            command.Parameters.Add(new SqlParameter("zonaTFCond", zona.TFCond));
            command.Parameters.Add(new SqlParameter("zonaTFHigh", int.Parse(zona.TFHigh)));
            command.Parameters.Add(new SqlParameter("zonaTFLow", int.Parse(zona.TFLow)));
            
            command.ExecuteNonQuery();
        }

        public void VerificareDate()
        {

        }

        public Weather weatherInformation(string city)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8000/4gapi/weather/" + city);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())

            using (StreamReader reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<Weather>(json);
            }
        }
    }
}