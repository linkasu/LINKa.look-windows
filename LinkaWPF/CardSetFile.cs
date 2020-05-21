using LinkaWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace LinkaWPF
{
    public class CardJsonConverter : JsonConverter
    {
        private readonly Type[] _types;

        public CardJsonConverter(params Type[] types)
        {
            _types = types;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);

            if (t.Type != JTokenType.Object)
            {
                t.WriteTo(writer);
            }
            else
            {
                JObject o = (JObject)t;

                var imagePathProperty = o.Property("ImagePath");
                imagePathProperty.Value = Path.GetFileName(imagePathProperty.Value.ToString());

                var audioPathProperty = o.Property("AudioPath");
                audioPathProperty.Value = Path.GetFileName(audioPathProperty.Value.ToString());

                o.WriteTo(writer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return _types.Any(t => t == objectType);
        }
    }

    public class CardSetFile
    {
        public CardSetFile(int columns, int rows, IList<Card> cards)
        {
            Version = "1.0";
            Columns = columns;
            Rows = rows;
            Cards = cards;
        }

        public void SaveToFile(string path)
        {
            File.Delete(path);
            string json = JsonConvert.SerializeObject(this, Formatting.Indented, new CardJsonConverter(typeof(Card)));

            using (var archive = ZipFile.Open(path, ZipArchiveMode.Create))
            {
                var configEntry = archive.CreateEntry("config.json");
                using (var writer = new StreamWriter(configEntry.Open()))
                {
                    writer.WriteLine(json);
                }

                foreach (var card in Cards)
                {
                    if (card.ImagePath != null) archive.CreateEntryFromFile(card.ImagePath, Path.GetFileName(card.ImagePath));
                    if (card.AudioPath != null) archive.CreateEntryFromFile(card.AudioPath, Path.GetFileName(card.AudioPath));
                }
            }
        }

        public void LoadFromFile(string path)
        {

        }

        public string Version { get; set; }

        public int Columns { get; set; }

        public int Rows { get; set; }

        public IList<Card> Cards { get; set; }
    }
}
