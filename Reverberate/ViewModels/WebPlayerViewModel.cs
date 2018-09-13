using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        public static string DeviceId { get; set; }

        public static event EventHandler<SpotifyWebPlaybackStateEventArgs> WebPlaybackStateChanged;

        static WebPlayerViewModel()
        {
            windowReady = false;

            webView = new WebView();
            webView.ScriptNotify += WebView_ScriptNotify;
            webView.Source = new Uri("ms-appx-web:///SpotifyWebView/index.html");
            WebView = webView;
        }

        private static async void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            JObject message = JObject.Parse(e.Value);
            Debug.WriteLine(message);
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
                    await SetAccessToken(AppConstants.SpotifyClient.AccessToken);
                }
            }
            else if (message["player"] != null)
            {
                if (message["player"]["ready"] != null)
                {
                    playerReady = (bool)message["player"]["ready"];
                    DeviceId = (string)message["player"]["deviceId"];
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

        private static async Task<string> InvokeScript(string functionName, params string[] args)
        {
            return await webView.InvokeScriptAsync(functionName, args);
        }
    }
}
