using System;
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
using System.Data.SqlClient;
using System.Configuration;

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

            var request = (HttpWebRequest)WebRequest.Create(
                "http://www.norwegian.no/fly/velg-flyvning/?D_City=OSLALL&A_City=KRS&TripType=2&D_Day=" 
                + dayStart + "&D_Month=" + yearMonthStart + "&R_Day=" + dayReturn + "&R_Month=" + yearMonthReturn + "&AdultCount=1&ChildCount=0&InfantCount=0");
            var response = (HttpWebResponse)request.GetResponse();

            var stream = new StreamReader(response.GetResponseStream());

            string final_response = stream.ReadToEnd();

            var rTimes = new Regex(
                "\\d+:\\d+</div></td><td class=\"arrdest\"><div class=\"content emphasize\">\\d+:\\d+" +
                ".*fareselect standardlowfare\"><div class=\"content\" title=\"\"><label class=\"label seatsokfare\" title=\"NOK\">\\d+");

            var rDestinations = new Regex(
                "[A-Åa-å -]*</div></td><td class=\"arrdest\"><div class=\"content\">[A-Åa-å -]*");

            var timeMatches = rTimes.Matches(final_response);
            var destinationMatches = rDestinations.Matches(final_response);

            if (timeMatches.Count != destinationMatches.Count)
            {
                labelError.Text = "CriticalError: timeMatches != destinationMatches";
                return;
            }

            using (var writer = new StreamWriter(_logfileName, true))
            {
                var previousStartPlace = string.Empty;
                for (var i = 0; i < timeMatches.Count; i++)
                {

                    var times = timeMatches[i].Value.Split(new string[] { "</div></td><td class=\"arrdest\"><div class=\"content emphasize\">", "title=\"NOK\">" }, StringSplitOptions.None);
                    var destinations = destinationMatches[i].Value.Split(new string[] { "</div></td><td class=\"arrdest\"><div class=\"content\">" }, StringSplitOptions.None);
                    var departureTimeString = times[0];
                    var destinationTimeString = times[1].Remove(5);
                    var departureTimeConverted = Convert.ToDateTime(departureTimeString);
                    var destinationTimeConverted = Convert.ToDateTime(destinationTimeString);
                    var departureDateTime = dateTimePicker1.Value.AddHours(departureTimeConverted.Hour).AddMinutes(departureTimeConverted.Minute);
                    var destinationDateTime = dateTimePicker2.Value.AddHours(destinationTimeConverted.Hour).AddMinutes(destinationTimeConverted.Minute);
                    var flightTime = destinationTimeConverted - departureTimeConverted;
                    var price = times[2];
                    var departurePlace = destinations[0];
                    var destinationPlace = destinations[1];

                    if (!string.IsNullOrEmpty(previousStartPlace) && previousStartPlace != departurePlace)
                        writer.WriteLine("--------------------------------------------------------------------------------------------------------");
                    previousStartPlace = departurePlace;
                    writer.WriteLine(departurePlace + " ---> " + destinationPlace + " Departure: " + departureTimeString + " Arrival: " + destinationTimeString + " Price: " + price);

                    using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["dbCon"].ConnectionString))
                    {
                        connection.Open();
                        string sql = "INSERT INTO Flight (DeparturePlace, DestinationPlace, DepartureTime, DestinationTime, FlightTime, Price) VALUES(@param1,@param2,@param3,@param4,@param5,@param6)";
                            SqlCommand cmd = new SqlCommand(sql, connection);
                            cmd.Parameters.AddWithValue("@param1", departurePlace);
                            cmd.Parameters.AddWithValue("@param2", destinationPlace);
                            cmd.Parameters.AddWithValue("@param3", departureDateTime);
                            cmd.Parameters.AddWithValue("@param4", destinationDateTime);
                            cmd.Parameters.AddWithValue("@param5", flightTime);
                            cmd.Parameters.AddWithValue("@param6", price);
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                    }

                }
            }

            Process.Start("notepad.exe", _logfileName);
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


    }
}
