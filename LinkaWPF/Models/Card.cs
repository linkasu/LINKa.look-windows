using System;
using System.Collections.Generic;
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

        public Card(int id, string title, string imagePath, string audioPath = null) :this(id, title, imagePath, audioPath, CardType.Common)
        {
        }

        public Card(int id, string title, string imagePath, string audioPath, CardType cardType)
        {
            Id = id;
            Title = title;
            ImagePath = imagePath;
            AudioPath = audioPath;
            CardType = cardType;
        }

        public int Id { get; set; }

        public string ImagePath { get; set; }

        public string Title { get; set; }

        public string AudioPath { get; set; }
        
        public CardType CardType { get; set; }
    }
}
