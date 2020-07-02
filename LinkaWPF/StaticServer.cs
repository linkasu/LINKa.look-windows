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
        private static string SERVER = "http://linka.su:5511";
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