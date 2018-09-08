using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json.Linq;
using Reverb;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Reverberate.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;

        public LoginPageViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public WebView webView;
        private string state;

        public void OnNavigatedTo()
        {
            List<SpotifyConstants.SpotifyScopes> scopes = new List<SpotifyConstants.SpotifyScopes>()
            {
                SpotifyConstants.SpotifyScopes.UserLibraryRead,
                SpotifyConstants.SpotifyScopes.UserLibraryModify,
                SpotifyConstants.SpotifyScopes.PlaylistReadPrivate,
                SpotifyConstants.SpotifyScopes.PlaylistModifyPublic,
                SpotifyConstants.SpotifyScopes.PlaylistModifyPrivate,
                SpotifyConstants.SpotifyScopes.PlaylistReadCollaborative,
                SpotifyConstants.SpotifyScopes.UserReadRecentlyPlayed,
                SpotifyConstants.SpotifyScopes.UserTopRead,
                SpotifyConstants.SpotifyScopes.UserReadPrivate,
                SpotifyConstants.SpotifyScopes.UserReadEmail,
                SpotifyConstants.SpotifyScopes.UserReadBirthdate,
                SpotifyConstants.SpotifyScopes.Streaming,
                SpotifyConstants.SpotifyScopes.AppRemoteControl,
                SpotifyConstants.SpotifyScopes.UserModifyPlaybackState,
                SpotifyConstants.SpotifyScopes.UserReadCurrentlyPlaying,
                SpotifyConstants.SpotifyScopes.UserReadPlaybackState,
                SpotifyConstants.SpotifyScopes.UserFollowModify,
                SpotifyConstants.SpotifyScopes.UserFollowRead
            };
            string state = Guid.NewGuid().ToString();
            this.state = state;
            webView.Source = new Uri(AppConstants.SpotifyClient.GetAuthorizeUrl(scopes, state));
        }

        public async Task WebView_NavigationComplete(WebView webView)
        {
            string schemeHost = $"{webView.Source.Scheme}://{webView.Source.Host}";
            if (schemeHost == Secrets.RedirectUrl)
            {
                webView.Visibility = Visibility.Collapsed;
                await AppConstants.SpotifyClient.ProcessRedirect(webView.Source, state);
                navigationService.NavigateTo(nameof(MainPage));
            }
        }
    }
}
