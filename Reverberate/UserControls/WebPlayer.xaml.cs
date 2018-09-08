using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Reverberate.ViewModels;
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
    public sealed partial class WebPlayer : UserControl
    {
        public WebPlayer()
        {
            this.InitializeComponent();
            
            //if (!Grid.Children.Contains(WebPlayerViewModel.WebView))
            //{
            //    Grid.Children.Add(WebPlayerViewModel.WebView);
            //}
        }

        public WebPlayerViewModel Vm
        {
            get
            {
                return (WebPlayerViewModel)DataContext;
            }
        }
    }
}
