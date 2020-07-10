using LinkaWPF.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkaWPF
{
    public class CardSetLoader
    {
        public void SaveToFile(string filePath, CardSetFile cardSetFile)
        {
            File.Delete(filePath);
            string json = JsonConvert.SerializeObject(cardSetFile, Formatting.Indented, new CardJsonConverter(typeof(Card)));

            using (var archive = ZipFile.Open(filePath, ZipArchiveMode.Create))
            {
                var configEntry = archive.CreateEntry("config.json");
                using (var writer = new StreamWriter(configEntry.Open()))
                {
                    writer.WriteLine(json);
                }

                foreach (var card in cardSetFile.Cards)
                {
                    if (card.ImagePath != null && card.ImagePath != string.Empty) archive.CreateEntryFromFile(card.ImagePath, Path.GetFileName(card.ImagePath));
                    if (card.AudioPath != null && card.AudioPath != string.Empty) archive.CreateEntryFromFile(card.AudioPath, Path.GetFileName(card.AudioPath));
                }
            }
        }

        public CardSetFile LoadFromFile(string filePath, string tempPath)
        {
            ZipFile.ExtractToDirectory(filePath, tempPath);

            using (var file = File.OpenText(tempPath + "\\config.json"))
            {
                var serializer = new JsonSerializer();
                return (CardSetFile)serializer.Deserialize(file, typeof(CardSetFile));
            }
        }
    }
}
