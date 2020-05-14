using LinkaWPF.Interfaces;
using Microsoft.DirectX.AudioVideoPlayback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkaWPF
{
    public class DirectxPlayer : IPlayer
    {
        private readonly Audio _audio;

        public DirectxPlayer(string path)
        {
            _audio = new Audio(path);
        }

        public event EventHandler Ending;

        public void Play()
        {
            _audio.Ending += Ending;
            _audio.Play();
        }
    }
}
