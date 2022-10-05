using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LinkaWPF.Models
{
    public enum CardType
    {
        Common,
        Space,
        Fake
    }

    public class Card
    {
        public Card()
        {

        }

        public Card(int id, int width, int height, string title, string imagePath, string audioPath = null) :this(id, width, height, title, imagePath, audioPath, CardType.Common)
        {
        }

        public Card(int id, int width, int height, string title, string imagePath, string audioPath, CardType cardType)
        {
            Id = id;
            Title = title;
            ImagePath = imagePath;
            AudioPath = audioPath;
            CardType = cardType;
            Width = width;
            Height = height;
        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [DefaultValue(1)]
        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Width { get; set; }

        [DefaultValue(1)]
        [JsonProperty("height", DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Height { get; set; }

        [JsonProperty("imagePath")]
        public string ImagePath { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("audioPath")]
        public string AudioPath { get; set; }

        [JsonProperty("cardType")]
        public CardType CardType { get; set; }
    }
}
