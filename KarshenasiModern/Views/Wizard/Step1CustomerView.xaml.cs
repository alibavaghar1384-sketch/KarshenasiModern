using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore; // اضافه شده برای استفاده از Include و FirstOrDefaultAsync
using KarshenasiModern.Database;
using KarshenasiModern.Models;
using KarshenasiModern.Services;

namespace KarshenasiModern.Views.Wizard
{
    public partial class Step1CustomerView : UserControl
    {
        public Step1CustomerView()
        {
            InitializeComponent();

            // رویداد لود شدن صفحه برای بررسی حالت ویرایش
            Loaded += Step1CustomerView_Loaded;

            // ۱. محدودیت ورود فقط عدد
            PhoneTextBox.PreviewTextInput += OnlyNumeric_PreviewTextInput;
            PlatePart1TextBox.PreviewTextInput += OnlyNumeric_PreviewTextInput;
            PlatePart2TextBox.PreviewTextInput += OnlyNumeric_PreviewTextInput;
            PlateCityTextBox.PreviewTextInput += OnlyNumeric_PreviewTextInput;

            // ۲. محدودیت عدم ورود عدد در نام
            CustomerNameTextBox.PreviewTextInput += CustomerNameTextBox_PreviewTextInput;

            // ۳. رویدادهای انتقال خودکار فوکوس (Auto-Tabbing) پلاک
            PlatePart1TextBox.TextChanged += PlatePart1TextBox_TextChanged;
            PlateAlphaComboBox.SelectionChanged += PlateAlphaComboBox_SelectionChanged;
            PlatePart2TextBox.TextChanged += PlatePart2TextBox_TextChanged;
            PlateCityTextBox.TextChanged += PlateCityTextBox_TextChanged;

            NextButton.Click += NextButton_Click;
        }

        // بارگذاری اطلاعات قبلی در صورت وجود کارشناسی (حالت ویرایش یا بازگشت به مرحله قبل)
        private async void Step1CustomerView_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentSession.CurrentInspectionId > 0)
            {
                try
                {
                    using var db = new AppDbContext();
                    // بارگذاری اطلاعات کارشناسی به همراه اطلاعات خودرو مربوطه
                    var inspection = await db.Inspections
                        .Include(x => x.Car)
                        .FirstOrDefaultAsync(x => x.Id == CurrentSession.CurrentInspectionId);

                    if (inspection != null && inspection.Car != null)
                    {
                        var car = inspection.Car;
                        CustomerNameTextBox.Text = car.CustomerName ?? string.Empty;
                        PhoneTextBox.Text = car.CustomerPhone ?? string.Empty;
                        ChassisTextBox.Text = car.ChassisNumber ?? string.Empty;
                        BrandTextBox.Text = car.Brand ?? string.Empty;
                        ModelTextBox.Text = car.Model ?? string.Empty;
                        ColorTextBox.Text = car.Color ?? string.Empty;

                        // تفکیک و بازیابی اجزای پلاک (فرمت ذخیره شده: "22 ب 345 ایران 11")
                        if (!string.IsNullOrEmpty(car.Plate))
                        {
                            var parts = car.Plate.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 5)
                            {
                                PlatePart1TextBox.Text = parts[0];

                                string alpha = parts[1];
                                foreach (ComboBoxItem item in PlateAlphaComboBox.Items)
                                {
                                    if (item.Content?.ToString() == alpha)
                                    {
                                        PlateAlphaComboBox.SelectedItem = item;
                                        break;
                                    }
                                }

                                PlatePart2TextBox.Text = parts[2];
                                // parts[3] کلمه "ایران" است
                                PlateCityTextBox.Text = parts[4];
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در بارگذاری اطلاعات ویرایش: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // الف) وقتی ۲ رقم اول پلاک وارد شد -> باز شدن لیست حروف
        private void PlatePart1TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PlatePart1TextBox.Text.Length == 2)
            {
                PlateAlphaComboBox.Focus();
                PlateAlphaComboBox.IsDropDownOpen = true; // باز شدن خودکار لیست کشویی
            }
        }

        // ب) وقتی حرف پلاک انتخاب شد -> رفتن به ۳ رقم وسط
        private void PlateAlphaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlatePart2TextBox == null) return;
            PlatePart2TextBox.Focus();
        }

