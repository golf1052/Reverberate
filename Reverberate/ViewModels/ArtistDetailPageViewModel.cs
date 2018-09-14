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
using Reverberate.Views;

namespace Reverberate.ViewModels
{
    public class ArtistDetailPageViewModel : ViewModelBase
    {
        private readonly NavigationService navigationService;

        private Uri artistImageUrl;
        public Uri ArtistImageUrl
        {
            get { return artistImageUrl; }
            set { artistImageUrl = value; RaisePropertyChanged(nameof(ArtistImageUrl)); }
        }

        private string artistName;
        public string ArtistName
        {
            get { return artistName; }
            set { artistName = value; RaisePropertyChanged(nameof(ArtistName)); }
        }

        public ObservableCollection<SpotifyTrack> PopularTracks { get; set; }

        public ObservableCollection<SpotifyAlbum> Albums { get; set; }

        public ObservableCollection<SpotifyAlbum> Singles { get; set; }

        public ObservableCollection<SpotifyArtist> RelatedArtists { get; set; }

        private SpotifyArtist artist;

        public ArtistDetailPageViewModel(NavigationService navigationService)
        {
            this.navigationService = navigationService;
            PopularTracks = new ObservableCollection<SpotifyTrack>();
            Albums = new ObservableCollection<SpotifyAlbum>();
            Singles = new ObservableCollection<SpotifyAlbum>();
            RelatedArtists = new ObservableCollection<SpotifyArtist>();
        }

        public async Task OnNavigatedTo(SpotifyArtist artist)
        {
            PopularTracks = new ObservableCollection<SpotifyTrack>();
            Albums = new ObservableCollection<SpotifyAlbum>();
            Singles = new ObservableCollection<SpotifyAlbum>();
            RelatedArtists = new ObservableCollection<SpotifyArtist>();
            this.artist = artist;
            var highestResImage = artist.GetLargestImage();
            if (highestResImage == null)
            {
                ArtistImageUrl = new Uri("ms-appx:///Assets/PlaceholderArtist.png");
            }
            else
            {
                ArtistImageUrl = new Uri(highestResImage.Url);
            }
            ArtistName = artist.Name;
            PopularTracks.AddRange(await AppConstants.SpotifyClient.GetArtistsTopTracks(artist.Id, "us"));
            SpotifyPagingObject<SpotifyAlbum> albums = await AppConstants.SpotifyClient.GetArtistsAlbums(artist.Id, new List<Reverb.SpotifyConstants.SpotifyArtistIncludeGroups>()
            {
                Reverb.SpotifyConstants.SpotifyArtistIncludeGroups.Album
            });
            Albums.AddRange(albums.Items);
            SpotifyPagingObject<SpotifyAlbum> singles = await AppConstants.SpotifyClient.GetArtistsAlbums(artist.Id, new List<Reverb.SpotifyConstants.SpotifyArtistIncludeGroups>()
            {
                Reverb.SpotifyConstants.SpotifyArtistIncludeGroups.Single
            });
            Singles.AddRange(singles.Items);
            RelatedArtists.AddRange(await AppConstants.SpotifyClient.GetArtistsRelatedArtists(artist.Id));
        }

        public async Task PlayButton_Click()
        {
            try
            {
                await AppConstants.SpotifyClient.Play(MediaControlBarViewModel.ActiveDeviceId, artist.Uri);
            }
            catch (SpotifyException)
            {
                await WebPlayerViewModel.ReconnectPlayer(MediaControlBarViewModel.ActiveDeviceId);
                return;
            }
            await Task.Delay(TimeSpan.FromMilliseconds(250));
            await HelperMethods.GetViewModelLocator().MediaControlBarInstance.SetupPlayback();
        }

        public async Task AlbumsListView_ItemClick(SpotifyAlbum album)
        {
            SpotifyAlbum fullAlbum = await AppConstants.SpotifyClient.GetAlbum(album.Id);
            navigationService.NavigateTo(nameof(AlbumDetailPage), fullAlbum);
        }

        public async Task SinglesListView_ItemClick(SpotifyAlbum single)
        {
            SpotifyAlbum fullSingle = await AppConstants.SpotifyClient.GetAlbum(single.Id);
            navigationService.NavigateTo(nameof(AlbumDetailPage), fullSingle);
        }

        public void RelatedArtistsListView_ItemClick(SpotifyArtist artist)
        {
            navigationService.NavigateTo(nameof(ArtistDetailPage), artist);
        }
    }
}
