using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Windows.Media;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Reverb;

namespace Reverberate.ViewModels
{
    public class MediaControlBarViewModel : ViewModelBase
    {
        private Uri albumImageUrl;
        public Uri AlbumImageUrl
        {
            get { return albumImageUrl; }
            set { albumImageUrl = value; RaisePropertyChanged(nameof(AlbumImageUrl)); }
        }

        private string trackTitle;
        public string TrackTitle
        {
            get { return trackTitle; }
            set { trackTitle = value; RaisePropertyChanged(nameof(TrackTitle)); }
        }

        private string trackArtist;
        public string TrackArtist
        {
            get { return trackArtist; }
            set { trackArtist = value; RaisePropertyChanged(nameof(TrackArtist)); }
        }

        private Symbol playPauseIcon;
        public Symbol PlayPauseIcon
        {
            get { return playPauseIcon; }
            set { playPauseIcon = value; RaisePropertyChanged(nameof(PlayPauseIcon)); }
        }

        private string playPauseLabel;
        public string PlayPauseLabel
        {
            get { return playPauseLabel; }
            set { playPauseLabel = value; RaisePropertyChanged(nameof(PlayPauseLabel)); }
        }

        private bool shuffleButtonVisible;
        public bool ShuffleButtonVisible
        {
            get { return shuffleButtonVisible; }
            set { shuffleButtonVisible = value; RaisePropertyChanged(nameof(ShuffleButtonVisible)); }
        }

        private bool repeatButtonVisible;
        public bool RepeatButtonVisible
        {
            get { return repeatButtonVisible; }
            set { repeatButtonVisible = value; RaisePropertyChanged(nameof(RepeatButtonVisible)); }
        }

        private int volume;
        public int Volume
        {
            get { return volume; }
            set { volume = value; RaisePropertyChanged(nameof(Volume)); }
        }

        private bool devicesButtonVisible;
        public bool DevicesButtonVisible
        {
            get { return devicesButtonVisible; }
            set { devicesButtonVisible = value; RaisePropertyChanged(nameof(DevicesButtonVisible)); }
        }

        private SolidColorBrush devicesButtonColor;
        public SolidColorBrush DevicesButtonColor
        {
            get { return devicesButtonColor; }
            set { devicesButtonColor = value; RaisePropertyChanged(nameof(DevicesButtonColor)); }
        }

        private MenuFlyout devicesMenuFlyout;
        public MenuFlyout DevicesMenuFlyout
        {
            get { return devicesMenuFlyout; }
            set { devicesMenuFlyout = value; RaisePropertyChanged(nameof(DevicesMenuFlyout)); }
        }

        private bool connectionBarVisible;
        public bool ConnectionBarVisible
        {
            get { return connectionBarVisible; }
            set { connectionBarVisible = value; RaisePropertyChanged(nameof(ConnectionBarVisible)); }
        }

        private string connectionBarText;
        public string ConnectionBarText
        {
            get { return connectionBarText; }
            set { connectionBarText = value; RaisePropertyChanged(nameof(ConnectionBarText)); }
        }

        private SolidColorBrush connectionBarColor;
        public SolidColorBrush ConnectionBarColor
        {
            get { return connectionBarColor; }
            set { connectionBarColor = value; RaisePropertyChanged(nameof(ConnectionBarColor)); }
        }

        private SystemMediaTransportControls systemMediaTransportControls;
        private SystemMediaTransportControlsDisplayUpdater display;

        public static string ActiveDeviceId { get; set; }

        public MediaControlBarViewModel()
        {
            ShuffleButtonVisible = false;
            RepeatButtonVisible = false;
            Volume = 100;
            DevicesButtonVisible = true;
            DevicesButtonColor = new SolidColorBrush(Colors.Black);
            ConnectionBarVisible = false;
            ConnectionBarColor = AppConstants.RedBrush;
            ConnectionBarText = "Disconnected";
            PlayPauseIcon = Symbol.Play;
            systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
            systemMediaTransportControls.IsPlayEnabled = true;
            systemMediaTransportControls.IsPauseEnabled = true;
            systemMediaTransportControls.IsNextEnabled = true;
            systemMediaTransportControls.IsPreviousEnabled = true;
            systemMediaTransportControls.ButtonPressed += SystemMediaTransportControls_ButtonPressed;
            systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Closed;
            display = systemMediaTransportControls.DisplayUpdater;
            display.Type = MediaPlaybackType.Music;
            display.Update();

            WebPlayerViewModel.WebPlaybackStateChanged += WebPlayerViewModel_WebPlaybackStateChanged;
        }

