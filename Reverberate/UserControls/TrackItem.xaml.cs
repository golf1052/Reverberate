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

        public SpotifyTrack Model
        {
            get { return (SpotifyTrack)DataContext; }
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
                TrackNumber = Model.TrackNumber;
                AddSaveIcon = Symbol.Accept;
                TrackTitle = Model.Name;
                IsExplicit = Model.Explicit;
                TimeSpan trackLength = TimeSpan.FromMilliseconds(Model.Duration);
                TrackLength = trackLength.MinimalToString();
            }
        }
    }
}
