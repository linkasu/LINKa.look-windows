using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tobii.Interaction;
using Tobii.Interaction.Wpf;

namespace LinkaWPF
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Host _host;
        private WpfInteractorAgent _agent;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Создать директорию для временных файлов
            TempDirPath = Environment.CurrentDirectory + "\\temp\\";
            Directory.CreateDirectory(TempDirPath);

            // TODO: Заменить на загрузку из конфига
            YandexSpeech = new YandexSpeech("4e68a4e5-b590-448d-9a66-f3d8f2854348", TempDirPath);

            // Everything starts with initializing Host, which manages connection to the 
            // Tobii Engine and provides all Tobii Core SDK functionality.
            // NOTE: Make sure that Tobii.EyeX.exe is running
            _host = new Host();

            // We need to instantiate InteractorAgent so it could control lifetime of the interactors.
            _agent = _host.InitializeWpfAgent();

            var editorWindow = new EditorWindow(TempDirPath, YandexSpeech);
            editorWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            editorWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // we will close the coonection to the Tobii Engine before exit.
            _host.DisableConnection();

            base.OnExit(e);
        }

        public static string TempDirPath { get; set; }

        public static YandexSpeech YandexSpeech { get; set; }
    }
}
