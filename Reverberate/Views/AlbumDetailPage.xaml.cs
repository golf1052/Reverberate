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
    public sealed partial class AlbumDetailPage : Page
    {
        public AlbumDetailPageViewModel Vm
        {
            get
            {
                return (AlbumDetailPageViewModel)DataContext;
            }
        }

        public AlbumDetailPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await Vm.OnNavigatedTo((SpotifyAlbum)e.Parameter);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Vm.OnNavigatingFrom();
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            await Vm.PlayButton_Click();
        }

        private async void TracksListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await Vm.TracksListView_ItemClick((SpotifyTrack)e.ClickedItem);
        }
    }
}
