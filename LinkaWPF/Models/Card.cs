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
        private string _imagePath;

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

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                if (value != null && value != string.Empty) Image = new BitmapImage(new Uri(value));
                _imagePath = value;
            }
        }

        public ImageSource Image { get; private set; }

        public string Title { get; set; }

        public string AudioPath { get; set; }

        public bool WithoutSpace { get; set; }
    }
}
