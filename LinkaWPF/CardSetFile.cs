﻿using LinkaWPF.Models;
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
        public CardSetFile(int columns, int rows, bool withoutSpace, IList<Card> cards, String description, bool directSet)
        {
            Version = "1.3";
            Columns = columns;
            Rows = rows;
            WithoutSpace = withoutSpace;
            Cards = cards;
            Description = description;
            DirectSet = directSet;
        }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("directSet")]
        public bool DirectSet { get; set; }
        [JsonProperty("columns")]
        public int Columns { get; set; }

        [JsonProperty("rows")]
        public int Rows { get; set; }

        [JsonProperty("withoutSpace")]
        public bool WithoutSpace { get; set; }

        [JsonProperty("cards")]
        public IList<Card> Cards { get; set; }

    }
}
        