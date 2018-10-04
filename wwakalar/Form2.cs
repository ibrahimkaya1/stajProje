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

        public void LabelYazdir(string proje, int i, string ilktarih, string sontarih)
        {
            con.Open();

            string secmeSorgusu = "SELECT SUM(duration) from Durations where project=@proje";
            SqlCommand command = new SqlCommand(secmeSorgusu, con);
            command.Parameters.AddWithValue("@proje", proje);
            string d_toplam = (command.ExecuteScalar()).ToString();
            
            //Console.WriteLine(d_toplam);

            string secmeSorgusu3 = "SELECT count(DISTINCT project) from Durations";
            SqlCommand command3 = new SqlCommand(secmeSorgusu3, con);
            string sayi = (command3.ExecuteScalar()).ToString();

            string secmeSorgusu2 = "SELECT SUM(duration) from Durations";
            SqlCommand command2 = new SqlCommand(secmeSorgusu2, con);
            string t_toplam = (command2.ExecuteScalar()).ToString();

            con.Close();

            int proje_sayisi = Convert.ToInt32(sayi);
            //  Console.Write(d_toplam);
            //     Grafik_bilgileri(proje_sayisi, i,proje,t_toplam,d_toplam);

            float[] deger = new float[proje_sayisi];
            float[] proje_sure = new float[proje_sayisi];
            proje_sure[i] = Convert.ToInt32(d_toplam);

            string[] proje_ismi = new string[proje_sayisi];
            proje_ismi[i] = proje;

            //Console.WriteLine(proje_ismi[i] + " " + proje_sure[i]);
            con.Open();

            string kontrol = "select * from top_duration where proje_ismi=@proje";
            SqlCommand kontrol_cmd = new SqlCommand(kontrol,con);
            kontrol_cmd.Parameters.AddWithValue("@proje",proje_ismi[i]);

            //Console.WriteLine(kontrol_cmd);

            if (kontrol_cmd!=null)
            {
         
                string silSorgusu = "Delete from top_duration where proje_ismi=@proje";
                SqlCommand kayit_sil = new SqlCommand(silSorgusu, con);
                kayit_sil.Parameters.AddWithValue("@proje", proje_ismi[i]);
                kayit_sil.ExecuteNonQuery();

                SqlCommand sorgu_top = new SqlCommand("INSERT into top_duration(proje_ismi, toplam_duration)values" +
            " ('" + proje_ismi[i] + "','" + proje_sure[i] + "')", con);
                sorgu_top.ExecuteNonQuery();
            }

            else
            {

                SqlCommand sorgu_top = new SqlCommand("INSERT into top_duration(proje_ismi, toplam_duration)values" +
            " ('" + proje_ismi[i] + "','" + proje_sure[i] + "')", con);
                sorgu_top.ExecuteNonQuery();
             
            }

           
        
            con.Close();

            float toplam = Convert.ToInt32(t_toplam);
            Color[] colorSet = { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Pink,
                Color.Orange, Color.MintCream, Color.MidnightBlue, Color.Purple, Color.PapayaWhip, Color.PaleGreen , Color.Magenta,Color.DarkCyan,Color.MediumSeaGreen,Color.Plum,Color.Blue,Color.Chocolate};
            deger[i] = (proje_sure[i] / toplam) * 360;
            chart1.Series["Grafik"].Points.Add(deger[i]);
            chart1.Series["Grafik"].Points[i].Label = proje_ismi[i];
            chart1.Series["Grafik"].Points[i].Color = colorSet[i];

            //  chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -70;
            chart1.BackColor = Color.YellowGreen;
           Grafik_olustur();
        }
        public void Grafik_olustur()
        {
            con.Open();

            string veri_sirala = "select * from top_duration ORDER BY toplam_duration desc";
            SqlCommand veri_cmd = new SqlCommand(veri_sirala, con);
            veri_cmd.ExecuteNonQuery();
            con.Close();
        }

        int t = 1;

        private void button1_Click(object sender, EventArgs e)
        {
           
            if (t == 1)
            {
                string ilktarih = dateTimePicker1.Value.ToString("yyyy-M-d");
                string sontarih = dateTimePicker2.Value.ToString("yyyy-M-d");
                Console.WriteLine(ilktarih + " " + sontarih);
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

                    LabelYazdir(liste[i], i, ilktarih, sontarih);

                }

                t = 2;
            }

        }
        
        private void Form2_Load(object sender, EventArgs e)
        {

        }

    }
}

