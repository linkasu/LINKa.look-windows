using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkaWPF.Interfaces
{
    public interface IPlayer
    {
        void Play(string path);
        event EventHandler Ending;
    }
}
