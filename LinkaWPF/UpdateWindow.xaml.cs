using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;

namespace LinkaWPF
{
    /// <summary>
    /// Логика взаимодействия для UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        string downloadFileUrl = StaticServer.DISTFOLDER + "linka.looks.setup.exe";
        string destinationFilePath = Path.GetTempPath()+ "\\linka.exe";

        public UpdateWindow()
        {
            InitializeComponent();


            startDownload();
        }
        private void startDownload()
        {

            Thread thread = new Thread(() => {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += client_DownloadFileCompleted;

                client.DownloadFileAsync(new Uri(downloadFileUrl), destinationFilePath);
            });
            thread.Start();
        }


        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.
            BeginInvoke((MethodInvoker)delegate {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                label.Text = "Загружено " + e.BytesReceived / (1024*1024)+ "мб из " +( e.TotalBytesToReceive /(1024*1024))+" мб. ("+Math.Round( percentage)+"%).";
                progressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
            });
        }
        void client_DownloadFileCompleted(object o, object a)
                {

            System.Diagnostics.Process.Start(new ProcessStartInfo(
                destinationFilePath
                )
            {
                Arguments = "/silent /restartapplications"
            });
            Environment.Exit(0);
        }
    }
}
