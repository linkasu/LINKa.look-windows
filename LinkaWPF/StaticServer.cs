using DeviceId;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinkaWPF
{
    class StaticServer
    {
        private static string SERVER = "http://linka.su:5443/";
        private static string DISTFOLDER = "https://linka.su/dist/linka.looks/";
        private static StaticServer _instance;

        public static StaticServer instance
        {
            get
            {
                if (_instance == null)
                    _instance = new StaticServer();
                return _instance;
            }
        }


        public async Task ReportEvent(string eventType, Dictionary<string, string> parameters)
        {
            var uuid = new DeviceIdBuilder()
                .AddSystemUUID()
                .AddOSVersion()
                .AddUserName()
                .ToString();
            JObject obj = new JObject();
            obj.Add("event", eventType);
            obj.Add("uuid", uuid);
            if (parameters!=null) obj.Add("params", JObject.FromObject(parameters));

            var httpContent = new StringContent(obj.ToString());
            var mediaType = new MediaTypeWithQualityHeaderValue("application/json");
            httpContent.Headers.ContentType = mediaType;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(mediaType);
            var response = await client.PostAsync(SERVER+"report", httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            var jobject = JObject.Parse(responseBody);
                
        }

        internal Task ReportEvent(string eventType)
        {
            return ReportEvent(eventType, null);
        }

        public async Task CheckUpdateAsync()
        {

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetAsync(DISTFOLDER + "version.json");
            var responseBody = await response.Content.ReadAsStringAsync();
            var jobject = JObject.Parse(responseBody);
            var newVersion = new Version(jobject.Value<string>("version"));
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (newVersion.CompareTo(currentVersion) == 1)
            {


                const string message =
                    "Вышло новая версия программы, хотите загрузить?";
                const string caption = "Вышло обновление.";
                var result = MessageBox.Show(message, caption,
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

                // If the no button was pressed ...
                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(DISTFOLDER + "linka.looks.setup.exe");
                }

            }
        }
    }
}