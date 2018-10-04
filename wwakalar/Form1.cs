using System;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Win32;

namespace wwakalar
{
    public partial class Form1 : Form
    {

        //https://wakatime.com/api/v1/users/current/durations?date=2018-07-13&api_key=5fff4da8-ee27-43e6-9511-903fd88be248

        public Form1()
        {
         
            InitializeComponent();

        }
        //public SqlConnection con;
        SqlConnection con = new SqlConnection("Server=.; Initial Catalog = WakaTime;Integrated Security=True");
        private string get_data(string url)
        {

            string urlAddress = url;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
        
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                readStream = new StreamReader(receiveStream);
                string data = readStream.ReadToEnd();
              //  var cek = JsonConvert.DeserializeObject<Baslik>(data);
              //  listView1.Text = cek.Data.ToString();
                response.Close();
                readStream.Close();
               
                return data;

            }
            else
                return "Deger yok";
        }

      
        string api_key = "5fff4da8-ee27-43e6-9511-903fd88be248";
        //string api_key = "084fb563-1f53-44d0-9ff2-48f2c51d202c";

     //   string tarih = DateTime.Now.ToString();
        string veri_duration;
        string veri_heartbeats;
        string veri_user;
       
        public void Form1_Load(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            key.SetValue("ProgramAdı", "\"" + Application.ExecutablePath + "\"");

            int saat =DateTime.Now.Hour;
            int bayrak = 1;
            string tarih=Tarih_Cagir();
            /*formdan tarih aralığı alınacak ona göre ay yıl ataması yapılacak. Ama aralık girilmez ise görüntülenecek zaman  bugün veya dün olacak*/
            if ((saat == 14 || saat == 17)&bayrak==1)
            {
                veri_duration = get_data("https://wakatime.com/api/v1/users/current/durations?date=" + tarih + "&api_key=" + api_key);
                veri_heartbeats = get_data("https://wakatime.com/api/v1/users/current/heartbeats?date=" + tarih + "&api_key=" + api_key);
                veri_user = get_data("https://wakatime.com/api/v1/users/current?api_key=" + api_key);

                parse_json(veri_duration);
                parse_json2(veri_heartbeats);
                parse_json3(veri_user);
                bayrak = 0;
            }
            else
            {
                bayrak = 1;
            }
          
        }   

        public void parse_json3(string json)
        {
            //string value = string.Empty;

            JObject json_object = JObject.Parse(json);
            var json_result = json_object["data"];


                string cretad = json_result["created_at"].ToString();

                string mail = json_result["email"].ToString();
               
                string ids = json_result["id"].ToString();

                string last_heartbeat = json_result["last_heartbeat"].ToString();
                
                string last_plugin_name = json_result["last_plugin_name"].ToString();

                string last_project = json_result["last_project"].ToString();

                string photo= json_result["photo"].ToString();

                string timezone = json_result["timezone"].ToString();
               /* value = "created_at=" + cretad + "\n" + "email=" + mail + "\n" + "id=" + ids + "\n" + "last_heartbeat=" 
                    + last_heartbeat + "\n" + "last_plugin_name=" + last_plugin_name + "\n"+ "last_project="+ last_project+ "\n"+
                    "photo="+ photo+"\n"+ "timezone="+ timezone+"\n";*/

            vt_user(cretad,mail,ids,last_heartbeat,last_plugin_name,last_project,photo,timezone);
             //   Console.WriteLine(value); //burda ki çıktılar veritabanına kaydedilecek

           // return value;
        }
        public void vt_user(string created_at,string email,string id,string last_heartbeats,string last_plugin,string last_project,string photo,string timezone)
        {
            con.Open();
            string tarih = Tarih_Cagir();
            string secmeSorgusu = "SELECT * from users where vt_kayit=@tarih";
            SqlCommand secmeKomutu = new SqlCommand(secmeSorgusu, con);
            secmeKomutu.Parameters.AddWithValue("@tarih", tarih);
            if (secmeKomutu != null)
            {
                string silSorgusu = "Delete from users where vt_kayit=@tarih and created=@times";
                SqlCommand kayit_sil = new SqlCommand(silSorgusu, con);
                kayit_sil.Parameters.AddWithValue("@tarih", tarih);
                kayit_sil.Parameters.AddWithValue("@times", created_at);
                kayit_sil.ExecuteNonQuery();
            }

            SqlCommand kayitekle = new SqlCommand("insert into users(created,email,id,last_heartbeat,last_plugin,last_project,photo,timezone,vt_kayit) values" +
              " ('" + created_at + "','" + email + "','" + id + "','" + last_heartbeats + "','" +last_plugin + "','" +last_project + "','" +photo + "','" + timezone +"','" +tarih+ "')", con);
            kayitekle.ExecuteNonQuery();
            // value = "duration=" + duration + "\n" + "project=" + project + "\n" + "time=" + time + "\n"+timezone+"\n";

            con.Close();
        }
      
