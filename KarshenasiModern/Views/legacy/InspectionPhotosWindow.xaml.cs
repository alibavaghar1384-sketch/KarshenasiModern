using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using KarshenasiModern.Models;
using KarshenasiModern.Services;

namespace KarshenasiModern.Views;

public partial class InspectionPhotosWindow : Window
{
    public InspectionPhotosWindow(
        Inspection inspection)
    {
        InitializeComponent();

        LoadImages(inspection);
    }

    private void LoadImages(
        Inspection inspection)
    {
        foreach (var photo in inspection.Photos)
        {
            var file =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Data",
                    "Images",
                    photo.EncryptedFileName);

            if (!File.Exists(file))
                continue;

            var bytes =
                ImageEncryptionService
                .DecryptToBytes(file);

            var image =
                CreateBitmap(bytes);

            switch (photo.Category)
            {
                case "Front":
                    FrontImage.Source = image;
                    break;

                case "Rear":
                    RearImage.Source = image;
                    break;

                case "Left":
                    LeftImage.Source = image;
                    break;

                case "Right":
                    RightImage.Source = image;
                    break;
            }
        }
    }

    private BitmapImage CreateBitmap(
        byte[] bytes)
    {
        using var stream =
            new MemoryStream(bytes);

        var image =
            new BitmapImage();

        image.BeginInit();
        image.CacheOption =
            BitmapCacheOption.OnLoad;
        image.StreamSource =
            stream;
        image.EndInit();
        image.Freeze();

        return image;
    }
}