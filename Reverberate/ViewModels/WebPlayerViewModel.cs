using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reverb;
using Reverb.Models.WebPlayer;
using Windows.Media;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Windows.UI.Xaml.Controls;

namespace Reverberate.ViewModels
{
    public class WebPlayerViewModel : ViewModelBase
    {
        private static WebView webView;
        public static WebView WebView { get; private set; }

        private static bool windowReady;
        private static bool playerReady;
        private static Task checkActivityTask;
        private static CancellationTokenSource checkActivityCancellationTokenSource;
        public static string DeviceId { get; private set; }
        public static bool PlayerConnected { get; private set; }
        public static bool PlayerReconnecting { get; private set; }

        public static event EventHandler<SpotifyWebPlaybackStateEventArgs> WebPlaybackStateChanged;

        static WebPlayerViewModel()
        {
            windowReady = false;

            webView = new WebView();
            webView.ScriptNotify += WebView_ScriptNotify;
            webView.Source = new Uri("ms-appx-web:///SpotifyWebView/index.html");
            WebView = webView;

            PlayerConnected = false;
        }

        private static async void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            JObject message = JObject.Parse(e.Value);
            if (message["windowReady"] != null)
            {
                windowReady = (bool)message["windowReady"];
                if (windowReady)
                {
                    string deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
                    string deviceName = "Windows 10";
                    if (deviceFamily.Contains("Mobile"))
                    {
                        deviceName = "Windows Phone";
                    }
                    else if (deviceFamily.Contains("Xbox"))
                    {
                        deviceName = "Xbox";
                    }
                    else if (deviceFamily.Contains("Holographic"))
                    {
                        deviceName = "HoloLens";
                    }
                    else if (deviceFamily.Contains("IoT"))
                    {
                        deviceName = "Windows IoT";
                    }
                    else if (deviceFamily.Contains("Desktop"))
                    {
                        deviceName = "Windows 10";
                    }
                    else
                    {
                        deviceName = "Windows 10";
                    }
                    EasClientDeviceInformation info = new EasClientDeviceInformation();
                    await SetPlayerName($"Reverberate on {deviceName}: {info.FriendlyName}");
                    await SetAccessToken(AppConstants.SpotifyClient.AccessToken);
                    await CreatePlayer();
                    await ConnectPlayer();
                }
            }
            else if (message["request"] != null)
            {
                string request = (string)message["request"];
                if (request == "accessToken")
                {
                    if (AppConstants.SpotifyClient.AccessTokenExpiresAt > DateTimeOffset.UtcNow)
                    {
                        await SetAccessToken(AppConstants.SpotifyClient.AccessToken);
                    }
                    else
                    {
                        await SetAccessToken(await AppConstants.SpotifyClient.RefreshAccessToken());
                    }
                }
            }
            else if (message["player"] != null)
            {
                if (message["player"]["ready"] != null)
                {
                    playerReady = (bool)message["player"]["ready"];
                    DeviceId = (string)message["player"]["deviceId"];
                    if (playerReady)
                    {
                        if (!PlayerConnected)
                        {
                            PlayerConnected = true;
                            if (checkActivityTask != null)
                            {
                                checkActivityCancellationTokenSource.Cancel();
                                checkActivityTask = null;
                                checkActivityCancellationTokenSource.Dispose();
                            }
                            checkActivityCancellationTokenSource = new CancellationTokenSource();
                            checkActivityTask = CheckActivity(checkActivityCancellationTokenSource.Token);
                            HelperMethods.GetViewModelLocator().MediaControlBarInstance.SetConnected();
                        }   
                    }
                    else
                    {
                        PlayerConnected = false;
                        if (checkActivityTask != null)
                        {
                            checkActivityCancellationTokenSource.Cancel();
                            checkActivityTask = null;
                            checkActivityCancellationTokenSource.Dispose();
                        }
                        HelperMethods.GetViewModelLocator().MediaControlBarInstance.SetDisconnected();
                    }
                    if (MediaControlBarViewModel.ActiveDeviceId == null)
                    {
                        MediaControlBarViewModel.ActiveDeviceId = DeviceId;
                    }
                }
                if (message["player"]["state"] != null)
                {
                    SpotifyWebPlaybackState playbackState = JsonConvert.DeserializeObject<SpotifyWebPlaybackState>(message["player"]["state"].ToString());
                    WebPlaybackStateChanged?.Invoke(null, new SpotifyWebPlaybackStateEventArgs(playbackState));
                }
                if (message["player"]["connected"] != null)
                {
                    if (!(bool)message["player"]["connected"])
                    {
                        PlayerConnected = false;
                        HelperMethods.GetViewModelLocator().MediaControlBarInstance.SetDisconnected();
                    }
                }
            }
        }

        private static async Task CheckActivity(CancellationToken cancellationToken)
        {
            while (true)
            {
                var devicesList = await AppConstants.SpotifyClient.GetUserDevices();
                if (devicesList.Count == 0)
                {
                    await ReconnectPlayer();
                }
                else
                {
                    if (devicesList.FirstOrDefault(device => device.Id == DeviceId) == null)
                    {
                        await ReconnectPlayer();
                    }
                }
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

        public static async Task ReconnectPlayer(string deviceId)
        {
            if (deviceId == DeviceId)
            {
                await ReconnectPlayer();
            }
        }

        public static async Task ReconnectPlayer()
        {
            bool wasPlaying = HelperMethods.GetViewModelLocator().MediaControlBarInstance.Playing;
            PlayerReconnecting = true;
            HelperMethods.GetViewModelLocator().MediaControlBarInstance.ReconnectButtonEnabled = false;
            await DisconnectPlayer();
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            await ConnectPlayer();
            PlayerReconnecting = false;
            HelperMethods.GetViewModelLocator().MediaControlBarInstance.ReconnectButtonEnabled = true;
            if (wasPlaying && !HelperMethods.GetViewModelLocator().MediaControlBarInstance.Playing)
            {
                await HelperMethods.GetViewModelLocator().MediaControlBarInstance.Play();
            }
        }

        public static async Task SetPlayerName(string name)
        {
            await InvokeScript("setPlayerName", name);
        }

        public static async Task SetAccessToken(string accessToken)
        {
            await InvokeScript("setAccessToken", accessToken);
        }

        public static async Task CreatePlayer()
        {
            await InvokeScript("createPlayer");
        }

        public static async Task ConnectPlayer()
        {
            await InvokeScript("connectPlayer");
        }

        public static async Task DisconnectPlayer()
        {
            HelperMethods.GetViewModelLocator().MediaControlBarInstance.SetDisconnected();
            await InvokeScript("disconnectPlayer");
        }

        private static async Task<string> InvokeScript(string functionName, params string[] args)
        {
            return await webView.InvokeScriptAsync(functionName, args);
        }
    }
}
