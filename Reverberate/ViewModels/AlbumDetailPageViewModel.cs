using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Reverb.Models;
using Windows.UI.Xaml.Controls;

namespace Reverberate.ViewModels
{
    public class AlbumDetailPageViewModel : ViewModelBase
    {
        public ObservableCollection<SpotifyTrack> Tracks { get; set; }

        public AlbumDetailPageViewModel()
        {
            Tracks = new ObservableCollection<SpotifyTrack>();
        }

        public async Task OnNavigatedTo()
        {

        }
    }
}
