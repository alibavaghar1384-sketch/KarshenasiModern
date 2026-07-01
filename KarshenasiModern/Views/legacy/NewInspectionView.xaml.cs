using System.Windows.Controls;
using KarshenasiModern.Services;
using KarshenasiModern.Database;
using KarshenasiModern.Models;
using System.Windows;   
namespace KarshenasiModern.Views;

public partial class NewInspectionView : UserControl
{
    private readonly CarService _carService = new();

    public NewInspectionView()
    {
        InitializeComponent();

        ChassisTextBox.TextChanged += ChassisTextBox_TextChanged;
        SaveButton.Click += SaveButton_Click;
    }

    private async void ChassisTextBox_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        var chassis = ChassisTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(chassis))
        {
            StatusText.Text = "شماره شاسی وارد نشده";
            return;
        }

        var exists =
            await _carService.ExistsAsync(chassis);

        if (exists)
        {
            StatusText.Text =
                "⚠ خودرو قبلاً ثبت شده است";
        }
        else
        {
            StatusText.Text =
                "✅ خودرو جدید است";
        }
    }

    private async void SaveButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        var chassis =
            ChassisTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(chassis))
        {
            MessageBox.Show(
                "شماره شاسی وارد نشده است");

            return;
        }

        var exists =
            await _carService.ExistsAsync(chassis);

        if (exists)
        {
            MessageBox.Show(
                "این خودرو قبلاً ثبت شده است");

            return;
        }

        using var db = new AppDbContext();

        var car = new Car
        {
            CustomerName = CustomerNameTextBox.Text,
            CustomerPhone = PhoneTextBox.Text,

            ChassisNumber = chassis,
            Plate = PlateTextBox.Text,
            Brand = BrandTextBox.Text,
            Model = ModelTextBox.Text,
            Color = ColorTextBox.Text,
            FirstVisitDate = DateTime.Now
        };

        db.Cars.Add(car);

        await db.SaveChangesAsync();

        var inspection = new Inspection
        {
            CarId = car.Id,
            CreatedAt =
    InspectionDatePicker.SelectedDate
    ?? DateTime.Now,

            Description =
        DescriptionTextBox.Text
        };

        db.Inspections.Add(inspection);

        await db.SaveChangesAsync();

        CurrentSession.CurrentInspectionId =
    inspection.Id;


        var bodyParts =
    BodyInspectionControl.GetData();

        BodyInspectionService.Save(
            inspection.Id,
            bodyParts);

        MessageBox.Show(
            "خودرو با موفقیت ثبت شد");

        ChassisTextBox.Clear();
        PlateTextBox.Clear();
        BrandTextBox.Clear();
        ModelTextBox.Clear();
        ColorTextBox.Clear();
    }
}