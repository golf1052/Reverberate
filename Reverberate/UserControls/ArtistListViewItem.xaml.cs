using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ArtistListViewItem : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Uri artistImageUrl;
        public Uri ArtistImageUrl
        {
            get { return artistImageUrl; }
            set { artistImageUrl = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ArtistImageUrl))); }
        }

        private string artistName;
        public string ArtistName
        {
            get { return artistName; }
            set { artistName = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ArtistName))); }
        }

        public SpotifyArtist Model
        {
            get { return (SpotifyArtist)DataContext; }
        }

        public ArtistListViewItem()
        {
            this.InitializeComponent();
            DataContextChanged += ArtistListViewItem_DataContextChanged;
        }

        private void ArtistListViewItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (Model != null)
            {
                var highestResImage = Model.GetLargestImage();
                if (highestResImage == null)
                {
                    ArtistImageUrl = new Uri("ms-appx:///Assets/PlaceholderArtist.png");
                }
                else
                {
                    ArtistImageUrl = new Uri(highestResImage.Url);
                }
                
                ArtistName = Model.Name;
            }
        }
    }
}
