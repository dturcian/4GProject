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
        private SqlConnection conn;
        private const string connectionString = "Server=tcp:4gserverdatc.database.windows.net,1433;Initial Catalog=4G;Persist Security Info=False;User ID=davidtorje;Password=4g@datc1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void Process()
        {
            while (true)
            {
                //collects weather information from different cities
                var zona1 = weatherInformation("timisoara/ro/");
                var zona2 = weatherInformation("iasi/ro/");
                var zona3 = weatherInformation("brasov/ro/");
                var zona4 = weatherInformation("bucuresti/ro/");
                var zona5 = weatherInformation("deva/ro/");
                var zona6 = weatherInformation("constanta/ro/");
                var zona7 = weatherInformation("cluj/ro/");

                OpenConnDB();

                //save the informations in DB
                SaveInfomationInDB(zona1, 1);
                SaveInfomationInDB(zona2, 2);
                SaveInfomationInDB(zona3, 3);
                SaveInfomationInDB(zona4, 4);
                SaveInfomationInDB(zona5, 5);
                SaveInfomationInDB(zona6, 6);
                SaveInfomationInDB(zona7, 7);

                CloseConnDB();

                //every 10 minutes collects weather information
                Thread.Sleep(10 * 60 * 1000);
            }
        }

        public void OpenConnDB()
        {
            conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            conn.Open();
        }

        public void CloseConnDB()
        {
            conn.Close();
        }

        //save informations in DB
        public void SaveInfomationInDB(Weather zona, int nrZona)
        {
            SqlCommand command = new SqlCommand("INSERT INTO WeatherInformationTable VALUES " +
                "(@nrZona, @zonaTemperature, @zonaHumidity, @zonaDate, @zonaCondition, " +
                "@zonaWinSpeed, @zonaTown, @zonaTFCond, @zonaTFHigh, @zonaTFLow)", conn);
            // Add the parameters.
            command.Parameters.Add(new SqlParameter("nrZona", nrZona));
            command.Parameters.Add(new SqlParameter("zonaTemperature", VerificareDate(FahrenheitToCelsius(int.Parse(zona.Temperature)), "temperature")));
            command.Parameters.Add(new SqlParameter("zonaHumidity", VerificareDate(int.Parse(zona.Humidity), "humidity")));
            command.Parameters.Add(new SqlParameter("zonaDate", DateTime.Now));
            command.Parameters.Add(new SqlParameter("zonaCondition", zona.Condition));
            command.Parameters.Add(new SqlParameter("zonaWinSpeed", VerificareDate(int.Parse(zona.WinSpeed), "winspeed"))).ToString();
            command.Parameters.Add(new SqlParameter("zonaTown", zona.Town));
            command.Parameters.Add(new SqlParameter("zonaTFCond", zona.TFCond));
            command.Parameters.Add(new SqlParameter("zonaTFHigh", VerificareDate(FahrenheitToCelsius(int.Parse(zona.TFHigh)), "temperature")));
            command.Parameters.Add(new SqlParameter("zonaTFLow", VerificareDate(FahrenheitToCelsius(int.Parse(zona.TFLow)), "temperature")));
            
            command.ExecuteNonQuery();
        }

        const int errorCodeParameterWeather = 777;
        public int VerificareDate(int parameterWeather, string typeParameter)
        {
            switch(typeParameter)
            {
                case "temperature":
                    if(parameterWeather > 65 || parameterWeather < -95)
                    {
                        return errorCodeParameterWeather;
                    }
                    else
                    {
                        return parameterWeather;
                    }
                case "humidity":
                    if (parameterWeather > 100 || parameterWeather < 0)
                    {
                        return errorCodeParameterWeather;
                    }
                    else
                    {
                        return parameterWeather;
                    }
                case "winspeed":
                    if (parameterWeather > 650 || parameterWeather < 0)
                    {
                        return errorCodeParameterWeather;
                    }
                    else
                    {
                        return parameterWeather;
                    }
                default:
                    return parameterWeather;
            }
        }

        //conversion from Fahrenheit to Celsius
        public int FahrenheitToCelsius(int tempFahrenheit)
        {
            int tempCelsius = Convert.ToInt16((tempFahrenheit - 32) / 1.8);

            return tempCelsius;
        }

        //collects weather information
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