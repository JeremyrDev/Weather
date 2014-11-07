using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Xml;
using System.Globalization;

namespace Weather
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string json;
        string json2;
        string json3;
        string zipcode;
        string state;
        string city;
        string lat;
        string lon;
        string location;
        string WOEID;
        int updateCounter = 0;
        int timer = 0;
        bool started = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            timer = 119;
        }
        void getLocation()
        {
            json2 = "";
            using (WebClient c = new WebClient()) // get location weatherunderground
            {
                json2 = c.DownloadString("http://api.wunderground.com/api/b0ad6743f596f56f/geolookup/q/autoip.json");

                var results = JsonConvert.DeserializeObject<dynamic>(json2);
                var id = results.location;
                var mresults = JsonConvert.DeserializeObject<dynamic>(id.ToString());
                lat = mresults.lat;
                lon = mresults.lon;
                location = mresults.lat + "," + mresults.lon;
                zipcode = mresults.zip;
                state = mresults.state;
                city = mresults.city;

                label1.Text = city + ", " + state + " " + zipcode + Environment.NewLine + location+Environment.NewLine;
            }
            //get location from yahoo with WOEID
            string query = String.Format("http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20geo.placefinder%20where%20text%3D%22" + lat + "%2C" + lon + "%22%20and%20gflags%3D%22R%22");

            XmlDocument doc = new XmlDocument();
            doc.Load(query);

            XmlNode node = doc.SelectSingleNode("query");
            XmlNode node2 = node.SelectSingleNode("results");
            XmlNode node3 = node2.SelectSingleNode("Result");
            WOEID = node3.SelectSingleNode("woeid").InnerText;
            label1.Text += WOEID;

        }
        void getData()
        {
            json = "";
            json3 = "";
            //get weatherunderground data
            try
            {
                using (var webClient = new System.Net.WebClient())
                {
                    json = webClient.DownloadString("http://api.wunderground.com/api/b0ad6743f596f56f/conditions/q/" + state + "/" + city + ".json");

                    var results = JsonConvert.DeserializeObject<dynamic>(json);
                    var id = results.current_observation;
                    var mresults = JsonConvert.DeserializeObject<dynamic>(id.ToString());
                    var summary = mresults.weather;
                    var temperature = mresults.temperature_string;
                    var humidity = mresults.relative_humidity;
                    var wind = mresults.wind_mph;
                    var feelsLike = mresults.feelslike_string;
                    var pressure = mresults.pressure_in;
                    label2.Text = summary + Environment.NewLine +
                        temperature + Environment.NewLine +
                        humidity + Environment.NewLine+
                        wind + Environment.NewLine+
                        feelsLike + Environment.NewLine+
                        pressure;
                }
            }
            catch
            {
                label2.Text = "failed";
            }

            //get forecast.IO weather data
            try
            {
                using (var webClient = new System.Net.WebClient())
                {
                    json3 = webClient.DownloadString("https://api.forecast.io/forecast/e27b59d61b3f8e9c51f71fdc33b15018/"+location);

                    var results = JsonConvert.DeserializeObject<dynamic>(json3);
                    var id = results.currently;
                    var mresults = JsonConvert.DeserializeObject<dynamic>(id.ToString());
                    var summary = mresults.summary;
                    string temperature = mresults.temperature;
                    string humidity = mresults.humidity;
                    string wind = mresults.windSpeed;
                    string pressure = mresults.pressure;
                    var h2 = (float.Parse(humidity, CultureInfo.CurrentCulture) * 100).ToString("f0") + "%";
                    
                    //Calculate windchil
                    float w = float.Parse(wind, CultureInfo.InvariantCulture);

                    float t = float.Parse(temperature, CultureInfo.CurrentCulture);

                    double wc = (35.74 + (0.6215*t)) - (35.75 * (Math.Pow(w, .016))) + ((0.4275 * t) * (Math.Pow(w, .016)));

                    string feelsLike = wc.ToString("f2")+"°F";

                    float temp = float.Parse(pressure, CultureInfo.CurrentCulture);
                    double p = temp * 0.0295301;
                    
                    label5.Text = summary + Environment.NewLine +
                        temperature + "°F" + Environment.NewLine +
                        h2 + Environment.NewLine +
                        wind +"mph" + Environment.NewLine+
                        feelsLike + Environment.NewLine +
                        p.ToString("f2");
                }
            }
            catch
            {
                label5.Text = "failed";
            }
            //get yahoo weather data
            try
            {
                
                string query = String.Format("http://weather.yahooapis.com/forecastrss?w=" + WOEID);
                XmlDocument d = new XmlDocument();
                d.Load(query);

                XmlNamespaceManager m = new XmlNamespaceManager(d.NameTable);
                m.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

                XmlNode channel = d.SelectSingleNode("rss").SelectSingleNode("channel");
                XmlNodeList nodes = d.SelectNodes("rss/channel/item/yweather:forecast", m);

                string condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", m).Attributes["text"].Value;
                string t = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", m).Attributes["temp"].Value + "°F";
                string h = channel.SelectSingleNode("yweather:atmosphere", m).Attributes["humidity"].Value + "%";
                string p = channel.SelectSingleNode("yweather:atmosphere", m).Attributes["pressure"].Value;
                string w = channel.SelectSingleNode("yweather:wind", m).Attributes["speed"].Value + "mph";
                string wc = channel.SelectSingleNode("yweather:wind", m).Attributes["chill"].Value + "°F";
                string sr = channel.SelectSingleNode("yweather:astronomy", m).Attributes["sunrise"].Value;
                string ss = channel.SelectSingleNode("yweather:astronomy", m).Attributes["sunset"].Value;

                label6.Text = condition + Environment.NewLine + t + Environment.NewLine + h +Environment.NewLine+
                    w+Environment.NewLine+
                    wc+Environment.NewLine+
                        p+Environment.NewLine+Environment.NewLine+
                        sr+Environment.NewLine+
                        ss;
            }
            catch
            {
                label5.Text = "failed";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer++;
            if (timer > 119)
            {
                if(!started)
                {
                    started = true;
                    getLocation();
                }
                getData();
                updateCounter++;
                label9.Text = updateCounter.ToString();
                timer = 0;
            }
        }
    }
}
