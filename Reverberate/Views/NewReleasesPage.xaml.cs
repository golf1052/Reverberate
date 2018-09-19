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
    public sealed partial class NewReleasesPage : Page
    {
        public NewReleasesPageViewModel Vm
        {
            get { return (NewReleasesPageViewModel)DataContext; }
        }

        public NewReleasesPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            HelperMethods.EnableBackButton();
            await Vm.OnNavigatedTo();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            HelperMethods.DisableBackButton();
        }

        private async void AllListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await Vm.AllListView_ItemClick((SpotifyAlbum)e.ClickedItem);
        }

        private async void ForYouListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await Vm.ForYouListView_ItemClick((SpotifyAlbum)e.ClickedItem);
        }
    }
}
