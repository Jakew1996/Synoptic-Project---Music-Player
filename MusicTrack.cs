using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicApp
{
    public class MusicTrack
    {
        public string Name { get; set; }
        public Array Artists { get; set; } //TODO: Make sure it shows multiple artists
        public string Album { get; set; }
        public string FileLocation { get; set; }

        public void PlayTrack()
        {

        }

    }
}
