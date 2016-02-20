﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace B_Priser
{
    public partial class Form1 : Form
    {
        string _logfileName = "log.txt";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            File.Delete(_logfileName);

            var d_datoArray = dateTimePicker1.Text.Split('.');
            var r_datoArray = dateTimePicker2.Text.Split('.');

            var dayStart = d_datoArray[0];
            var yearMonthStart = d_datoArray[2] + d_datoArray[1];

            var dayReturn = r_datoArray[0];
            var yearMonthReturn = r_datoArray[2] + r_datoArray[1];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                "http://www.norwegian.no/fly/velg-flyvning/?D_City=OSLALL&A_City=KRS&TripType=2&D_Day=" 
                + dayStart + "&D_Month=" + yearMonthStart + "&R_Day=" + dayReturn + "&R_Month=" + yearMonthReturn + "&AdultCount=1&ChildCount=0&InfantCount=0");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader stream = new StreamReader(response.GetResponseStream());

            string final_response = stream.ReadToEnd();

            Regex rTimes = new Regex(
                "\\d+:\\d+</div></td><td class=\"arrdest\"><div class=\"content emphasize\">\\d+:\\d+" +
                ".*fareselect standardlowfare\"><div class=\"content\" title=\"\"><label class=\"label seatsokfare\" title=\"NOK\">\\d+");

            var timeMatches = rTimes.Matches(final_response);

            using (StreamWriter writer = new StreamWriter(_logfileName, true))
            {
                foreach (Match match in timeMatches)
                {
                    string[] times = match.Value.Split(new string[] { "</div></td><td class=\"arrdest\"><div class=\"content emphasize\">", "title=\"NOK\">" }, StringSplitOptions.None);
                    writer.WriteLine("Departure: " + times[0] + " Arrival: " + times[1].Remove(5) + " Price: " + times[2]);
                }
            }


            Process.Start("notepad.exe", _logfileName);
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


    }
}
