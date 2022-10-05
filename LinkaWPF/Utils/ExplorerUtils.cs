using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkaWPF.Utils
{
    class ExplorerUtils
    {
        public static void ExploreDirectory(string directory) {
            string path = directory;
            System.Diagnostics.Process.Start(path);
        }
    }
}
