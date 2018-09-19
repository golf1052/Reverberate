using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Reverb.Models;
using Reverberate.Models;
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
    public sealed partial class TrackItem : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int trackNumber;
        public int TrackNumber
        {
            get { return trackNumber; }
            set { trackNumber = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TrackNumber))); }
        }

        private Symbol addSaveIcon;
        public Symbol AddSaveIcon
        {
            get { return addSaveIcon; }
            set { addSaveIcon = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddSaveIcon))); }
        }

        private string trackTitle;
        public string TrackTitle
        {
            get { return trackTitle; }
            set { trackTitle = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TrackTitle))); }
        }

        private bool isExplicit;
        public bool IsExplicit
        {
            get { return isExplicit; }
            set { isExplicit = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExplicit))); }
        }

        private string trackLength;
        public string TrackLength
        {
            get { return trackLength; }
            set { trackLength = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TrackLength))); }
        }

        public SavedTrack Model
        {
            get { return (SavedTrack)DataContext; }
        }

        public TrackItem()
        {
            this.InitializeComponent();
            DataContextChanged += TrackItem_DataContextChanged;
        }

        private void TrackItem_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (Model != null)
            {
                TrackNumber = Model.Track.TrackNumber;
                TrackTitle = Model.Track.Name;
                IsExplicit = Model.Track.Explicit;
                TimeSpan trackLength = TimeSpan.FromMilliseconds(Model.Track.Duration);
                TrackLength = trackLength.MinimalToString();
                if (Model.Saved)
                {
                    AddSaveIcon = Symbol.Accept;
                }
                else
                {
                    AddSaveIcon = Symbol.Add;
                }
            }
        }

        private async void AddSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Model.Saved)
            {
                await AppConstants.SpotifyClient.RemoveTrack(Model.Track.Id);
                Model.Saved = false;
                AddSaveIcon = Symbol.Add;
            }
            else
            {
                await AppConstants.SpotifyClient.SaveTrack(Model.Track.Id);
                Model.Saved = true;
                AddSaveIcon = Symbol.Accept;
            }
        }
    }
}
