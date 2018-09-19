using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reverb.Models;

namespace Reverberate.Models
{
    public class SavedTrack
    {
        public bool Saved { get; set; }

        public SpotifyTrack Track { get; set; }
    }
}
