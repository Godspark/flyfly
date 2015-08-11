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

namespace B_Priser
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] d_datoArray = dateTimePicker1.Text.Split('.');
            string[] r_datoArray = dateTimePicker2.Text.Split('.');
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.norwegian.no/fly/velg-flyvning/?D_City=OSLALL&A_City=KRS&TripType=2&D_Day=" + d_datoArray[0] + "&D_Month=" + d_datoArray[1] + "&R_Day=" + r_datoArray[0] + "&R_Month=" + r_datoArray[1] + "&AdultCount=1&ChildCount=0&InfantCount=0");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader stream = new StreamReader(response.GetResponseStream());

            string final_response = stream.ReadToEnd();

            using (StreamWriter writer = new StreamWriter("log.txt"))
            {
                writer.Write(final_response);
            }

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        
    }
}
