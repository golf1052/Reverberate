using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;

namespace Reverberate.ViewModels
{
    public class BrowsePageViewModel : ViewModelBase
    {
        private readonly NavigationService navigationService;

        public BrowsePageViewModel(NavigationService navigationService)
        {
            this.navigationService = navigationService;
        }
    }
}
