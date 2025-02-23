using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace Business.Extensions
{
    public static class ByteArrayExtensions
    {
        public static BitmapSource ToBitmapSource(this byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                throw new ArgumentException("Image data is empty or null");

            using (var ms = new MemoryStream(imageData))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze(); // Allows cross-thread access
                return bitmap;
            }
        }

        public static byte[] ToByteArray(this Image image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            using MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
}
