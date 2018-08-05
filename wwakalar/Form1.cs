using System;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Data.SqlClient;
using System.Data;

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

        

        string[] durat = new string[100];
        string[] tm = new string[100];
        string[] prject = new string[100];
        string[] arrray_value = new string[100];
        string api_key = "5fff4da8-ee27-43e6-9511-903fd88be248";
        //string api_key = "084fb563-1f53-44d0-9ff2-48f2c51d202c";

     //   string tarih = DateTime.Now.ToString();
        string[] created = new string[100];
        string[] entity = new string[100];
        string[] time = new string[100];
        string[] type = new string[100];

        string veri_duration;
        string veri_heartbeats;
        string veri_user;
       
        public void Form1_Load(object sender, EventArgs e)
        {
            string tarih=Tarih_Cagir();
            /*formdan tarih aralığı alınacak ona göre ay yıl ataması yapılacak. Ama aralık girilmez ise görüntülenecek zaman  bugün veya dün olacak*/
            veri_duration = get_data("https://wakatime.com/api/v1/users/current/durations?date="+tarih+"&api_key="+api_key);
            veri_heartbeats = get_data("https://wakatime.com/api/v1/users/current/heartbeats?date=" + tarih+ "&api_key=" + api_key);
            veri_user = get_data("https://wakatime.com/api/v1/users/current?api_key=" + api_key);
        }   
        /*{
  "data": {
    "created_at": "2018-06-25T05:48:32Z",+
    "display_name": "Anonymous User",
    "email": "ibrahimkaya334298@gmail.com",+
    "email_public": false,
    "full_name": null,
    "has_premium_features": false,
    "human_readable_website": null,
    "id": "52cb9b43-fa90-4617-9c55-6e300ffb9c8f",+
    "is_email_confirmed": true,
    "is_hireable": false,
    "languages_used_public": false,
    "last_heartbeat": "2018-07-25T08:40:07Z",+
    "last_plugin": "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.0.1594 Safari/537.36 chrome-wakatime/1.0.2",
    "last_plugin_name": "Chrome",+
    "last_project": "wwakalar",+
    "location": null,
    "logged_time_public": false,
    "modified_at": null,
    "photo": "https://wakatime.com/gravatar/52cb9b43-fa90-4617-9c55-6e300ffb9c8f",
    "photo_public": true,
    "plan": "basic",
    "timezone": "Europe/Istanbul",
    "username": null,
    "website": null
  }
}*/
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
        /*
      data": [
    {
      "branch": null,+++++++++++++++++++
      "category": "browsing",++++++++++++++++++
      "created_at": "2018-07-23T01:38:45Z",++++++++++
      "cursorpos": null,----------------------
      "dependencies": [],--------------------
      "entity": "https://www.youtube.com",++++++++++++++++++
      "id": "3cd73229-b63d-4c63-86aa-be56a1baca34", ++++++++++++
      "is_debugging": false,------------
      "is_write": false,----------
      "language": null,-----------
      "lineno": null,-----------
      "lines": null,-------
      "machine_name_id": null,-------------
      "project": "wwakalar",        ++++++++++++++
      "time": 1532309955,   ++++++++++
      "type": "domain",     +++++++++++++++
      "user_agent_id": "76973644-4f88-4873-bc31-9fd33f4be7a4",  +++++++
      "user_id": "52cb9b43-fa90-4617-9c55-6e300ffb9c8f"     +++++
    },
             
             */
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
            parse_json(veri_duration);  
            
          //  Console.WriteLine("*************************************************************************************");
            parse_json2(veri_heartbeats);
          //  Console.WriteLine("*************************************************************************************");
          parse_json3(veri_user);
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
    }
}
