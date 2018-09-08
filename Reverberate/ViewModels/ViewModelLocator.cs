using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Reverberate.Views;

namespace Reverberate.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            NavigationService navigationService = new NavigationService();
            navigationService.Configure(nameof(LoginPage), typeof(LoginPage));
            navigationService.Configure(nameof(MainPage), typeof(MainPage));
            navigationService.Configure(nameof(AlbumsPage), typeof(AlbumsPage));
            navigationService.Configure(nameof(AlbumDetailPage), typeof(AlbumDetailPage));

            SimpleIoc.Default.Register<INavigationService>(() => navigationService);
            SimpleIoc.Default.Register<LoginPageViewModel>();
            SimpleIoc.Default.Register<WebPlayerViewModel>();
            SimpleIoc.Default.Register<AlbumsPageViewModel>();
            SimpleIoc.Default.Register<MediaControlBarViewModel>();
            SimpleIoc.Default.Register<AlbumDetailPageViewModel>();
        }

        public LoginPageViewModel LoginPageInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginPageViewModel>();
            }
        }

        public WebPlayerViewModel WebPlayerInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WebPlayerViewModel>();
            }
        }

        public AlbumsPageViewModel AlbumsPageInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AlbumsPageViewModel>();
            }
        }

        public MediaControlBarViewModel MediaControlBarInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MediaControlBarViewModel>();
            }
        }

        public AlbumDetailPageViewModel AlbumDetailPageInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AlbumDetailPageViewModel>();
            }
        }
    }
}
