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
        string xml;
        string zipcode;
        string state;
        string city;
        string lat;
        string lon;
        string location;
        string WOEID;
        int updateCounter = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            getLocation();
            getData();
        }
        void getLocation()
        {
            json2 = "";
            xml = "";
            using (WebClient Client = new WebClient()) // get location weatherunderground
            {
                Client.DownloadFile("http://api.wunderground.com/api/b0ad6743f596f56f/geolookup/q/autoip.json", "j2.txt");

                int counter = 0;
                string line;

                // Read the file and display it line by line.
                StreamReader file = new StreamReader("j2.txt");
                while ((line = file.ReadLine()) != null)
                {
                    json2 += line;
                    counter++;
                }

                file.Close();

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
            using (WebClient Client = new WebClient()) // get location yahoo with WOEID
            {
                Client.DownloadFile("http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20geo.placefinder%20where%20text%3D%22"+lat+"%2C"+lon+"%22%20and%20gflags%3D%22R%22", "x.xml");

                int counter = 0;
                string line;

                // Read the file and display it line by line.
                StreamReader file = new StreamReader("x.xml");
                while ((line = file.ReadLine()) != null)
                {
                    xml += line;
                    counter++;
                }

                file.Close();

                XmlDocument doc = new XmlDocument();
                doc.Load("x.xml");

                XmlNode node = doc.SelectSingleNode("query");
                XmlNode node2 = node.SelectSingleNode("results");
                XmlNode node3 = node2.SelectSingleNode("Result");
                WOEID = node3.SelectSingleNode("woeid").InnerText;
                label1.Text += WOEID;
            }
        }
        void getData()
        {
            json = "";
            json3 = "";
            using (WebClient Client = new WebClient()) //get weather data from location -- weather underground
            {
                //Client.DownloadFile("https://api.forecast.io/forecast/e27b59d61b3f8e9c51f71fdc33b15018/"+location, "j.txt");
                Client.DownloadFile("http://api.wunderground.com/api/b0ad6743f596f56f/conditions/q/"+state+"/"+city+".json", "j.txt");
                int counter = 0;
                string line;

                // Read the file and display it line by line.
                StreamReader file = new StreamReader("j.txt");
                while ((line = file.ReadLine()) != null)
                {
                    json += line;
                    counter++;
                }

                file.Close();

                var results = JsonConvert.DeserializeObject<dynamic>(json);
                var id = results.current_observation;
                var mresults = JsonConvert.DeserializeObject<dynamic>(id.ToString());
                var summary = mresults.weather;
                var temperature = mresults.temperature_string;
                var humidity = mresults.relative_humidity;

                label2.Text = summary + Environment.NewLine +
                    temperature + Environment.NewLine +
                    humidity;
            }
            using (WebClient Client = new WebClient()) //get weather data from location -- forecast.IO
            {
                Client.DownloadFile("https://api.forecast.io/forecast/e27b59d61b3f8e9c51f71fdc33b15018/"+location, "j3.txt");
                //Client.DownloadFile("http://api.wunderground.com/api/b0ad6743f596f56f/conditions/q/" + state + "/" + city + ".json", "j.txt");
                int counter = 0;
                string line;

                // Read the file and display it line by line.
                StreamReader file = new StreamReader("j3.txt");
                while ((line = file.ReadLine()) != null)
                {
                    json3 += line;
                    counter++;
                }

                file.Close();
                try
                {
                    var results = JsonConvert.DeserializeObject<dynamic>(json3);
                    var id = results.currently;
                    var mresults = JsonConvert.DeserializeObject<dynamic>(id.ToString());
                    var summary = mresults.summary;
                    var temperature = mresults.temperature;
                    string humidity = mresults.humidity;
                    string h2 = (float.Parse(humidity, CultureInfo.CurrentCulture)*100).ToString("f0") + "%";
                    label5.Text = summary + Environment.NewLine +
                        temperature+"°F" + Environment.NewLine +
                        h2;
                }
                catch
                {
                    label5.Text = "failed";
                }
            }
            String query = String.Format("http://weather.yahooapis.com/forecastrss?w=" + WOEID);
            XmlDocument d = new XmlDocument();
            d.Load(query);

            XmlNamespaceManager m = new XmlNamespaceManager(d.NameTable);
            m.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

            XmlNode channel = d.SelectSingleNode("rss").SelectSingleNode("channel");
            XmlNodeList nodes = d.SelectNodes("rss/channel/item/yweather:forecast", m);

            string condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", m).Attributes["text"].Value;
            string t = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", m).Attributes["temp"].Value + "°F";
            string h = channel.SelectSingleNode("yweather:atmosphere", m).Attributes["humidity"].Value + "%";

            label6.Text = condition + Environment.NewLine + t + Environment.NewLine + h;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            getData();
            updateCounter++;
            this.Text = updateCounter.ToString();
        }
    }
}
