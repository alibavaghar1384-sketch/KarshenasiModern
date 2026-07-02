using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KarshenasiModern.Models;
using KarshenasiModern.Services;

namespace KarshenasiModern.Views.Controls
{
    public partial class CarBodyDiagram : UserControl
    {
        // مکانیزم کش برای جلوگیری از پردازش تکراری تصاویر و افزایش سرعت برنامه
        private readonly Dictionary<string, BitmapSource> _coloredImagesCache = new();

        public CarBodyDiagram()
        {
            InitializeComponent();
        }

        public void SetPartStatus(string partName, BodyPartStatus status)
        {
            List<Image> targetImages = new();
            List<string> assetPaths = new();

            switch (partName)
            {
                case "کاپوت":
                    targetImages.Add(HoodPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/hood.png");
                    targetImages.Add(FrontViewHoodPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/front_view_hood.png");
                    break;

                case "سقف":
                    targetImages.Add(RoofPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/roof.png");
                    targetImages.Add(FrontViewRoofPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/front_view_roof.png");
                    targetImages.Add(RearViewRoofPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/rear_view_roof.png");
                    break;

                case "صندوق":
                case "صندوق عقب":
                    targetImages.Add(TrunkPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/trunk.png");
                    targetImages.Add(RearViewTrunkPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/rear_view_trunk.png");
                    break;

                case "سپر جلو":
                    targetImages.Add(FrontViewBumperPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/front_view_bumper.png");
                    targetImages.Add(LeftViewFrontBumperPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/left_view_front_bumper.png");
                    targetImages.Add(RightViewFrontBumperPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/right_view_front_bumper.png");
                    break;

                case "سپر عقب":
                    targetImages.Add(RearViewBumperPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/rear_view_bumper.png");
                    targetImages.Add(LeftViewRearBumperPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/left_view_rear_bumper.png");
                    targetImages.Add(RightViewRearBumperPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/right_view_rear_bumper.png");
                    break;

                case "درب جلو راننده":
                case "در جلو راننده":
                    targetImages.Add(FrontLeftDoorPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/front_left_door.png");
                    break;

                case "درب جلو شاگرد":
                case "در جلو شاگرد":
                    targetImages.Add(FrontRightDoorPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/front_right_door.png");
                    break;

                case "درب عقب راننده":
                case "در عقب راننده":
                    targetImages.Add(RearLeftDoorPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/rear_left_door.png");
                    break;

                case "درب عقب شاگرد":
                case "در عقب شاگرد":
                    targetImages.Add(RearRightDoorPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/rear_right_door.png");
                    break;

                case "گلگیر جلو راننده":
                    targetImages.Add(FrontLeftFenderPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/front_left_fender.png");
                    break;

                case "گلگیر جلو شاگرد":
                    targetImages.Add(FrontRightFenderPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/front_right_fender.png");
                    break;

                case "گلگیر عقب راننده":
                    targetImages.Add(RearLeftFenderPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/rear_left_fender.png");
                    break;

                case "گلگیر عقب شاگرد":
                    targetImages.Add(RearRightFenderPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/rear_right_fender.png");
                    break;

                case "رکاب راننده":
                    targetImages.Add(LeftSillPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/left_sill.png");
                    break;

                case "رکاب شاگرد":
                    targetImages.Add(RightSillPart); assetPaths.Add("pack://application:,,,/KarshenasiModern;component/Assets/Car/right_sill.png");
                    break;
            }

            // دریافت رنگ کاملاً دقیق و هماهنگ با سرویس رسمی پروژه شما
            Color targetColor = GetColorFromStatus(status);

            for (int i = 0; i < targetImages.Count; i++)
            {
                Image imgControl = targetImages[i];
                string path = assetPaths[i];

                if (imgControl == null) continue;

                if (status == BodyPartStatus.Healthy)
                {
                    imgControl.Visibility = Visibility.Collapsed;
                    imgControl.Source = null;
                }
                else
                {
                    imgControl.Visibility = Visibility.Visible;
                    imgControl.Source = GetOrCreateColoredSource(path, targetColor);
                }
            }
        }

        private BitmapSource GetOrCreateColoredSource(string packUri, Color color)
        {
            string cacheKey = $"{packUri}_{color.A}_{color.R}_{color.G}_{color.B}";
            if (_coloredImagesCache.TryGetValue(cacheKey, out var cachedBitmap))
            {
                return cachedBitmap;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(packUri);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                // رفع باگ فرمت فتوشاپ: تبدیل لایه‌های تک‌رنگ و Grayscale به ساختار 32بیتی استاندارد جهت رندر صحیح رنگ‌ها
                var convertedBitmap = new FormatConvertedBitmap();
                convertedBitmap.BeginInit();
                convertedBitmap.Source = bitmap;
                convertedBitmap.DestinationFormat = PixelFormats.Bgra32;
                convertedBitmap.EndInit();

                int width = convertedBitmap.PixelWidth;
                int height = convertedBitmap.PixelHeight;

                int stride = width * 4;
                byte[] pixels = new byte[height * stride];
                convertedBitmap.CopyPixels(pixels, stride, 0);

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    byte alpha = pixels[i + 3];
                    if (alpha > 0)
                    {
                        // اعمال دقیق رنگ جدید روی پیکسل‌های قطعه ماشین
                        pixels[i] = color.B;     // آبی
                        pixels[i + 1] = color.G; // سبز
                        pixels[i + 2] = color.R; // قرمز

                        // تلفیق کانال آلفا جهت حفظ سافت بودن و آنتی‌آلیاسینگ لبه‌های بیرونی قطعات
                        pixels[i + 3] = (byte)((alpha * color.A) / 255);
                    }
                }

                // حل دائمی و قاطعانه باگ جابه‌جایی و کات شدن عکس‌ها با تنظیم اجباری رزولوشن روی 96 DPI
                var finalSource = BitmapSource.Create(
                    width,
                    height,
                    96,
                    96,
                    PixelFormats.Bgra32,
                    null,
                    pixels,
                    stride);

                finalSource.Freeze(); // قفل حافظه جهت ایجاد پرفورمنس فوق‌العاده سریع و روان
                _coloredImagesCache[cacheKey] = finalSource;
                return finalSource;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Color GetColorFromStatus(BodyPartStatus status)
        {
            // خواندن مستقیم رنگ از سرویس اصلی شما (BodyPartColorService) جهت اعمال رنگ‌های کاملاً واقعی
            Brush brush = BodyPartColorService.GetColor(status);

            if (brush is SolidColorBrush solidBrush)
            {
                Color baseColor = solidBrush.Color;

                // مقدار آلفا (شفافیت) روی 180 تنظیم شده تا خطوط مشکی و ظریف زیرین بدنه خودرو از بین نروند.
                // اگر می‌خواهید رنگ‌ها کاملاً غلیظ و توپر باشند، عدد 180 را به 255 تغییر دهید.
                return Color.FromArgb(180, baseColor.R, baseColor.G, baseColor.B);
            }

            return Colors.Transparent;
        }
    }
}