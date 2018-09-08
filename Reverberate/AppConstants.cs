using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reverb;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Reverberate
{
    public static class AppConstants
    {
        private static SpotifyClient spotifyClient;
        public static SpotifyClient SpotifyClient
        {
            get
            {
                return spotifyClient;
            }
        }

        public static Color SpotifyGreen
        {
            get { return Color.FromArgb(255, 30, 215, 96); }
        }

        public static SolidColorBrush SpotifyGreenBrush
        {
            get { return new SolidColorBrush(SpotifyGreen); }
        }

        static AppConstants()
        {
            spotifyClient = new SpotifyClient(Secrets.ClientId, Secrets.ClientSecret, $"{Secrets.RedirectUrl}/");
        }
    }
}
