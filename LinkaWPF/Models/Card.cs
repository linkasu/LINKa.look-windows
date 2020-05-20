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
        public Card(int id, string title, string path, string audio = null)
        {
            this.Id = id;
            this.Title = title;

            if (path != null && path != string.Empty) this.Source = new BitmapImage(new Uri(path));
            //this.Source = new Uri(Environment.CurrentDirectory + "/images/" + path, UriKind.Absolute);

            Audio = audio;
            WithoutSpace = false;
        }
        public int Id { get; set; }
        public ImageSource Source { get; set; }
        public string Title { get; set; }
        public string Audio { get; set; }
        public bool WithoutSpace { get; set; }
    }
}
