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
        public Card(int id, string title, string imagePath, string audioPath = null) : this(id, title, imagePath, audioPath, false)
        {
            
        }

        public Card(int id, string title, string imagePath, string audioPath, bool withoutSpace)
        {
            Id = id;
            Title = title;
            ImagePath = imagePath;
            AudioPath = audioPath;
            WithoutSpace = withoutSpace;
        }

        public int Id { get; set; }

        public string ImagePath { get; set; }

        public string Title { get; set; }

        public string AudioPath { get; set; }

        public bool WithoutSpace { get; set; }
    }
}