        public void parse_json2(string json)
        {
            //string value = string.Empty;

            JObject json_object = JObject.Parse(json);
            var json_result = json_object["data"];

            foreach (var datas in json_result)
            {
                int i = 0;

                string cretad = datas["created_at"].ToString();
                string entit = datas["entity"].ToString();
                string ids = datas["user_id"].ToString();
                string times = datas["time"].ToString();
                string types  = datas["type"].ToString();
                string katagori = datas["category"].ToString();
                string proje = datas["project"].ToString();
                string user_agent_id = datas["user_agent_id"].ToString();
                //  string branch = datas["branch"].ToString();
                string language = datas["language"].ToString();
               // value = "created_at=" + cretad + "\n"+"proje="+proje+"\n" + "language="+ language+"\n" + "entity=" + entit + "\n" + "id="+ids+"\n"
                 //   + "user_agent_id="+ user_agent_id+"\n"+ "time=" + times + "\n" +"type="+ types + "\n"+"Kategori="+katagori+"\n";

               // Console.WriteLine(value); //burda ki çıktılar veritabanına kaydedilecek

                                        vt_heartbeats(cretad, entit, ids,times,types,katagori,proje,user_agent_id,language);
                continue;
            }
            
        }
        public void vt_heartbeats(string cretad,string entit,string ids,string times,string types,string kategori,string proje, string user_agent_id,string language)
        {
            string tarih = Tarih_Cagir();
            con.Open();
            string secmeSorgusu = "SELECT * from Heartbeats where vt_kayit=@tarih";
            SqlCommand secmeKomutu = new SqlCommand(secmeSorgusu, con);
            secmeKomutu.Parameters.AddWithValue("@tarih", tarih);
            if (secmeKomutu != null)
            {
                string silSorgusu = "Delete from Heartbeats where vt_kayit=@tarih and time=@times";
                SqlCommand kayit_sil = new SqlCommand(silSorgusu, con);
                kayit_sil.Parameters.AddWithValue("@tarih", tarih);
                kayit_sil.Parameters.AddWithValue("@times", times);
                kayit_sil.ExecuteNonQuery();
            }
            SqlCommand kayitekle = new SqlCommand("insert into Heartbeats(project,created_at,entity,user_id,types,time,category,user_agent_id,language,vt_kayit) values" +
                " ( @proje,@cretad,@entit,@ids,@types,@times,@kategori,@user_agent_id,@language,@tarih)", con);
          
            kayitekle.Parameters.AddWithValue("@proje", proje);
            kayitekle.Parameters.AddWithValue("@cretad", cretad);
            kayitekle.Parameters.AddWithValue("@entit", entit);
            kayitekle.Parameters.AddWithValue("@ids", ids);
            kayitekle.Parameters.AddWithValue("@types", types);
            kayitekle.Parameters.AddWithValue("@times", times);
            kayitekle.Parameters.AddWithValue("@kategori", kategori);
            kayitekle.Parameters.AddWithValue("@user_agent_id", user_agent_id);
            kayitekle.Parameters.AddWithValue("@language", language);
            kayitekle.Parameters.AddWithValue("@tarih", tarih);
            kayitekle.ExecuteNonQuery();
            con.Close();
        }
      
