using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using Reverberate.ViewModels;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

namespace Reverberate
{
    public static class HelperMethods
    {
        public static NavigationService NavigationService { get; set; }

        public static void AddRange<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                observableCollection.Add(item);
            }
        }

        public static ViewModelLocator GetViewModelLocator()
        {
            return (ViewModelLocator)App.Current.Resources["Locator"];
        }

        public static SolidColorBrush GetOppositeRequestedThemeColor()
        {
            if (App.Current.RequestedTheme == Windows.UI.Xaml.ApplicationTheme.Dark)
            {
                return new SolidColorBrush(Colors.White);
            }
            else
            {
                return new SolidColorBrush(Color.FromArgb(255, 29, 29, 29));
            }
        }

        public static string MinimalToString(this TimeSpan timeSpan)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool hasDays = false;
            if (timeSpan.Days > 0)
            {
                hasDays = true;
                stringBuilder.Append($"{timeSpan.Days}.");
            }
            bool hasHours = false;
            if (hasDays || timeSpan.Hours > 0)
            {
                hasHours = true;
                stringBuilder.Append($"{timeSpan.Hours}:");
            }
            if (hasHours)
            {
                stringBuilder.Append($"{timeSpan.Minutes.ToString("00")}:");
            }
            else
            {
                stringBuilder.Append($"{timeSpan.Minutes}:");
            }
            stringBuilder.Append(timeSpan.Seconds.ToString("00"));
            return stringBuilder.ToString();
        }

        public static void EnableBackButton(EventHandler<BackRequestedEventArgs> handler)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += handler;
        }

        public static void DisableBackButton(EventHandler<BackRequestedEventArgs> handler)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested -= handler;
        }

        public static void EnableBackButton()
        {
            EnableBackButton(GoBack);
        }

        public static void DisableBackButton()
        {
            DisableBackButton(GoBack);
        }

        private static void GoBack(object sender, BackRequestedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            e.Handled = true;
        }
    }
}
