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
            HelperMethods.NavigationService = navigationService;
            navigationService.Configure(nameof(LoginPage), typeof(LoginPage));
            navigationService.Configure(nameof(MainPage), typeof(MainPage));
            navigationService.Configure(nameof(AlbumsPage), typeof(AlbumsPage));
            navigationService.Configure(nameof(AlbumDetailPage), typeof(AlbumDetailPage));
            navigationService.Configure(nameof(SearchPage), typeof(SearchPage));
            navigationService.Configure(nameof(ArtistDetailPage), typeof(ArtistDetailPage));

            SimpleIoc.Default.Register(() => navigationService);
            SimpleIoc.Default.Register<LoginPageViewModel>();
            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<WebPlayerViewModel>();
            SimpleIoc.Default.Register<AlbumsPageViewModel>();
            SimpleIoc.Default.Register<MediaControlBarViewModel>();
            SimpleIoc.Default.Register<AlbumDetailPageViewModel>();
            SimpleIoc.Default.Register<SearchPageViewModel>();
            SimpleIoc.Default.Register<ArtistDetailPageViewModel>();
        }

        public LoginPageViewModel LoginPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<LoginPageViewModel>(); }
        }

        public MainPageViewModel MainPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<MainPageViewModel>(); }
        }

        public WebPlayerViewModel WebPlayerInstance
        {
            get { return ServiceLocator.Current.GetInstance<WebPlayerViewModel>(); }
        }

        public AlbumsPageViewModel AlbumsPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<AlbumsPageViewModel>(); }
        }

        public MediaControlBarViewModel MediaControlBarInstance
        {
            get { return ServiceLocator.Current.GetInstance<MediaControlBarViewModel>(); }
        }

        public AlbumDetailPageViewModel AlbumDetailPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<AlbumDetailPageViewModel>(); }
        }

        public SearchPageViewModel SearchPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<SearchPageViewModel>(); }
        }

        public ArtistDetailPageViewModel ArtistDetailPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<ArtistDetailPageViewModel>(); }
        }
    }
}
