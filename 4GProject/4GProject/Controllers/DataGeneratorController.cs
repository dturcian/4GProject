using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using _4GProject.Models;

namespace _4GProject.Controllers
{
    public class DataGeneratorController : ApiController
    {
        private SqlConnection _conn;
        private const string ConnectionString = "Server=tcp:4gserverdatc.database.windows.net,1433;Initial Catalog=4G;Persist Security Info=False;User ID=davidtorje;Password=4g@datc1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private List<Record> _records;

        [HttpGet]
        [Route("4gapi/records/{max}")]
        public HttpResponseMessage GetLastRecords(string max)
        {
            var top = string.Empty;
            int result;
            if (int.TryParse(max, out result))
                top = "TOP " + result;

            _records = new List<Record>();

            _conn = new SqlConnection
            {
                ConnectionString = ConnectionString
            };
            try
            {
                _conn.Open();

                var command = new SqlCommand("SELECT " + top + " * FROM WeatherInformationTable ORDER BY Date DESC", _conn);

                var reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        _records.Add(new Record()
                        {
                            Zone = (int)reader["Zone"],
                            Temperature = reader["Temperature"].ToString(),
                            Humidity = reader["Humidity"].ToString(),
                            Date = (DateTime)reader["Date"],
                            Condition = reader["Condition"].ToString(),
                            Town = reader["Town"].ToString(),
                            WinSpeed = reader["WinSpeed"].ToString(),
                            TFCond = reader["TFCond"].ToString(),
                            TFHigh = reader["TFHigh"].ToString(),
                            TFLow = reader["TFLow"].ToString()
                        });
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            finally
            {
                _conn.Close();
            }

            var json = JsonConvert.SerializeObject(_records);
            return new HttpResponseMessage()
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        [Route("4gapi/weather/{town}/{country}")]
        public HttpResponseMessage GetWeatherByTown(string town, string country)
        {
            var wObj = new Weather();
            try
            {
                var query = String.Format("https://query.yahooapis.com/v1/public/yql?q=select * from weather.forecast where woeid in (select woeid from geo.places(1) where text='{0}, {1}')&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys", town, country);

                XmlDocument wData = new XmlDocument();
                wData.Load(query);

                XmlNamespaceManager man = new XmlNamespaceManager(wData.NameTable);
                man.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

                XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");
                XmlNodeList nodes = wData.SelectNodes("/query/results/channel");

                wObj.Temperature = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", man).Attributes["temp"].Value;

                wObj.Condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", man).Attributes["text"].Value;

                wObj.Humidity = channel.SelectSingleNode("yweather:atmosphere", man).Attributes["humidity"].Value;

                wObj.WinSpeed = channel.SelectSingleNode("yweather:wind", man).Attributes["speed"].Value;

                wObj.Town = channel.SelectSingleNode("yweather:location", man).Attributes["city"].Value;

                wObj.TFCond = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", man).Attributes["text"].Value;

                wObj.TFHigh = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", man).Attributes["high"].Value;

                wObj.TFLow = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", man).Attributes["low"].Value;

            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            var json = JsonConvert.SerializeObject(wObj);
            return new HttpResponseMessage()
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        [Route("4gapi/weather/{longitude:decimal}/{latitude:decimal}/")]
        public HttpResponseMessage GetWeather(decimal longitude, decimal latitude)
        {
            var wObj = new Weather();
            try
            {
                var query = string.Format("https://query.yahooapis.com/v1/public/yql?q=select * from weather.forecast where woeid in (select woeid from geo.places where text='({0},{1})')&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys", longitude, latitude);
                //String query = String.Format("https://query.yahooapis.com/v1/public/yql?q=select * from weather.forecast where woeid in (select woeid from geo.places(1) where text='deva, ro')&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
                //select * from geo.places where text="(45.788521,21.29394)"

                XmlDocument wData = new XmlDocument();
                wData.Load(query);

                XmlNamespaceManager man = new XmlNamespaceManager(wData.NameTable);
                man.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

                XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");
                XmlNodeList nodes = wData.SelectNodes("/query/results/channel");

                wObj.Temperature = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", man).Attributes["temp"].Value;

                wObj.Condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", man).Attributes["text"].Value;

                wObj.Humidity = channel.SelectSingleNode("yweather:atmosphere", man).Attributes["humidity"].Value;

                wObj.WinSpeed = channel.SelectSingleNode("yweather:wind", man).Attributes["speed"].Value;

                wObj.Town = channel.SelectSingleNode("yweather:location", man).Attributes["city"].Value;

                wObj.TFCond = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", man).Attributes["text"].Value;

                wObj.TFHigh = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", man).Attributes["high"].Value;

                wObj.TFLow = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", man).Attributes["low"].Value;

            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            var json = JsonConvert.SerializeObject(wObj);
            return new HttpResponseMessage()
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };

        }

        [HttpGet]
        [Route("4gapi")]
        public IHttpActionResult GetInfo()
        {
            var responseMsg = new HttpResponseMessage(HttpStatusCode.RedirectMethod);
            responseMsg.Headers.Location = new Uri("/Api.html", UriKind.Relative);
            IHttpActionResult response = ResponseMessage(responseMsg);
            return response;
        }
    }
}