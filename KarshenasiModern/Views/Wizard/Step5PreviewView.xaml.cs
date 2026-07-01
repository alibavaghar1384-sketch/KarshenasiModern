using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using KarshenasiModern.Database;
using KarshenasiModern.Models;
using KarshenasiModern.Services;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace KarshenasiModern.Views.Wizard
{
    public partial class Step5PreviewView : UserControl
    {
        public Step5PreviewView()
        {
            InitializeComponent();

            Loaded += Step5PreviewView_Loaded;
            BackButton.Click += BackButton_Click;
            PrintButton.Click += PrintButton_Click;
            FinishButton.Click += FinishButton_Click; // هندلر دکمه ثبت نهایی ثبت شد
        }

        private async void Step5PreviewView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using var db = new AppDbContext();

                var inspection = await db.Inspections
                    .Include(x => x.Car)
                    .Include(x => x.BodyParts)
                    .Include(x => x.Photos)
                    .FirstOrDefaultAsync(x => x.Id == CurrentSession.CurrentInspectionId);

                if (inspection == null)
                {
                    MessageBox.Show("اطلاعات پرونده یافت نشد.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ReportNumberText.Text = $"KM-1405-{inspection.Id:D5}";

                var pc = new PersianCalendar();
                InspectionDateText.Text = $"{pc.GetYear(inspection.CreatedAt)}/" +
                                         $"{pc.GetMonth(inspection.CreatedAt):00}/" +
                                         $"{pc.GetDayOfMonth(inspection.CreatedAt):00}";

                ExpertNameText.Text = inspection.ExpertName ?? "-";

                MileageText.Text = inspection.Mileage > 0 ? inspection.Mileage.ToString("N0") : "0";
                EngineStatusText.Text = !string.IsNullOrEmpty(inspection.EngineStatus) ? inspection.EngineStatus : "ثبت نشده";
                GearboxStatusText.Text = !string.IsNullOrEmpty(inspection.GearboxStatus) ? inspection.GearboxStatus : "ثبت نشده";
                ChassisStatusText.Text = !string.IsNullOrEmpty(inspection.ChassisStatus) ? inspection.ChassisStatus : "ثبت نشده";
                AirbagStatusText.Text = !string.IsNullOrEmpty(inspection.AirbagStatus) ? inspection.AirbagStatus : "ثبت نشده";

                CustomerText.Text = inspection.Car?.CustomerName ?? "-";
                PhoneText.Text = inspection.Car?.CustomerPhone ?? "-";
                PlateText.Text = inspection.Car?.Plate ?? "-";
                ChassisText.Text = inspection.Car?.ChassisNumber ?? "-";
                BrandText.Text = inspection.Car?.Brand ?? "-";
                ModelText.Text = inspection.Car?.Model ?? "-";

                DescriptionText.Text = string.IsNullOrWhiteSpace(inspection.Description)
                    ? "توضیحاتی توسط کارشناس ثبت نشده است."
                    : inspection.Description.Trim();
                DescriptionText.UpdateLayout();

                try
                {
                    LogoImage.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/logo.png"));
                }
                catch { }

                if (inspection.BodyParts != null)
                {
                    foreach (var part in inspection.BodyParts)
                    {
                        CarDiagram.SetPartStatus(part.PartName, part.Status);
                    }
                    BodyPartsGrid.ItemsSource = inspection.BodyParts;
                }

                var loadedPhotosList = new List<PhotoViewModel>();

                if (inspection.Photos != null)
                {
                    foreach (var photo in inspection.Photos)
                    {
                        if (string.IsNullOrWhiteSpace(photo.Category))
                            continue;

                        BitmapImage bitmap;
                        try
                        {
                            bitmap = LoadPhoto(photo.EncryptedFileName);
                        }
                        catch
                        {
                            continue;
                        }

                        string persianTitle = photo.Category.Trim() switch
                        {
                            "Front" => "عکس جلو",
                            "Rear" => "عکس عقب",
                            "DriverSide" or "Left" => "عکس سمت راننده",
                            "PassengerSide" or "Right" => "عکس سمت شاگرد",
                            "Engine" => "عکس موتور",
                            "Interior" => "عکس کابین",
                            _ => photo.Category
                        };

                        loadedPhotosList.Add(new PhotoViewModel
                        {
                            Title = persianTitle,
                            ImageSource = bitmap
                        });
                    }
                }

                DynamicPhotosControl.ItemsSource = loadedPhotosList;

                if (inspection.BodyParts != null)
                {
                    int totalIssues = inspection.BodyParts.Count(x => x.Status != BodyPartStatus.Healthy);
                    int severeIssues = inspection.BodyParts.Count(x => x.Status == BodyPartStatus.Replaced || x.Status == BodyPartStatus.Damaged);

                    if (totalIssues == 0)
                    {
                        FinalResultCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8F5E9"));
                        FinalResultCard.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C8E6C9"));
                        FinalResultText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E7D32"));
                        FinalResultIcon.Foreground = FinalResultText.Foreground;
                        FinalResultIcon.Text = "✓";
                        FinalResultText.Text = "خودرو از نظر اصالت و سلامت بدنه کاملاً سالم و بدون رنگ ارزیابی شد.";
                    }
                    else if (severeIssues > 0)
                    {
                        FinalResultCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEBEE"));
                        FinalResultCard.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCDD2"));
                        FinalResultText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C62828"));
                        FinalResultIcon.Foreground = FinalResultText.Foreground;
                        FinalResultIcon.Text = "⚠";
                        FinalResultText.Text = $"هشدار: خودرو دارای سابقه تعویض یا آسیب‌دیدگی شدید در {severeIssues} قطعه (مجموعاً {totalIssues} افت وضعیت بدنه) می‌باشد.";
                    }
                    else
                    {
                        FinalResultCard.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF3E0"));
                        FinalResultCard.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE0B2"));
                        FinalResultText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E65100"));
                        FinalResultIcon.Foreground = FinalResultText.Foreground;
                        FinalResultIcon.Text = "ℹ";
                        FinalResultText.Text = $"خودرو دارای {totalIssues} مورد اصلاحات جزیی بدنه (شامل سابقه رنگ‌شدگی، لیسه‌گیری, صافکاری بی‌رنگ، آفتاب‌سوختگی یا خط‌ و‌ خش) است.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری پیش‌نمایش گزارش: {ex.Message}", "خطای سیستم", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private BitmapImage LoadPhoto(string encryptedFileName)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Images", encryptedFileName);
            if (!File.Exists(file)) throw new FileNotFoundException(file);

            var bytes = ImageEncryptionService.DecryptToBytes(file);
            var bitmap = new BitmapImage();
            using (var stream = new MemoryStream(bytes))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            bitmap.Freeze();
            return bitmap;
        }

        private async void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var db = new AppDbContext();
                var inspection = await db.Inspections.FirstOrDefaultAsync(x => x.Id == CurrentSession.CurrentInspectionId);
                if (inspection == null) return;

                var dialog = new SaveFileDialog
                {
                    Filter = "PDF File|*.pdf",
                    FileName = $"Inspection_{inspection.Id}.pdf"
                };

                if (dialog.ShowDialog() != true) return;

                ExportControlToPdf(PrintableArea, dialog.FileName);

                MessageBox.Show("گزارش با موفقیت ذخیره شد.", "عملیات موفق", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا در ایجاد فایل PDF", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportControlToPdf(FrameworkElement control, string destinationPath)
        {
            control.UpdateLayout();

            double width = control.ActualWidth > 0 ? control.ActualWidth : control.DesiredSize.Width;
            double height = control.ActualHeight > 0 ? control.ActualHeight : control.DesiredSize.Height;

            if (width == 0 || height == 0)
            {
                control.Measure(new Size(840, double.PositiveInfinity));
                control.Arrange(new Rect(new Point(), control.DesiredSize));
                control.UpdateLayout();
                width = control.DesiredSize.Width;
                height = control.DesiredSize.Height;
            }

            double dpi = 150;
            double scale = dpi / 96.0;

            var renderTarget = new RenderTargetBitmap(
                (int)(width * scale),
                (int)(height * scale),
                dpi, dpi, PixelFormats.Pbgra32);

            renderTarget.Render(control);

            var encoder = new JpegBitmapEncoder { QualityLevel = 90 };
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));

            using var memoryStream = new MemoryStream();
            encoder.Save(memoryStream);
            memoryStream.Position = 0;

            using var document = new PdfDocument();
            using var xImage = XImage.FromStream(memoryStream);

            var page = document.AddPage();
            double pageWidth = page.Width.Point;
            double pageHeight = page.Height.Point;

            // ۱. حاشیه را در PDF صفر کردیم (چون کنترل شما در XAML خودش Padding دارد)
            double margin = 0;

            double availableWidth = pageWidth - (2 * margin);
            double availableHeight = pageHeight - (2 * margin);

            double widthRatio = availableWidth / width;
            double heightRatio = availableHeight / height;
            double minRatio = Math.Min(widthRatio, heightRatio);

            double finalWidth = width * minRatio;
            double finalHeight = height * minRatio;

            // ۲. وسط‌چین افقی، ولی چسبیده به بالای صفحه (yPosition = 0)
            double xPosition = margin + (availableWidth - finalWidth) / 2;
            double yPosition = margin;

            using var gfx = XGraphics.FromPdfPage(page);

            gfx.Save();
            gfx.TranslateTransform(xPosition + finalWidth, yPosition);
            gfx.ScaleTransform(-1, 1);

            gfx.DrawImage(xImage, 0, 0, finalWidth, finalHeight);

            gfx.Restore();

            document.Save(destinationPath);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var wizard = Window.GetWindow(this) as InspectionWizardWindow;
            if (wizard == null) return;
            wizard.ShowStep4();
        }

        // عملکرد دکمه ثبت نهایی و خروج
        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("پرونده کارشناسی با موفقیت تایید نهایی شد و در سیستم آرشیو گردید.", "ثبت موفق", MessageBoxButton.OK, MessageBoxImage.Information);

            var wizardWindow = Window.GetWindow(this);
            wizardWindow?.Close(); // بستن کل پنجره ویزارد
        }
    }
}