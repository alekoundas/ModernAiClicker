using Business.Helpers;
using Business.Interfaces;
using Model.Business;
using OpenCvSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Rectangle = Model.Structs.Rectangle;

namespace Business.Services
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class TemplateSearchService : ITemplateSearchService
    {
        public ISystemService SystemService;
        public TemplateSearchService(ISystemService systemService)
        {
            SystemService = systemService;
        }


        public TemplateMatchingResult SearchForTemplate(Bitmap template, Bitmap screenshot, bool removeTemplateFromResult)
        {
            //Bitmap template = (Bitmap)Image.FromFile(templatePath);
            //Bitmap? screenshot = SystemService.TakeScreenShot(windowRectangle);

            Mat matTemplate = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(Convert(template));
            Mat matScreenshot = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(Convert(screenshot));
            Mat result = matScreenshot.MatchTemplate(matTemplate, OpenCvSharp.TemplateMatchModes.CCoeffNormed);

            // Execute search.
            result.MinMaxLoc(out double minConfidence,
                             out double maxConfidence,
                             out OpenCvSharp.Point minLoc,
                             out OpenCvSharp.Point maxLoc);

            // Get center possition of template image.
            Rectangle resultRectangle = new Rectangle()
            {
                Top = maxLoc.Y,
                Left = maxLoc.X,
                Right = maxLoc.X + matTemplate.Width,
                Bottom = maxLoc.Y + matTemplate.Height,
            };

            // Draws rectangle in result image.
            DrawResultRectangle(maxConfidence, matScreenshot, resultRectangle, removeTemplateFromResult);

            //Save result image to drive
            string resultFilePath = Path.Combine(PathHelper.GetAppDataPath(), "Result.png");
            matScreenshot.SaveImage(resultFilePath);

            return new TemplateMatchingResult()
            {
                ResultRectangle = resultRectangle,
                Confidence = (decimal)maxConfidence,
                ResultImagePath = resultFilePath,
                ResultImage = matScreenshot.ToBytes()
            };
        }

        private static BitmapSource Convert(Bitmap bitmap)
        {
            var rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width,
                bitmapData.Height,
                bitmap.HorizontalResolution,
                bitmap.VerticalResolution,
                PixelFormats.Bgra32,
                null,
                bitmapData.Scan0,
                bitmapData.Stride * bitmapData.Height,
                bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            bitmap.Dispose();

            return bitmapSource;
        }

        private static void DrawResultRectangle(double confidence, Mat matScreenshot, Rectangle resultRectangle, bool removeTemplateFromResult)
        {
            OpenCvSharp.Point point1 = new OpenCvSharp.Point(resultRectangle.Left, resultRectangle.Top);
            OpenCvSharp.Point point2 = new OpenCvSharp.Point(resultRectangle.Right, resultRectangle.Bottom);

            if (removeTemplateFromResult)
                matScreenshot.Rectangle(point1, point2, new Scalar(0, 0, 255), -1);
            else
                matScreenshot.Rectangle(point1, point2, new Scalar(0, 0, 255), 2);

            //string text = $"Confidence: {Math.Round((float)(maxVal), 2)}";
            string text = Math.Round((float)(confidence), 2).ToString();
            HersheyFonts font = HersheyFonts.HersheyPlain;
            Scalar textColor = new Scalar(255, 0, 0);
            int fontScale = 2;
            int thickness = 4;

            matScreenshot.PutText(text, point1, font, fontScale, textColor, thickness);
        }
    }
}
