using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicApp
{
    public class Playlist
    {
        public string Name { get; set; }
        
        public List<MusicTrack> Tracks { get; set; }

        public string Created { get; set; }

    }
}
