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
        WebClient w;
        public Form1()
        {
            InitializeComponent();
        }
        string json = @"{
              'Name': 'Bad Boys',
              'ReleaseDate': '1995-4-7T00:00:00',
              'Genres': [
                'Action',
                'Comedy'
              ]
               }";

        public class Item
        {
            public int millis;
            public string stamp;
            public DateTime datetime;
            public string light;
            public float temp;
            public float vcc;
        }
        public void LoadJson()
        {
            label1.Text = "Started";
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString("https://api.forecast.io/forecast/e27b59d61b3f8e9c51f71fdc33b15018/40.935277,-80.361621");
                // Now parse with JSON.Net
            }
            using (StreamReader r = new StreamReader(json))
            {
                string jsons = r.ReadToEnd();
                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(jsons);

                dynamic array = JsonConvert.DeserializeObject(jsons);
                foreach (var item in array)
                {
                    label1.Text = String.Format("{0}", item);
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void getData()
        {
            //https://api.forecast.io/forecast/e27b59d61b3f8e9c51f71fdc33b15018/40.935277,-80.361621

             //var temp = JsonConvert.DeserializeObject(json);

            //label1.Text = temp.Name;

//            string json = @"{
//            'CPU': 'Intel',
//            'PSU': '500W',
//            'Drives': [
//              'DVD read/writer'
//             /*(broken)*/,
//             '500 gigabyte hard drive',
//             '200 gigabype hard drive'
//            ]
//        }";
            label1.Text = "Started"+Environment.NewLine;
            //w = new WebClient();
            //w.DownloadFile("https://api.forecast.io/forecast/e27b59d61b3f8e9c51f71fdc33b15018/40.935277,-80.361621", @"E:\Users\Jeremy-SSDOS\Desktop\j.txt");

            using (WebClient Client = new WebClient())
            {
                Client.DownloadFile("https://api.forecast.io/forecast/e27b59d61b3f8e9c51f71fdc33b15018/40.935277,-80.361621", "j.txt");
            }
                // Now parse with JSON.Net

            //label1.Text += File.ReadAllText(@"E:\Users\Jeremy-SSDOS\Desktop\j.txt");
            int counter = 0;
            string line;

            // Read the file and display it line by line.
            StreamReader file = new StreamReader("j.txt");
            while ((line = file.ReadLine()) != null)
            {
               label2.Text += counter.ToString()+" : "+line+Environment.NewLine;
                counter++;
            }

            file.Close();

            label1.Text += counter+" : Ended" + Environment.NewLine;
            //JsonTextReader reader = new JsonTextReader(new StringReader(@"E:\Users\Jeremy-SSDOS\Desktop\j.json"));
            //while (reader.Read())
            //{
            //    if (reader.Value != null)
            //       label1.Text += String.Format("Token: {0}, Value: {1}"+Environment.NewLine, reader.TokenType, reader.Value);
            //   else
            //        label1.Text += String.Format("Token: {0}", reader.TokenType);
            //}

            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            getData();
        }
    }
}