        // ج) وقتی ۳ رقم وسط وارد شد -> رفتن به ۲ رقم کد شهر
        private void PlatePart2TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PlatePart2TextBox.Text.Length == 3)
            {
                PlateCityTextBox.Focus();
            }
        }

        // د) وقتی ۲ رقم کد شهر وارد شد -> رفتن به تکست‌باکس برند خودرو
        private void PlateCityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PlateCityTextBox.Text.Length == 2)
            {
                BrandTextBox.Focus();
            }
        }

        // متد مشترک برای اعتبارسنجی ورود فقط اعداد
        private void OnlyNumeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }

        private void CustomerNameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[\d]");
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var wizard = Window.GetWindow(this) as InspectionWizardWindow;
            if (wizard == null) return;

            if (string.IsNullOrWhiteSpace(ModelTextBox.Text))
            {
                MessageBox.Show("لطفاً مدل خودرو را وارد کنید.");
                return;
            }

            string selectedAlpha = (PlateAlphaComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "ب";
            string combinedPlate = $"{PlatePart1TextBox.Text.Trim()} {selectedAlpha} {PlatePart2TextBox.Text.Trim()} ایران {PlateCityTextBox.Text.Trim()}";

            // به‌روزرسانی کش سشن جاری (ویزارد)
            WizardSession.CustomerName = CustomerNameTextBox.Text;
            WizardSession.Phone = PhoneTextBox.Text;
            WizardSession.Chassis = ChassisTextBox.Text;
            WizardSession.Plate = combinedPlate;
            WizardSession.Brand = BrandTextBox.Text;
            WizardSession.Model = ModelTextBox.Text;
            WizardSession.Color = ColorTextBox.Text;

            try
            {
                using var db = new AppDbContext();

                if (CurrentSession.CurrentInspectionId > 0)
                {
                    // حالت اول: ویرایش کارشناسی موجود
                    var inspection = await db.Inspections
                        .Include(x => x.Car)
                        .FirstOrDefaultAsync(x => x.Id == CurrentSession.CurrentInspectionId);

                    if (inspection != null && inspection.Car != null)
                    {
                        var car = inspection.Car;
                        car.Brand = BrandTextBox.Text;
                        car.Model = ModelTextBox.Text;
                        car.Color = ColorTextBox.Text;
                        car.Plate = combinedPlate;
                        car.ChassisNumber = ChassisTextBox.Text;
                        car.CustomerName = CustomerNameTextBox.Text;
                        car.CustomerPhone = PhoneTextBox.Text;

                        await db.SaveChangesAsync();
                        wizard.ShowStep2();
                    }
                    else
                    {
                        MessageBox.Show("اطلاعات کارشناسی جهت ویرایش یافت نشد.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    // حالت دوم: ثبت جدید کارشناسی و خودرو
                    var newCar = new Car
                    {
                        Brand = BrandTextBox.Text,
                        Model = ModelTextBox.Text,
                        Color = ColorTextBox.Text,
                        Plate = combinedPlate,
                        ChassisNumber = ChassisTextBox.Text,
                        CustomerName = CustomerNameTextBox.Text,
                        CustomerPhone = PhoneTextBox.Text,
                        FirstVisitDate = DateTime.Now
                    };

                    db.Cars.Add(newCar);
                    await db.SaveChangesAsync();

                    var newInspection = new Inspection
                    {
                        CarId = newCar.Id,
                        CreatedAt = DateTime.Now
                    };

                    db.Inspections.Add(newInspection);
                    await db.SaveChangesAsync();

                    CurrentSession.CurrentInspectionId = newInspection.Id;
                    wizard.ShowStep2();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ثبت و ذخیره‌سازی اطلاعات: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}