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
    }
}
