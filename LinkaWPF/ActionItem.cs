using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LinkaWPF
{
    public class ActionItem
    {
        public ActionItem(string name, string title) : this(name, title, new List<string>())
        {
        }

        public ActionItem(string name, string title, IList<string> keys)
        {
            Name = name;
            Title = title;
            Keys = new ObservableCollection<string>(keys);
        }

        public string Name { get; set; }

        public string Title { get; set; }

        public ObservableCollection<string> Keys { get; set; }
    }
}
