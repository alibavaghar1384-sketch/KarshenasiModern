using KarshenasiModern.Database;
using KarshenasiModern.Models;
using KarshenasiModern.Services;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace KarshenasiModern.Views.Wizard
{
    public partial class Step3CameraView : UserControl
    {
        private readonly DispatcherTimer _timer = new();
        private VideoCapture? _capture;

        public Step3CameraView()
        {
            InitializeComponent();

            Loaded += Step3CameraView_Loaded;
            Unloaded += Step3CameraView_Unloaded;

            CaptureButton.Click += CaptureButton_Click;
            BackButton.Click += BackButton_Click;
            NextButton.Click += NextButton_Click;

            CameraCombo.SelectionChanged += CameraCombo_SelectionChanged;
        }

        private void Step3CameraView_Loaded(object sender, RoutedEventArgs e)
        {
            var cameras = CameraService.GetCameras();
            CameraCombo.ItemsSource = cameras;

            if (cameras.Count > 0)
            {
                CameraCombo.SelectedIndex = 0;
            }

            _timer.Interval = TimeSpan.FromMilliseconds(30);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            OpenSelectedCamera();
            RefreshPhotosList();
        }

        private void CameraCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenSelectedCamera();
        }

        private void OpenSelectedCamera()
        {
            _capture?.Release();
            _capture?.Dispose();

            var cameras = CameraService.GetCameras();

            if (CameraCombo.SelectedItem == null)
                return;

            var cameraName = CameraCombo.SelectedItem.ToString();
            var index = cameras.IndexOf(cameraName);

            if (index < 0)
                return;

            _capture = new VideoCapture(index);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_capture == null)
                return;

            using var frame = new Mat();
            _capture.Read(frame);

            if (frame.Empty())
                return;

            CameraImage.Source = frame.ToBitmapSource();
        }

        private async void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            if (_capture == null)
                return;

            if (CategoryCombo.SelectedItem == null)
            {
                MessageBox.Show("نوع عکس را انتخاب کنید");
                return;
            }

            using var frame = new Mat();
            _capture.Read(frame);

            if (frame.Empty())
                return;

            var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.jpg");
            Cv2.ImWrite(tempFile, frame);

            var category = ((ComboBoxItem)CategoryCombo.SelectedItem).Tag!.ToString()!;

            await PhotoStorageService.SaveEncryptedAsync(tempFile, category);
            RefreshPhotosList();

            MessageBox.Show("عکس ذخیره شد");
        }

        // متد حذف دقیق عکس انتخاب شده از لیست بر اساس شناسه دیتابیس
        private async void DeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is PhotoDisplayItem selectedItem)
            {
                var result = MessageBox.Show(
                    $"آیا از حذف این عکس ({selectedItem.CategoryPersian} - شماره {selectedItem.SequenceNumber}) مطمئن هستید؟",
                    "تایید حذف عکس",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using var db = new AppDbContext();

                        // پیدا کردن موجودیت عکس با استفاده از Id اختصاصی آن
                        var photoToDelete = db.Photos.FirstOrDefault(x => x.Id == selectedItem.PhotoId);

                        if (photoToDelete != null)
                        {
                            db.Photos.Remove(photoToDelete);
                            await db.SaveChangesAsync();

                            // به‌روزرسانی لیست پس از حذف
                            RefreshPhotosList();
                            MessageBox.Show("عکس مورد نظر با موفقیت حذف شد.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"خطا در حذف عکس: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // بارگذاری و نمایش عکس‌ها به صورت تکی همراه با شماره ردیف و عنوان فارسی
        private void RefreshPhotosList()
        {
            using var db = new AppDbContext();

            var rawPhotos = db.Photos
                .Where(x => x.InspectionId == CurrentSession.CurrentInspectionId)
                .OrderBy(x => x.Id)
                .ToList();

            // تبدیل اطلاعات خام به کلاس مدل مناسب برای نمایش در UI
            var displayData = rawPhotos.Select((photo, index) => new PhotoDisplayItem
            {
                PhotoId = photo.Id,
                SequenceNumber = index + 1,
                CategoryPersian = GetPersianCategory(photo.Category)
            }).ToList();

            PhotosList.ItemsSource = displayData;
        }

        // متد کمکی برای ترجمه نام انگلیسی دسته‌ها به زبان فارسی در خروجی لیست
        private string GetPersianCategory(string category)
        {
            return category switch
            {
                "Front" => "جلو",
                "Rear" => "عقب",
                "DriverSide" => "سمت راننده",
                "PassengerSide" => "سمت شاگرد",
                "Engine" => "موتور",
                "Interior" => "کابین",
                _ => category
            };
        }

        private void Step3CameraView_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _capture?.Release();
            _capture?.Dispose();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // رفع مشکل ابهام کلمه Window با اضافه کردن کامل نام فضای نام System.Windows
            var wizard = System.Windows.Window.GetWindow(this) as InspectionWizardWindow;
            if (wizard == null) return;

            wizard.ShowStep2();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // رفع مشکل ابهام کلمه Window با اضافه کردن کامل نام فضای نام System.Windows
            var wizard = System.Windows.Window.GetWindow(this) as InspectionWizardWindow;
            if (wizard == null) return;

            wizard.ShowStep4();
        }
    }

    // کلاس کمکی مستقل برای مدیریت و نمایش منظم داده‌ها در ListBox
    public class PhotoDisplayItem
    {
        public int PhotoId { get; set; }
        public int SequenceNumber { get; set; }
        public string CategoryPersian { get; set; } = string.Empty;
    }
}