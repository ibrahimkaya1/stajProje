using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wwakalar
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        SqlConnection con = new SqlConnection("Server=.; Initial Catalog = WakaTime;Integrated Security=True");
        public void proje_al(string proje, int i)
        {

            con.Open();
            string secmeSorgusu = "SELECT SUM(duration) from Durations where project=@proje";
            SqlCommand command = new SqlCommand(secmeSorgusu, con);
            command.Parameters.AddWithValue("@proje",proje);
            string d_toplam = (command.ExecuteScalar()).ToString();
            Label[] lDizi = new Label[10];
            Label[] LDizi2 = new Label[10];
            lDizi[i] = new Label();
            LDizi2[i] = new Label();
            
            lDizi[i].Text = "Label" + i.ToString();
            LDizi2[i].Text = "Label" + i.ToString();

            this.Controls.Add(lDizi[i]);
            this.Controls.Add(LDizi2[i]);

            lDizi[i].Top = i * 30;
            LDizi2[i].Top = i * 30;

            lDizi[i].Left = 20;
            LDizi2[i].Left = 250;

            lDizi[i].Width = 100;
            LDizi2[i].Width = 100;

            lDizi[i].Text = proje;
            LDizi2[i].Text = d_toplam;

            con.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            con.Open();




            string projeler = "SELECT DISTINCT project from Durations";
            SqlCommand command2 = new SqlCommand(projeler, con);
            SqlDataReader drmdetayyy = command2.ExecuteReader();
            List<string> liste = new List<string>();
            while (drmdetayyy.Read())
            {

                liste.Add(drmdetayyy["project"].ToString());
        
            }
            con.Close();
            for (int i = 0; i < liste.Count; i++)
            {

                proje_al(liste[i], i);


            }
        }
    }
}

