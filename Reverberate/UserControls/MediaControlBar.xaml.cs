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
    public sealed partial class MediaControlBar : UserControl
    {
        private bool loaded = false;

        public MediaControlBarViewModel Vm
        {
            get
            {
                return (MediaControlBarViewModel)DataContext;
            }
        }

        public MediaControlBar()
        {
            this.InitializeComponent();
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            await Vm.PreviousButton_Click();
        }

        private async void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            await Vm.PlayPauseButton_Click();
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            await Vm.NextButton_Click();
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DevicesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (loaded)
            {
                await Vm.VolumeSlider_ValueChanged((int)e.NewValue);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }
    }
}
