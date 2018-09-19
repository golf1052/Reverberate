using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Reverb;
using Reverb.Models;
using Reverberate.Models;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Reverberate.ViewModels
{
    public class AlbumDetailPageViewModel : ViewModelBase
    {
        private readonly NavigationService navigationService;

        private Uri albumImageUrl;
        public Uri AlbumImageUrl
        {
            get { return albumImageUrl; }
            set { albumImageUrl = value; RaisePropertyChanged(nameof(AlbumImageUrl)); }
        }

        private string albumName;
        public string AlbumName
        {
            get { return albumName; }
            set { albumName = value; RaisePropertyChanged(nameof(AlbumName)); }
        }

        private string albumArtist;
        public string AlbumArtist
        {
            get { return albumArtist; }
            set { albumArtist = value; RaisePropertyChanged(nameof(AlbumArtist)); }
        }

        private string releaseDate;
        public string ReleaseDate
        {
            get { return releaseDate; }
            set { releaseDate = value; RaisePropertyChanged(nameof(ReleaseDate)); }
        }

        private string numSongs;
        public string NumSongs
        {
            get { return numSongs; }
            set { numSongs = value; RaisePropertyChanged(nameof(NumSongs)); }
        }

        private string albumLength;
        public string AlbumLength
        {
            get { return albumLength; }
            set { albumLength = value; RaisePropertyChanged(nameof(AlbumLength)); }
        }

        private string savedSaveText;
        public string SavedSaveText
        {
            get { return savedSaveText; }
            set { savedSaveText = value; RaisePropertyChanged(nameof(SavedSaveText)); }
        }

        public ObservableCollection<SavedTrack> Tracks { get; set; }

        private SpotifyAlbum album;

        public AlbumDetailPageViewModel(NavigationService navigationService)
        {
            this.navigationService = navigationService;
            Tracks = new ObservableCollection<SavedTrack>();
        }

        public async Task OnNavigatedTo(SpotifyAlbum album)
        {
            Tracks = new ObservableCollection<SavedTrack>();
            this.album = album;
            AlbumImageUrl = new Uri(album.GetLargestImage().Url);
            AlbumName = album.Name;
            AlbumArtist = string.Join(", ", album.Artists.Select(artist => artist.Name));
            DateTimeOffset releaseDate = HelperMethods.ParseReleaseDate(album.ReleaseDate);
            if (album.ReleaseDatePrecision == "year")
            {
                ReleaseDate = releaseDate.Year.ToString();
            }
            else if (album.ReleaseDatePrecision == "month")
            {
                ReleaseDate = releaseDate.ToString("y");
            }
            else if (album.ReleaseDatePrecision == "day")
            {
                ReleaseDate = releaseDate.ToString("d");
            }
            if (album.Tracks.Total == 1)
            {
                NumSongs = $"{album.Tracks.Total} song";
            }
            else
            {
                NumSongs = $"{album.Tracks.Total} songs";
            }
            SavedSaveText = "Saved";
            TimeSpan albumLength = TimeSpan.Zero;
            SpotifyPagingObject<SpotifyTrack> tracksPaging = album.Tracks;
            List<bool> savedTracks = await AppConstants.SpotifyClient.GetSavedTracks(tracksPaging.Items.Select(track => track.Id).ToList());
            for (int i = 0; i < tracksPaging.Items.Count; i++)
            {
                SavedTrack savedTrack = new SavedTrack()
                {
                    Track = tracksPaging.Items[i],
                    Saved = savedTracks[i]
                };
                Tracks.Add(savedTrack);
                albumLength += TimeSpan.FromMilliseconds(savedTrack.Track.Duration);
            }
            while (tracksPaging.Next != null)
            {
                tracksPaging = await AppConstants.SpotifyClient.GetNextPage(tracksPaging);
                savedTracks = await AppConstants.SpotifyClient.GetSavedTracks(tracksPaging.Items.Select(track => track.Id).ToList());
                for (int i = 0; i < tracksPaging.Items.Count; i++)
                {
                    SavedTrack savedTrack = new SavedTrack()
                    {
                        Track = tracksPaging.Items[i],
                        Saved = savedTracks[i]
                    };
                    Tracks.Add(savedTrack);
                    albumLength += TimeSpan.FromMilliseconds(savedTrack.Track.Duration);
                }
            }
            AlbumLength = albumLength.MinimalToString();
        }

        public async Task PlayButton_Click()
        {
            try
            {
                await AppConstants.SpotifyClient.Play(MediaControlBarViewModel.ActiveDeviceId, album.Uri);
            }
            catch (SpotifyException)
            {
                await WebPlayerViewModel.ReconnectPlayer(MediaControlBarViewModel.ActiveDeviceId);
                return;
            }
            await Task.Delay(TimeSpan.FromMilliseconds(250));
            await HelperMethods.GetViewModelLocator().MediaControlBarInstance.SetupPlayback();
        }

        public async Task TracksListView_ItemClick(SavedTrack track)
        {
            List<SpotifyTrack> tracksToPlay = new List<SpotifyTrack>();
            int i = 0;
            for (; i < Tracks.Count; i++)
            {
                if (track == Tracks[i])
                {
                    break;
                }
            }

            try
            {
                await AppConstants.SpotifyClient.Play(MediaControlBarViewModel.ActiveDeviceId, album.Uri, offset: i);
            }
            catch (SpotifyException)
            {
                await WebPlayerViewModel.ReconnectPlayer(MediaControlBarViewModel.ActiveDeviceId);
                return;
            }
        }
    }
}