        private async void SystemMediaTransportControls_ButtonPressed(SystemMediaTransportControls sender,
            SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            if (args.Button == SystemMediaTransportControlsButton.Play)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await Play();
                });
            }
            else if (args.Button == SystemMediaTransportControlsButton.Pause)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await Pause();
                });
                
            }
            else if (args.Button == SystemMediaTransportControlsButton.Previous)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await PreviousButton_Click();
                });
            }
            else if (args.Button == SystemMediaTransportControlsButton.Next)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await NextButton_Click();
                });
            }
        }

        public async Task SetupPlayback()
        {
            SetPlay();
            await UpdateDisplay();
        }

        private void WebPlayerViewModel_WebPlaybackStateChanged(object sender, SpotifyWebPlaybackStateEventArgs e)
        {
            if (e.State == null)
            {
                return;
            }
            if (e.State.Paused)
            {
                SetPaused();
            }
            else
            {
                SetPlay();
            }

            Uri albumImage = new Uri(e.State.TrackWindow.CurrentTrack.Album.GetLargestImage().Url);
            string trackTitle = e.State.TrackWindow.CurrentTrack.Name;
            string trackArtist = string.Join(", ", e.State.TrackWindow.CurrentTrack.Artists.Select(artist => artist.Name));
            string albumTitle = e.State.TrackWindow.CurrentTrack.Album.Name;
            UpdateDisplay(albumImage, trackTitle, trackArtist, albumTitle);
        }

        private async Task UpdateDisplay()
        {
            var currentlyPlaying = await AppConstants.SpotifyClient.GetCurrentlyPlayingPlayer();
            if (currentlyPlaying != null)
            {
                if (currentlyPlaying.Device != null)
                {
                    if (currentlyPlaying.Device.VolumePercent.HasValue)
                    {
                        Volume = currentlyPlaying.Device.VolumePercent.Value;
                    }
                }
                SetDeviceButtonColor(currentlyPlaying.Device.Id);
                if (currentlyPlaying.Track != null)
                {
                    Uri albumImage = new Uri(currentlyPlaying.Track.Album.GetLargestImage().Url);
                    string trackTitle = currentlyPlaying.Track.Name;
                    string trackArtist = string.Join(", ", currentlyPlaying.Track.Artists.Select(artist => artist.Name));
                    string albumTitle = currentlyPlaying.Track.Album.Name;
                    string albumArtist = string.Join(", ", currentlyPlaying.Track.Album.Artists.Select(artist => artist.Name));
                    UpdateDisplay(albumImage, trackTitle, trackArtist, albumTitle, albumArtist);
                }
            }

            var devicesList = await AppConstants.SpotifyClient.GetUserDevices();
            if (devicesList.Count == 0)
            {
                DevicesMenuFlyout = null;
            }
            else
            {
                MenuFlyout menuFlyout = new MenuFlyout();
                foreach (var device in devicesList)
                {
                    if (!device.IsRestricted)
                    {
                        MenuFlyoutItem menuFlyoutItem = new MenuFlyoutItem() { Text = device.Name };
                        menuFlyoutItem.Click += async (s, e) =>
                        {
                            MediaControlBarViewModel.ActiveDeviceId = device.Id;
                            SetDeviceButtonColor(device.Id);
                            await AppConstants.SpotifyClient.TransferPlayback(device.Id);
                        };
                        menuFlyout.Items.Add(menuFlyoutItem);
                    }
                }
                DevicesMenuFlyout = menuFlyout;
            }
        }

        private void SetDeviceButtonColor(string activeDeviceId)
        {
            if (WebPlayerViewModel.DeviceId == activeDeviceId)
            {
                DevicesButtonColor = HelperMethods.GetOppositeRequestedThemeColor();
            }
            else
            {
                DevicesButtonColor = AppConstants.SpotifyGreenBrush;
            }
        }

        public void UpdateDisplay(Uri albumImage, string trackTitle, string trackArtist, string albumTitle, string albumArtist = "")
        {
            AlbumImageUrl = albumImage;
            TrackTitle = trackTitle;
            TrackArtist = trackArtist;

            display.Thumbnail = RandomAccessStreamReference.CreateFromUri(albumImage);
            display.MusicProperties.Title = trackTitle;
            display.MusicProperties.Artist = trackArtist;
            display.MusicProperties.AlbumTitle = albumTitle;
            display.MusicProperties.AlbumArtist = albumArtist;
            display.Update();
        }

        public async Task PlayPauseButton_Click()
        {
            if (PlayPauseIcon == Symbol.Play)
            {
                await Play();
            }
            else if (PlayPauseIcon == Symbol.Pause)
            {
                await Pause();
            }
        }

        public async Task Play()
        {
            try
            {
                await AppConstants.SpotifyClient.Play(MediaControlBarViewModel.ActiveDeviceId);
            }
            catch (SpotifyException)
            {
                await WebPlayerViewModel.ReconnectClient(MediaControlBarViewModel.ActiveDeviceId);
                return;
            }
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            await SetupPlayback();
        }

        public async Task Pause()
        {
            try
            {
                await AppConstants.SpotifyClient.Pause(MediaControlBarViewModel.ActiveDeviceId);
            }
            catch (SpotifyException)
            {
                await WebPlayerViewModel.ReconnectClient(MediaControlBarViewModel.ActiveDeviceId);
                return;
            }
            SetPaused();
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            await UpdateDisplay();
        }

        public void SetPlay()
        {
            PlayPauseIcon = Symbol.Pause;
            PlayPauseLabel = "Pause";
            systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;
        }

        public void SetPaused()
        {
            PlayPauseIcon = Symbol.Play;
            PlayPauseLabel = "Play";
            systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
        }

        public async Task PreviousButton_Click()
        {
            try
            {
                await AppConstants.SpotifyClient.Previous(MediaControlBarViewModel.ActiveDeviceId);
            }
            catch (SpotifyException ex)
            {
                if (ex.Error.Message.Contains("Device"))
                {
                    await WebPlayerViewModel.ReconnectClient(MediaControlBarViewModel.ActiveDeviceId);
                }
                return;
            }
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            await UpdateDisplay();
        }

        public async Task NextButton_Click()
        {
            try
            {
                await AppConstants.SpotifyClient.Next(MediaControlBarViewModel.ActiveDeviceId);
            }
            catch (SpotifyException ex)
            {
                if (ex.Error.Message.Contains("Device"))
                {
                    await WebPlayerViewModel.ReconnectClient(MediaControlBarViewModel.ActiveDeviceId);
                }
                return;
            }
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            await UpdateDisplay();
        }

        public async Task VolumeSlider_ValueChanged(int volume)
        {
            try
            {
                await AppConstants.SpotifyClient.SetVolume(volume, MediaControlBarViewModel.ActiveDeviceId);
            }
            catch (SpotifyException)
            {
                await WebPlayerViewModel.ReconnectClient(MediaControlBarViewModel.ActiveDeviceId);
                return;
            }
        }

        public void SetConnected()
        {
            ConnectionBarText = "Connected";
            ConnectionBarColor = AppConstants.SpotifyGreenBrush;
            ConnectionBarVisible = true;
            Task hideConnectionBar = HideConnectionBar();
        }

        public void SetDisconnected()
        {
            ConnectionBarText = "Disconnected";
            ConnectionBarColor = AppConstants.RedBrush;
            ConnectionBarVisible = true;
        }

        public async Task HideConnectionBar()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            ConnectionBarVisible = false;
        }
    }
}
