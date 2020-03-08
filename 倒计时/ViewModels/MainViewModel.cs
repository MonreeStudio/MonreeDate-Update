using HHChaosToolkit.UWP.Mvvm;
using HHChaosToolkit.UWP.Picker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using 倒计时.Models;

namespace 倒计时.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand CropImageCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var source = await PickImage();
                    if (source != null && ViewModelLocator.Current.ObjectPickerService != null)
                    {
                        var config = new ImageCropperConfig
                        {
                            ImageFile = source
                        };
                        var croppedImage = await CropImage(config);
                        if (croppedImage != null)
                        {
                            CroppedImage = croppedImage;
                        }
                    }
                });
            }
        }

        public ICommand CropCircularImageCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var source = await PickImage();
                    if (source != null && ViewModelLocator.Current.ObjectPickerService != null)
                    {
                        var config = new ImageCropperConfig
                        {
                            ImageFile = source,
                            CircularCrop = true
                        };
                        var croppedImage = await CropImage(config);
                        if (croppedImage != null)
                        {
                            CroppedCircularImage = croppedImage;
                        }
                    }
                });
            }
        }

        public ICommand CropSquareImageCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var source = await PickImage();
                    if (source != null && ViewModelLocator.Current.ObjectPickerService != null)
                    {
                        var config = new ImageCropperConfig
                        {
                            ImageFile = source,
                            AspectRatio = 1
                        };
                        var croppedImage = await CropImage(config);
                        if (croppedImage != null)
                        {
                            CroppedSquareImage = croppedImage;
                        }
                    }
                });
            }
        }

        public ICommand CropLandscapeImageCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var source = await PickImage();
                    if (source != null && ViewModelLocator.Current.ObjectPickerService != null)
                    {
                        var config = new ImageCropperConfig
                        {
                            ImageFile = source,
                            AspectRatio = 16d / 9d
                        };
                        var croppedImage = await CropImage(config);
                        if (croppedImage != null)
                        {
                            CroppedLandscapeImage = croppedImage;
                        }
                    }
                });
            }
        }

        private ImageSource _croppedImage;
        public ImageSource CroppedImage
        {
            get => _croppedImage;
            set => Set(ref _croppedImage, value);
        }

        private ImageSource _croppedCircularImage;
        public ImageSource CroppedCircularImage
        {
            get => _croppedCircularImage;
            set => Set(ref _croppedCircularImage, value);
        }

        private ImageSource _croppedLandscapeImage;
        public ImageSource CroppedLandscapeImage
        {
            get => _croppedLandscapeImage;
            set => Set(ref _croppedLandscapeImage, value);
        }

        private ImageSource _croppedSquareImage;
        public ImageSource CroppedSquareImage
        {
            get => _croppedSquareImage;
            set => Set(ref _croppedSquareImage, value);
        }


        private async Task<ImageSource> CropImage(ImageCropperConfig config)
        {
            var startOption = new PickerOpenOption
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            var ret = await ViewModelLocator.Current.ObjectPickerService.PickSingleObjectAsync<WriteableBitmap>(
                typeof(ImageCropperPickerViewModel).FullName, config, startOption);
            if (!ret.Canceled)
            {
                return ret.Result;
            }
            return null;
        }

        private IAsyncOperation<StorageFile> PickImage()
        {
            var filePicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter =
                {
                    ".png", ".jpg", ".jpeg"
                }
            };
            return filePicker.PickSingleFileAsync();
        }

    
}
}