        public void parse_json(string json)
        {
            JObject json_object = JObject.Parse(json);
            var json_result = json_object["data"];

        //    con.Open();
            foreach (var datas in json_result)
            {
               
                string duration = datas["duration"].ToString();
                double s_duration = Convert.ToInt32(datas["duration"]);
                
                string project = datas["project"].ToString();
                
                string time = datas["time"].ToString();
                double s_time = Convert.ToInt32(datas["time"]);

                var end = datas["end"];
                var start = datas["start"];
                var timezone = datas["timezone"];
                vt_duration(project,s_duration,s_time);
                continue;
            }
           
        }
        public void vt_duration(string projects,double s_d,double s_t)
        {
            con.Open();
            string tarih = Tarih_Cagir();
            string secmeSorgusu = "SELECT * from Durations where tarih=@tarih";
            SqlCommand secmeKomutu = new SqlCommand(secmeSorgusu , con);
            secmeKomutu.Parameters.AddWithValue("@tarih", tarih);
            if (secmeKomutu!=null)
            {
                string silSorgusu = "Delete from Durations where tarih=@tarih and time=@times";
                SqlCommand kayit_sil = new SqlCommand(silSorgusu, con);
                kayit_sil.Parameters.AddWithValue("@tarih", tarih);
                kayit_sil.Parameters.AddWithValue("@times", s_t);
                kayit_sil.ExecuteNonQuery();
            }
          
            SqlCommand kayitekle = new SqlCommand("insert into Durations(project,duration,time,tarih) values" +
              " ('" + projects +"','" + s_d + "','"+s_t+"','"+ tarih + "')", con);
            kayitekle.ExecuteNonQuery();
            // value = "duration=" + duration + "\n" + "project=" + project + "\n" + "time=" + time + "\n"+timezone+"\n";

            con.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
        public string Tarih_Cagir()
        {
            DateTime dt = DateTime.Today;
            int yil = dt.Year;
            int ay = dt.Month;
            int gun = dt.Day;
            if (gun == 1)
            {
                if (ay == 3)
                {
                    gun = 28;
                    ay = 2;
                }
                else if (ay == 5 || ay == 7 || ay == 10 || ay == 12)
                {
                    gun = 30;
                    ay = ay - 1;
                }
                else
                {
                    gun = 31;
                    if (ay == 1)
                    {
                        ay = 12;
                        yil = yil - 1;
                    }
                    else
                    {
                        ay = ay - 1;
                    }
                }
            }
            else
            {
                gun = gun - 1;

            }

            string tarih = yil + "-" + ay + "-" + gun;
            return tarih;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            con.Open();
            if (con.State == ConnectionState.Open)
            {
                MessageBox.Show("BAĞLANDI");
                con.Close();
            }
            else
            {
                MessageBox.Show("YOK");
                con.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 formkapa = new Form1();
            formkapa.Close();
            Form2 form = new Form2();
            form.Show();
            this.Hide();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                /*   notifyIcon1.Visible = true;
                   notifyIcon1.ShowBalloonTip(500);
                   this.Hide();*/
                Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.Text = "NotifyIcon Denemesi";
                notifyIcon1.BalloonTipTitle = "Program Çalışıyor";
                notifyIcon1.BalloonTipText = "Program sağ alt köşede konumlandı.";
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;

                notifyIcon1.ShowBalloonTip(30000);
                notifyIcon1.ShowBalloonTip(100,"Bilgi","Yeni Bir mesajınız var",ToolTipIcon.Info);
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
                this.Show();
            }
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

        private void çıkışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        private void programıGösterToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
