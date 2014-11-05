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
        string location;
        int updateCounter = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            getData();
        }

        void getData()
        {
            json = "";
            json2 = "";
            json3 = "";
            using (WebClient Client2 = new WebClient()) // get location
            {
                Client2.DownloadFile("http://api.wunderground.com/api/b0ad6743f596f56f/geolookup/q/autoip.json", "j2.txt");

                int counter2 = 0;
                string line2;

                // Read the file and display it line by line.
                StreamReader file2 = new StreamReader("j2.txt");
                while ((line2 = file2.ReadLine()) != null)
                {
                    json2 += line2;
                    counter2++;
                }

                file2.Close();

                var results = JsonConvert.DeserializeObject<dynamic>(json2);
                var id = results.location;
                var mresults = JsonConvert.DeserializeObject<dynamic>(id.ToString());
                location = mresults.lat+","+mresults.lon;
                zipcode = mresults.zip;
                state = mresults.state;
                city = mresults.city;

                label1.Text = city +", "+state+ " "+zipcode +Environment.NewLine+location;
            }
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
            using (WebClient Client3 = new WebClient()) //get weather data from location -- forecast.IO
            {
                Client3.DownloadFile("https://api.forecast.io/forecast/e27b59d61b3f8e9c51f71fdc33b15018/"+location, "j3.txt");
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

                var results = JsonConvert.DeserializeObject<dynamic>(json3);
                var id = results.currently;
                var mresults = JsonConvert.DeserializeObject<dynamic>(id.ToString());
                var summary = mresults.summary;
                var temperature = mresults.temperature;
                var humidity = mresults.humidity;

                label5.Text = summary + Environment.NewLine +
                    temperature + Environment.NewLine +
                    humidity;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            getData();
            updateCounter++;
            this.Text = updateCounter.ToString();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            getData();
            updateCounter++;
            this.Text = updateCounter.ToString();
        }
    }
}
