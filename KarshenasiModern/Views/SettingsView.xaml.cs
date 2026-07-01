using System.Windows;
using System.Windows.Controls;
using KarshenasiModern.Models;
using KarshenasiModern.Services;

namespace KarshenasiModern.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();

        Loaded += SettingsView_Loaded;
        SaveButton.Click += SaveButton_Click;
    }

    private void SettingsView_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        var cameras =
            CameraService.GetCameras();

        FrontCameraBox.ItemsSource =
            cameras;

        RearCameraBox.ItemsSource =
            cameras;

        LeftCameraBox.ItemsSource =
            cameras;

        RightCameraBox.ItemsSource =
            cameras;

        var config =
            SettingsService.Load();

        FrontCameraBox.SelectedItem =
            config.FrontCamera;

        RearCameraBox.SelectedItem =
            config.RearCamera;

        LeftCameraBox.SelectedItem =
            config.LeftCamera;

        RightCameraBox.SelectedItem =
            config.RightCamera;
    }

    private void SaveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var config =
            new CameraConfiguration
            {
                FrontCamera =
                    FrontCameraBox.SelectedItem?.ToString() ?? "",

                RearCamera =
                    RearCameraBox.SelectedItem?.ToString() ?? "",

                LeftCamera =
                    LeftCameraBox.SelectedItem?.ToString() ?? "",

                RightCamera =
                    RightCameraBox.SelectedItem?.ToString() ?? ""
            };

        SettingsService.Save(config);

        MessageBox.Show(
            "تنظیمات ذخیره شد");
    }
}