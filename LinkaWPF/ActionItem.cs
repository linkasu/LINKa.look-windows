using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LinkaWPF
{
    public class ActionItem
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public IList<string> Keys { get; set; }

        public string KeysString
        {
            get
            {
                var result = "";
                foreach (var key in Keys)
                {
                    result += (result == string.Empty ? "" : " ") + key + ";";
                }
                return result;
            }
        }
    }
}
