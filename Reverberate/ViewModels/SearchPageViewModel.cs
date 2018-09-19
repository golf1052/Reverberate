using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Reverb.Models;
using Reverberate.Models;
using Reverberate.Views;

namespace Reverberate.ViewModels
{
    public class SearchPageViewModel : ViewModelBase
    {
        private readonly NavigationService navigationService;

        public ObservableCollection<SpotifyAlbum> Albums { get; set; }

        public ObservableCollection<SpotifyArtist> Artists { get; set; }

        public ObservableCollection<SavedTrack> Tracks { get; set; }

        private string searchQuery;
        public string SearchQuery
        {
            get { return searchQuery; }
            set { searchQuery = value; RaisePropertyChanged(nameof(SearchQuery)); }
        }

        public SearchPageViewModel(NavigationService navigationService)
        {
            this.navigationService = navigationService;
            Albums = new ObservableCollection<SpotifyAlbum>();
            Artists = new ObservableCollection<SpotifyArtist>();
            Tracks = new ObservableCollection<SavedTrack>();
        }

        public async Task OnNavigatedTo(SpotifySearch results)
        {
            Albums = new ObservableCollection<SpotifyAlbum>();
            Artists = new ObservableCollection<SpotifyArtist>();
            Tracks = new ObservableCollection<SavedTrack>();
            if (results.Albums != null)
            {
                Albums.AddRange(results.Albums.Items);
            }
            if (results.Artists != null)
            {
                Artists.AddRange(results.Artists.Items);
            }
            if (results.Tracks != null)
            {
                List<bool> savedTracks = await AppConstants.SpotifyClient.GetSavedTracks(results.Tracks.Items.Select(track => track.Id).ToList());
                for (int i = 0; i < savedTracks.Count; i++)
                {
                    Tracks.Add(new SavedTrack()
                    {
                        Track = results.Tracks.Items[i],
                        Saved = savedTracks[i]
                    });
                }
            }
        }

        public async Task TracksListView_ItemClick(SavedTrack track)
        {
            SpotifyAlbum fullAlbum = await AppConstants.SpotifyClient.GetAlbum(track.Track.Album.Id);
            navigationService.NavigateTo(nameof(AlbumDetailPage), fullAlbum);
        }

        public async Task AlbumsListView_ItemClick(SpotifyAlbum album)
        {
            SpotifyAlbum fullAlbum = await AppConstants.SpotifyClient.GetAlbum(album.Id);
            navigationService.NavigateTo(nameof(AlbumDetailPage), fullAlbum);
        }

        public void ArtistsListView_ItemClick(SpotifyArtist artist)
        {
            navigationService.NavigateTo(nameof(ArtistDetailPage), artist);
        }
    }
}
