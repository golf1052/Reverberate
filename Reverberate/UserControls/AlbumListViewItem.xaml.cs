using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Reverb.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Reverberate.UserControls
{
    public sealed partial class AlbumListViewItem : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Uri albumImageUrl;
        public Uri AlbumImageUrl
        {
            get { return albumImageUrl; }
            set { albumImageUrl = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlbumImageUrl))); }
        }

        private string albumTitle;
        public string AlbumTitle
        {
            get { return albumTitle; }
            set { albumTitle = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlbumTitle))); }
        }

        private string albumArtist;
        public string AlbumArtist
        {
            get { return albumArtist; }
            set { albumArtist = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlbumArtist))); }
        }

        public SpotifyAlbum Model
        {
            get { return (SpotifyAlbum)DataContext; }
        }

        public AlbumListViewItem()
        {
            this.InitializeComponent();
            DataContextChanged += AlbumItem_DataContextChanged;
        }

        private void AlbumItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (Model != null)
            {
                var highestResImage = Model.GetLargestImage();
                if (highestResImage == null)
                {
                    AlbumImageUrl = new Uri("ms-appx:///Assets/PlaceholderAlbum.png");
                }
                else
                {
                    AlbumImageUrl = new Uri(highestResImage.Url);
                }
                AlbumTitle = Model.Name;
                AlbumArtist = string.Join(", ", Model.Artists.Select(artist => artist.Name));
            }
        }
    }
}
