using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace 倒计时.Manager
{
    internal static class AvatarImage
    {
        // Prefilter large photos before they are scaled down by the avatar
        // control. Keep enough pixels for the preview and high-DPI displays.
        public static async Task<WriteableBitmap> LoadAsync(byte[] encoded)
        {
            using (var input = new InMemoryRandomAccessStream())
            {
                await input.WriteAsync(encoded.AsBuffer());
                input.Seek(0);
                var decoder = await BitmapDecoder.CreateAsync(input);
                double scale = Math.Min(1, 512.0 / Math.Max(decoder.OrientedPixelWidth, decoder.OrientedPixelHeight));
                uint width = Math.Max(1, (uint)Math.Round(decoder.OrientedPixelWidth * scale));
                uint height = Math.Max(1, (uint)Math.Round(decoder.OrientedPixelHeight * scale));
                var pixels = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied,
                    new BitmapTransform { ScaledWidth = width, ScaledHeight = height, InterpolationMode = BitmapInterpolationMode.Fant },
                    ExifOrientationMode.RespectExifOrientation, ColorManagementMode.ColorManageToSRgb);
                var result = new WriteableBitmap((int)width, (int)height);
                byte[] buffer = pixels.DetachPixelData();
                using (var output = result.PixelBuffer.AsStream())
                    await output.WriteAsync(buffer, 0, buffer.Length);
                result.Invalidate();
                return result;
            }
        }
    }
}
