using KarshenasiModern.Services;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks; // این فضای نام برای Task اضافه شد

namespace KarshenasiModern.Views;

public partial class CameraView : UserControl
{
    private readonly DispatcherTimer _timer = new();

    private VideoCapture? _frontCapture;
    private VideoCapture? _rearCapture;
    private VideoCapture? _leftCapture;
    private VideoCapture? _rightCapture;

    public CameraView()
    {
        InitializeComponent();

        Loaded += CameraView_Loaded;
        Unloaded += CameraView_Unloaded;
        CaptureButton.Click += CaptureButton_Click;
    }

    private void CameraView_Loaded(object sender, RoutedEventArgs e)
    {
        var cameras = CameraService.GetCameras();
        var config = SettingsService.Load();

        _frontCapture = OpenCamera(cameras, config.FrontCamera);
        _rearCapture = OpenCamera(cameras, config.RearCamera);
        _leftCapture = OpenCamera(cameras, config.LeftCamera);
        _rightCapture = OpenCamera(cameras, config.RightCamera);

        _timer.Interval = TimeSpan.FromMilliseconds(30);
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private VideoCapture? OpenCamera(List<string> cameras, string cameraName)
    {
        if (string.IsNullOrWhiteSpace(cameraName))
            return null;

        var index = cameras.IndexOf(cameraName);

        if (index < 0)
            return null;

        return new VideoCapture(index);
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        UpdateCamera(_frontCapture, FrontImage);
        UpdateCamera(_rearCapture, RearImage);
        UpdateCamera(_leftCapture, LeftImage);
        UpdateCamera(_rightCapture, RightImage);
    }

    private void UpdateCamera(VideoCapture? capture, Image image)
    {
        if (capture == null)
            return;

        using var frame = new Mat();
        capture.Read(frame);

        if (frame.Empty())
            return;

        image.Source = frame.ToBitmapSource();
    }

    private void CameraView_Unloaded(object sender, RoutedEventArgs e)
    {
        _timer.Stop();

        _frontCapture?.Release();
        _rearCapture?.Release();
        _leftCapture?.Release();
        _rightCapture?.Release();

        _frontCapture?.Dispose();
        _rearCapture?.Dispose();
        _leftCapture?.Dispose();
        _rightCapture?.Dispose();
    }

    // متد کلیک به async تبدیل شد
    private async void CaptureButton_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentSession.CurrentInspectionId <= 0)
        {
            MessageBox.Show("ابتدا خودرو را ثبت کنید");
            return;
        }

        // برای هر دوربین از await استفاده می‌کنیم تا ذخیره‌سازی به درستی انجام شود
        await CaptureCamera(_frontCapture, "Front");
        await CaptureCamera(_rearCapture, "Rear");
        await CaptureCamera(_leftCapture, "Left");
        await CaptureCamera(_rightCapture, "Right");

        MessageBox.Show("تصاویر با موفقیت ذخیره شدند");
    }

    // خروجی متد به Task تغییر کرد و async شد
    private async Task CaptureCamera(VideoCapture? capture, string Category)
    {
        if (capture == null)
            return;

        using var frame = new Mat();
        capture.Read(frame);

        if (frame.Empty())
            return;

        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.jpg");

        Cv2.ImWrite(tempFile, frame);

        // فراخوانی متد جدید به صورت ناهمگام
        await PhotoStorageService.SaveEncryptedAsync(tempFile, Category);
    }
}