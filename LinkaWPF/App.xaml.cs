using CommandLine;
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
    class Options
    {
        [Option('e', "editor", Required = false,
            HelpText = "If you need to open the editor, set this parameter.")]
        public bool IsEditor { get; set; }

        [Option('p', "path", Default = null,
            HelpText = "If you need to open cardset from file, set this parametr.")]
        public string Path { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Host _host;
        private WpfInteractorAgent _agent;
        private bool _isEditor = false;
        private string _path = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            var options = new Options();
            Parser.Default.ParseArguments<Options>(e.Args)
                .WithParsed<Options>(o =>
                {
                    if (o.IsEditor) _isEditor = o.IsEditor;
                    if (o.Path != null) _path = o.Path;
                });
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

            if (_isEditor == true)
            {
                var editorWindow = new EditorWindow(TempDirPath, YandexSpeech);
                editorWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                if (_path != null) editorWindow.LoadCardSet(_path);
                editorWindow.Show();
            }
            else
            {
                var mainWindow = new MainWindow(TempDirPath, YandexSpeech, _host);
                mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                if (_path != null) mainWindow.LoadCardSet(_path);
                mainWindow.Show();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // we will close the coonection to the Tobii Engine before exit.
            _host.DisableConnection();

            ClearCache();

            base.OnExit(e);
        }

        private void ClearCache()
        {
            var allfiles = Directory.GetFiles(TempDirPath);
            foreach (var filename in allfiles)
            {
                File.Delete(filename);
            }
        }

        public static string TempDirPath { get; set; }

        public static YandexSpeech YandexSpeech { get; set; }
    }
}
