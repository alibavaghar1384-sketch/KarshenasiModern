using System.Windows;
using KarshenasiModern.Models;
using System.IO;
using System.Windows.Media.Imaging;
using KarshenasiModern.Services;

namespace KarshenasiModern.Views;

public partial class InspectionReportWindow : Window
{
    private readonly Inspection _inspection;

    public InspectionReportWindow(
        Inspection inspection)
    {
        InitializeComponent();

        _inspection = inspection;

        LoadData();
    }

    private void LoadData()
    {
        if (_inspection.Car == null)
            return;

        DateText.Text =
            _inspection.CreatedAt.ToString();

        CustomerNameText.Text =
            $"مشتری: {_inspection.Car.CustomerName}";

        CustomerPhoneText.Text =
            $"تلفن: {_inspection.Car.CustomerPhone}";

        PlateText.Text =
            $"پلاک: {_inspection.Car.Plate}";

        BrandText.Text =
            $"برند: {_inspection.Car.Brand}";

        ModelText.Text =
            $"مدل: {_inspection.Car.Model}";

        ChassisText.Text =
            $"شاسی: {_inspection.Car.ChassisNumber}";

        DescriptionText.Text =
    _inspection.Description;

        BodyPartsGrid.ItemsSource =
            _inspection.BodyParts;

        foreach (var photo in _inspection.Photos)
        {
            var path =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Data",
                    "Images",
                    photo.EncryptedFileName);

            if (!File.Exists(path))
                continue;

            var bytes =
                ImageEncryptionService
                    .DecryptToBytes(path);

            var bitmap =
                CreateBitmap(bytes);

            switch (photo.Category)
            {
                case "Front":
                    FrontPhoto.Source =
                        bitmap;
                    break;

                case "Rear":
                    RearPhoto.Source =
                        bitmap;
                    break;

                case "Left":
                    LeftPhoto.Source =
                        bitmap;
                    break;

                case "Right":
                    RightPhoto.Source =
                        bitmap;
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