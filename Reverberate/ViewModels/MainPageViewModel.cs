using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Reverberate.Views;
using Windows.UI.Xaml.Controls;

namespace Reverberate.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly NavigationService navigationService;

        public MainPageViewModel(NavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public void SetNavigationServiceFrame(Frame frame)
        {
            navigationService.CurrentFrame = frame;
        }

        public void OnNavigatedTo()
        {
            navigationService.NavigateTo(nameof(AlbumsPage));
        }
    }
}
