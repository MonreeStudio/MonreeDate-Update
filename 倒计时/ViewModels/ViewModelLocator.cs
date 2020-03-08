using HHChaosToolkit.UWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace 倒计时.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            MainViewModel = new MainViewModel();
            ObjectPickerService = new ObjectPickerService();
            ObjectPickerService.Configure(typeof(WriteableBitmap).FullName, typeof(ImageCropperPickerViewModel).FullName, typeof(ImageCropperPickerPage));
        }

        public static ViewModelLocator Current => App.Current.Resources["Locator"] as ViewModelLocator;

        public MainViewModel MainViewModel { get; }
        public ImageCropperPickerViewModel ImageCropperPickerViewModel => new ImageCropperPickerViewModel();
        public ObjectPickerService ObjectPickerService { get; }
    }
}
