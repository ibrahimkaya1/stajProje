using System;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace wwakalar
{
    public partial class Form1 : Form
    {
        
        //https://wakatime.com/api/v1/users/current/durations?date=2018-07-13&api_key=5fff4da8-ee27-43e6-9511-903fd88be248
        public Form1()
        {
            
            InitializeComponent();

        }

        
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
        private void Form1_Load(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Today;
            int yil = dt.Year;
            int ay = dt.Month;
            int gun = dt.Day;
            gun = gun - 1;
            /*formdan tarih aralığı alınacak ona göre ay yıl ataması yapılacak. Ama aralık girilmez ise görüntülenecek zaman  bugün veya dün olacak*/
            veri_duration = get_data("https://wakatime.com/api/v1/users/current/durations?date="+yil+"-"+ay+"-"+gun+"&api_key="+api_key);
            veri_heartbeats = get_data("https://wakatime.com/api/v1/users/current/heartbeats?date=" + yil + "-" + ay + "-" + gun + "&api_key=" + api_key);
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
        public string parse_json3(string json)
        {
            string value = string.Empty;

            JObject json_object = JObject.Parse(json);
            var json_result = json_object["data"];

         
                int i = 0;

                string cretad = json_result["created_at"].ToString();

                string mail = json_result["email"].ToString();
               
                string ids = json_result["id"].ToString();

                string last_heartbeat = json_result["last_heartbeat"].ToString();
                
                string last_plugin_name = json_result["last_plugin_name"].ToString();

                string last_project = json_result["last_project"].ToString();

                string photo= json_result["photo"].ToString();

                string timezone = json_result["timezone"].ToString();
                value = "created_at=" + cretad + "\n" + "email=" + mail + "\n" + "id=" + ids + "\n" + "last_heartbeat=" 
                    + last_heartbeat + "\n" + "last_plugin_name=" + last_plugin_name + "\n"+ "last_project="+ last_project+ "\n"+
                    "photo="+ photo+"\n"+ "timezone="+ timezone+"\n";


                /*****************************************************************************************/



                Console.WriteLine(value); //burda ki çıktılar veritabanına kaydedilecek


                /*****************************************************************************************/
                //dizi_al(duration, i, time, project);
            
            return value;
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
        public string parse_json2(string json)
        {
            string value = string.Empty;

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
                value = "created_at=" + cretad + "\n"+"proje="+proje+"\n" + "language="+ language+"\n" + "entity=" + entit + "\n" + "id="+ids+"\n"
                    + "user_agent_id="+ user_agent_id+"\n"+ "time=" + times + "\n" +"type="+ types + "\n"+"Kategori="+katagori+"\n";


                /*****************************************************************************************/



                Console.WriteLine(value); //burda ki çıktılar veritabanına kaydedilecek
              
                
                /*****************************************************************************************/
                                          //dizi_al(duration, i, time, project);
            }
            return value;
        }
        public string parse_json(string json)
        {
            string value = string.Empty;

            JObject json_object = JObject.Parse(json);
            var json_result = json_object["data"];

           
            foreach (var datas in json_result)
            {
                int i = 0;
                   
                string duration = datas["duration"].ToString();
              //  Console.WriteLine(duration);
                durat[i]=duration.ToString();
               
                string project = datas["project"].ToString();
                string time = datas["time"].ToString();
                var end = datas["end"];
                var start = datas["start"];
                var timezone = datas["timezone"];

                value = "duration=" + duration + "\n" + "project=" + project + "\n" + "time=" + time + "\n"+timezone+"\n";



                /*****************************************************************************************/


                Console.WriteLine(value);//burdaki veriler  ayrı tabloda tutulacak
                                      
                
                /*****************************************************************************************/




              //  dizi_al(duration,i,time,project);
            }
            return value;
        }
        /*public void dizi_al(string duration,int i, string time,string project)
        {
           
            durat[i] = duration;
            tm[i] = time;
            prject[i] = project;
           Console.WriteLine(i+") "+durat[i]+" "+tm[i]+" "+prject[i]);
        }*/
        private void button1_Click(object sender, EventArgs e)
        {
            parse_json2(veri_heartbeats);
            Console.WriteLine("*************************************************************************************");
            parse_json(veri_duration);
            Console.WriteLine("*************************************************************************************");
            parse_json3(veri_user);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
