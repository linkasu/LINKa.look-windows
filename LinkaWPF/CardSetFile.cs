using LinkaWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace LinkaWPF
{
    public class CardSetFile
    {
        public CardSetFile(int columns, int rows, IList<Card> cards)
        {
            Version = "1.0";
            Columns = columns;
            Rows = rows;
            Cards = cards;
        }

        public string Version { get; set; }

        public int Columns { get; set; }

        public int Rows { get; set; }

        public IList<Card> Cards { get; set; }
    }
}
