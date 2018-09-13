using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Reverb.Models;
using Reverberate.Views;

namespace Reverberate.ViewModels
{
    public class SearchPageViewModel : ViewModelBase
    {
        private readonly NavigationService navigationService;

        public ObservableCollection<SpotifyAlbum> Albums { get; set; }

        public ObservableCollection<SpotifyArtist> Artists { get; set; }

        public ObservableCollection<SpotifyTrack> Tracks { get; set; }

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
            Tracks = new ObservableCollection<SpotifyTrack>();
        }

        public void OnNavigatedTo(SpotifySearch results)
        {
            Albums = new ObservableCollection<SpotifyAlbum>();
            Artists = new ObservableCollection<SpotifyArtist>();
            Tracks = new ObservableCollection<SpotifyTrack>();
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
                Tracks.AddRange(results.Tracks.Items);
            }
        }

        public async Task TracksListView_ItemClick(SpotifyTrack track)
        {
            SpotifyAlbum fullAlbum = await AppConstants.SpotifyClient.GetAlbum(track.Album.Id);
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
