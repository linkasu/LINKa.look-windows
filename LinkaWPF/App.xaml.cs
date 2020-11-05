using CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
        private Settings _settings;
        private string _tempDirPath;
        private YandexSpeech _yandexSpeech;

        protected override void OnStartup(StartupEventArgs e)
        {

            StaticServer.instance.CheckUpdateAsync();

            var options = new Options();
            Parser.Default.ParseArguments<Options>(e.Args)
                .WithParsed<Options>(o =>
                {
                    if (o.IsEditor) _isEditor = o.IsEditor;
                    if (o.Path != null) _path = o.Path;
                });


            // Создать директорию для временных файлов
            _tempDirPath = Path.GetTempPath()+ "\\linka.looks\\temp\\";
            Directory.CreateDirectory(_tempDirPath);


            // Everything starts with initializing Host, which manages connection to the 
            // Tobii Engine and provides all Tobii Core SDK functionality.
            // NOTE: Make sure that Tobii.EyeX.exe is running
            _host = new Host();

            // We need to instantiate InteractorAgent so it could control lifetime of the interactors.
            _agent = _host.InitializeWpfAgent();

            // Загрузка настроек из файла
            var configFile = Environment.GetFolderPath( Environment.SpecialFolder.Personal) + "\\linka.looks.config.json";
            var settingsLoader = new SettingsLoader();
            if (File.Exists(configFile) == true)
            {
                _settings = settingsLoader.LoadFromFile(configFile);
            }
            else
            {
                // Файла не существует, установим настройки по умолчанию
                _settings = new Settings();

                _settings.ConfigFilePath = configFile;

                _settings.Keys = new Dictionary<string, string>();
                _settings.Keys.Add("Left", "MoveSelectorLeft");
                _settings.Keys.Add("Right", "MoveSelectorRight");
                _settings.Keys.Add("Up", "MoveSelectorUp");
                _settings.Keys.Add("Down", "MoveSelectorDown");
                _settings.Keys.Add("Return", "Enter");
                _settings.Keys.Add("Space", "Enter");

                _settings.IsHasGazeEnabled = true;
                _settings.IsAnimatedClickEnabled = true;
                _settings.ClickDelay = 1;
                _settings.IsPlayAudioFromCard = false;
                _settings.IsPageButtonVisible = true;
                _settings.IsJoystickEnabled = true;
                _settings.IsKeyboardEnabled = true;
                _settings.IsMouseEnabled = true;

                settingsLoader.SaveToFile(configFile, _settings);
            }
            _yandexSpeech = new YandexSpeech(_tempDirPath, _settings);

            _settings.SettingsLoader = settingsLoader;
            _settings.TempDirPath = _tempDirPath;
            _settings.YandexSpeech = _yandexSpeech;
            
            _settings.Host = _host;

            if (_isEditor == true)
            {
                ShowEditorWindow(_path);
            }
            else
            {
                ShowMainWindow(_path);
            }

            // TODO: Заменить на загрузку из конфига
        }

        private void ShowEditorWindow(string path)
        {
            // Создаем окно редактора
            var editorWindow = new EditorWindow(_settings, _tempDirPath, _yandexSpeech);
            editorWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            // Функция смены режима работы программы
            editorWindow.ChangeMode = () =>
            {
                if (editorWindow.IsSave() == false) return false;

                // Открываем окно пользователя
                ShowMainWindow(editorWindow.CurrentFileName);

                // Закрываем окно редактора
                editorWindow.Close();

                return true;
            };

            if (path != null && path != string.Empty) editorWindow.LoadCardSet(path);

            editorWindow.Show();
        }

        private void ShowMainWindow(string path)
        {
            var mainWindow = new MainWindow(_settings);
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.ChangeMode = (str) =>
            {
                // Открываем окно редактора
                ShowEditorWindow(str);

                // Закрываем окно пользовательского режима
                mainWindow.Close();

                return true;
            };

            if (path != null && path != string.Empty) mainWindow.LoadCardSet(path);

            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // we will close the coonection to the Tobii Engine before exit.
            _host.DisableConnection();

            base.OnExit(e);

            ClearCache();
        }

        private void ClearCache()
        {
            var directories = Directory.GetDirectories(_tempDirPath);
            foreach (var directory in directories)
            {
                try
                {
                    Directory.Delete(directory, true);
                }
                catch { }
            }

            var files = Directory.GetFiles(_tempDirPath);
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }
    }
}
