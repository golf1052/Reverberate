using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Reverb.Models;
using Reverberate.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Reverberate.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        public SearchPageViewModel Vm
        {
            get { return (SearchPageViewModel)DataContext; }
        }

        public SearchPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SpotifySearch results = (SpotifySearch)e.Parameter;
            SearchPivot.Items.Remove(SearchPivot.Items.Single(p => (string)((PivotItem)p).Header == "Playlists"));
            HelperMethods.EnableBackButton();
            Vm.OnNavigatedTo(results);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            HelperMethods.DisableBackButton();
        }

        private async void TracksListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await Vm.TracksListView_ItemClick((SpotifyTrack)e.ClickedItem);
        }

        private async void AlbumsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await Vm.AlbumsListView_ItemClick((SpotifyAlbum)e.ClickedItem);
        }

        private void ArtistsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Vm.ArtistsListView_ItemClick((SpotifyArtist)e.ClickedItem);
        }
    }
}
