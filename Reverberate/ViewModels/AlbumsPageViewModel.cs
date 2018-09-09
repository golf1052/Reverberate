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
using Windows.UI.Xaml.Data;

namespace Reverberate.ViewModels
{
    public class AlbumsPageViewModel : ViewModelBase
    {
        private readonly NavigationService navigationService;

        public ObservableCollection<IGrouping<string, SpotifyAlbum>> AlbumGrouping { get; set; }

        public AlbumsPageViewModel(NavigationService navigationService)
        {
            this.navigationService = navigationService;
            AlbumGrouping = new ObservableCollection<IGrouping<string, SpotifyAlbum>>();
        }

        public async Task OnNavigatedTo()
        {
            List<SpotifyAlbum> albums = new List<SpotifyAlbum>();
            var albumsList = await AppConstants.SpotifyClient.GetUserSavedAlbums();
            foreach (var album in albumsList.Items)
            {
                albums.Add(album.Album);
            }
            while (albumsList.Next != null)
            {
                albumsList = await AppConstants.SpotifyClient.GetNextPage(albumsList);
                foreach (var album in albumsList.Items)
                {
                    albums.Add(album.Album);
                }
            }

            albums.Sort((a1, a2) => a1.Name.CompareTo(a2.Name));
            var group = albums.GroupBy(album =>
            {
                char firstChar = album.Name[0];
                if (char.IsDigit(firstChar))
                {
                    return "#";
                }
                else if (char.IsLetter(firstChar))
                {
                    return char.ToUpper(firstChar).ToString();
                }
                else
                {
                    return "...";
                }
            });
            AlbumGrouping.AddRange(group);
        }

        public void AlbumsListView_ItemClick(SpotifyAlbum album)
        {
            navigationService.NavigateTo(nameof(AlbumDetailPage), album);
        }
    }
}
