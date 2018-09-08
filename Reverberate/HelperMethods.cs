using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reverberate.ViewModels;

namespace Reverberate
{
    public static class HelperMethods
    {
        public static void AddRange<T>(this ObservableCollection<T> ts, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                ts.Add(item);
            }
        }

        public static ViewModelLocator GetViewModelLocator()
        {
            return (ViewModelLocator)App.Current.Resources["Locator"];
        }
    }
}
