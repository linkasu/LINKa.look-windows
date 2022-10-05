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

            using (var archive = ZipFile.Open(filePath, ZipArchiveMode.Create))
            {
               
                List<string> paths = new List<string>();
                foreach (var card in cardSetFile.Cards)
                {
                    string fix = card.ImagePath;
                    string name = Path.GetFileName(fix);
                    while(paths.Contains(name))
                    {
                        name = "_" + name;
                    }
                    paths.Add(name);
                    card.ImagePath = name;
                    if (card.ImagePath != null && card.ImagePath != string.Empty) archive.CreateEntryFromFile(fix, name);
                    if (card.AudioPath != null && card.AudioPath != string.Empty) archive.CreateEntryFromFile(card.AudioPath, Path.GetFileName(card.AudioPath));
                }

                string json = JsonConvert.SerializeObject(cardSetFile, Formatting.Indented, new CardJsonConverter(typeof(Card)));

                var configEntry = archive.CreateEntry("config.json");
                using (var writer = new StreamWriter(configEntry.Open()))
                {

                    writer.WriteLine(json);
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

        internal void SaveToDirectory(CardSetFile currentSeFile, string selectedPath)
        {
            for (int i = 0; i < currentSeFile.Cards.Count; i++)
            {
                string imagePath = currentSeFile.Cards[i].ImagePath;
                if (imagePath == null||imagePath.Length==0||!File.Exists(imagePath)) continue;
                File.Copy(imagePath, Path.Combine(selectedPath, "Картинка N" + i + Path.GetExtension(imagePath)), true);
            }
        }
    }
}
