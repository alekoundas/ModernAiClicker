using Business.Extensions;
using Business.Helpers;
using Business.Interfaces;
using Model.Business;
using Model.Enums;
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

        public TemplateMatchingResult SearchForTemplate(byte[] template, byte[] screenshot, TemplateMatchModesEnum? templateMatchModesEnum, bool removeTemplateFromResult)
        {
            //Bitmap template = (Bitmap)Image.FromFile(templatePath);
            //Bitmap? screenshot = SystemService.TakeScreenShot(windowRectangle);
            OpenCvSharp.TemplateMatchModes matchMode;
            switch (templateMatchModesEnum)
            {
                case TemplateMatchModesEnum.SqDiff:
                    matchMode = TemplateMatchModes.SqDiff;
                    break;
                case TemplateMatchModesEnum.SqDiffNormed:
                    matchMode = TemplateMatchModes.SqDiffNormed;
                    break;
                case TemplateMatchModesEnum.CCorr:
                    matchMode = TemplateMatchModes.CCorr;
                    break;
                case TemplateMatchModesEnum.CCorrNormed:
                    matchMode = TemplateMatchModes.CCorrNormed;
                    break;
                case TemplateMatchModesEnum.CCoeff:
                    matchMode = TemplateMatchModes.CCoeff;
                    break;
                case TemplateMatchModesEnum.CCoeffNormed:
                    matchMode = TemplateMatchModes.CCoeffNormed;
                    break;
                default:
                    matchMode = TemplateMatchModes.CCoeffNormed;
                    break;
            }

            Mat matTemplate = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(template.ToBitmapSource());
            Mat matScreenshot = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(screenshot.ToBitmapSource());
            Mat result = matScreenshot.MatchTemplate(matTemplate, matchMode);

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

            // Convert to %
            decimal minValue = (decimal)minConfidence;
            decimal maxValue = (decimal)maxConfidence;
            decimal r = (decimal)result.At<float>(maxLoc.Y, maxLoc.X);
            decimal percentage = ConvertToPercentage(r, templateMatchModesEnum, minValue, maxValue);

            // Convert to %
            maxConfidence *= 100d;

            // Draws rectangle in result image.
            DrawResultRectangle(percentage, matScreenshot, resultRectangle, removeTemplateFromResult);

            //Save result image to drive for debugging.
            string resultFilePath = Path.Combine(PathHelper.GetAppDataPath(), "Result.png");
            matScreenshot.SaveImage(resultFilePath);

            return new TemplateMatchingResult()
            {
                ResultRectangle = resultRectangle,
                Confidence = percentage,
                ResultImagePath = resultFilePath,
                ResultImage = matScreenshot.ToBytes()
            };
        }
        private decimal ConvertToPercentage(decimal r, TemplateMatchModesEnum? method, decimal minValue = decimal.MinValue, decimal maxValue = decimal.MaxValue)
        {
            decimal percentage;

            switch (method)
            {
                case TemplateMatchModesEnum.SqDiff: // SqDiff (Min: 0, Max: +\infty)
                        // maxValue should be the practical maximum from your result matrix
                    percentage = maxValue > 0 ? 100 * (1 - r / maxValue) : 100;
                    break;

                case TemplateMatchModesEnum.SqDiffNormed: // SqDiffNormed (Min: 0, Max: 1)
                    percentage = 100 * (1 - r);
                    break;

                case TemplateMatchModesEnum.CCorr: // CCorr (Min: -\infty, Max: +\infty)
                        // minValue and maxValue should be practical min/max from your result matrix
                    percentage = (maxValue - minValue) > 0 ? 100 * (r - minValue) / (maxValue - minValue) : 0;
                    break;

                case TemplateMatchModesEnum.CCorrNormed: // CCorrNormed (Min: -1, Max: 1)
                    percentage = 100 * (r + 1) / 2;
                    break;

                case TemplateMatchModesEnum.CCoeff: // CCoeff (Min: -\infty, Max: +\infty)
                        // minValue and maxValue should be practical min/max from your result matrix
                    percentage = (maxValue - minValue) > 0 ? 100 * (r - minValue) / (maxValue - minValue) : 0;
                    break;

                case TemplateMatchModesEnum.CCoeffNormed: // CCoeffNormed (Min: -1, Max: 1)
                    percentage = 100 * (r + 1) / 2;
                    break;

                default:
                    throw new ArgumentException("Invalid template matching method.");
            }

            // Clamp to [0, 100] to handle edge cases or arithmetic errors
            return Decimal.Max(0, Decimal.Min(100, percentage));
        }

        public TemplateMatchingResult SearchForTemplate(Bitmap template, Bitmap screenshot, TemplateMatchModesEnum? templateMatchModesEnum, bool removeTemplateFromResult)
        {
            //Bitmap template = (Bitmap)Image.FromFile(templatePath);
            //Bitmap? screenshot = SystemService.TakeScreenShot(windowRectangle);
            OpenCvSharp.TemplateMatchModes matchMode;
            switch (templateMatchModesEnum)
            {
                case TemplateMatchModesEnum.SqDiff:
                    matchMode = TemplateMatchModes.SqDiff;
                    break;
                case TemplateMatchModesEnum.SqDiffNormed:
                    matchMode = TemplateMatchModes.SqDiffNormed;
                    break;
                case TemplateMatchModesEnum.CCorr:
                    matchMode = TemplateMatchModes.CCorr;
                    break;
                case TemplateMatchModesEnum.CCorrNormed:
                    matchMode = TemplateMatchModes.CCorrNormed;
                    break;
                case TemplateMatchModesEnum.CCoeff:
                    matchMode = TemplateMatchModes.CCoeff;
                    break;
                case TemplateMatchModesEnum.CCoeffNormed:
                    matchMode = TemplateMatchModes.CCoeffNormed;
                    break;
                default:
                    matchMode = TemplateMatchModes.CCoeffNormed;
                    break;
            }

            Mat matTemplate = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(Convert(template));
            Mat matScreenshot = OpenCvSharp.WpfExtensions.BitmapSourceConverter.ToMat(Convert(screenshot));
            Mat result = matScreenshot.MatchTemplate(matTemplate, matchMode);

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

            // Convert to %
            maxConfidence *= 100d;

            // Draws rectangle in result image.
            DrawResultRectangle((decimal)maxConfidence, matScreenshot, resultRectangle, removeTemplateFromResult);

            //Save result image to drive for debugging.
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

        private static void DrawResultRectangle(decimal confidence, Mat matScreenshot, Rectangle resultRectangle, bool removeTemplateFromResult)
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
