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
    public class NewReleasesPageViewModel : ViewModelBase
    {
        public ObservableCollection<SpotifyAlbum> All { get; set; }
        public ObservableCollection<SpotifyAlbum> ForYou { get; set; }

        private readonly NavigationService navigationService;

        public NewReleasesPageViewModel(NavigationService navigationService)
        {
            this.navigationService = navigationService;
            All = new ObservableCollection<SpotifyAlbum>();
            ForYou = new ObservableCollection<SpotifyAlbum>();
        }

        public async Task OnNavigatedTo()
        {
            All = new ObservableCollection<SpotifyAlbum>();
            ForYou = new ObservableCollection<SpotifyAlbum>();

            SpotifyPagingObject<SpotifyAlbum> all = await AppConstants.SpotifyClient.GetNewReleases(HelperMethods.GetUsersCountry(), 50);
            All.AddRange(all.Items);

            List<SpotifyArtist> userArtists = new List<SpotifyArtist>();
            var userAlbums = await AppConstants.SpotifyClient.GetUserSavedAlbums(50);
            foreach (var album in userAlbums.Items)
            {
                foreach (var artist in album.Album.Artists)
                {
                    if (!userArtists.Any(a => { return a.Id == artist.Id; }))
                    {
                        userArtists.Add(artist);
                    }
                }
            }
            while (userAlbums.Next != null)
            {
                userAlbums = await AppConstants.SpotifyClient.GetNextPage(userAlbums);
                foreach (var album in userAlbums.Items)
                {
                    foreach (var artist in album.Album.Artists)
                    {
                        if (!userArtists.Any(a => { return a.Id == artist.Id; }))
                        {
                            userArtists.Add(artist);
                        }
                    }
                }
            }

            List<SpotifyAlbum> forYou = new List<SpotifyAlbum>();
            DateTimeOffset currentDate = DateTimeOffset.Now;
            foreach (var artist in userArtists)
            {
                var albums = await AppConstants.SpotifyClient.GetArtistsAlbums(artist.Id, new List<Reverb.SpotifyConstants.SpotifyArtistIncludeGroups>()
                {
                    Reverb.SpotifyConstants.SpotifyArtistIncludeGroups.Album,
                    Reverb.SpotifyConstants.SpotifyArtistIncludeGroups.Single
                });

                foreach (var album in albums.Items)
                {
                    if (HelperMethods.ParseReleaseDate(album.ReleaseDate) >= currentDate.Subtract(TimeSpan.FromDays(14)))
                    {
                        if (!forYou.Any(a => { return a.Id == album.Id; }))
                        {
                            forYou.Add(album);
                        }
                    }
                }
            }

            forYou.Sort((a1, a2) =>
            {
                DateTimeOffset a1ReleaseDate = HelperMethods.ParseReleaseDate(a1.ReleaseDate);
                DateTimeOffset a2ReleaseDate = HelperMethods.ParseReleaseDate(a2.ReleaseDate);
                return a2ReleaseDate.CompareTo(a1ReleaseDate);
            });
            ForYou.AddRange(forYou);
        }

        public async Task AllListView_ItemClick(SpotifyAlbum album)
        {
            SpotifyAlbum fullAlbum = await AppConstants.SpotifyClient.GetAlbum(album.Id);
            navigationService.NavigateTo(nameof(AlbumDetailPage), fullAlbum);
        }

        public async Task ForYouListView_ItemClick(SpotifyAlbum album)
        {
            SpotifyAlbum fullAlbum = await AppConstants.SpotifyClient.GetAlbum(album.Id);
            navigationService.NavigateTo(nameof(AlbumDetailPage), fullAlbum);
        }
    }
}
