using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LinkaWPF.Models
{
    public class Card
    {
        public Card()
        {

        }

        public Card(int id, string title, string imagePath, string audioPath = null)
        {
            Id = id;
            Title = title;
            ImagePath = imagePath;
            AudioPath = audioPath;
        }

        public int Id { get; set; }

        public string ImagePath { get; set; }

        public string Title { get; set; }

        public string AudioPath { get; set; }
    }
}
