using KarshenasiModern.Database;
using KarshenasiModern.Models;
using KarshenasiModern.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using System.Windows.Controls;

namespace KarshenasiModern.Views.Wizard
{
    public partial class Step2InspectionView : UserControl
    {
        public Step2InspectionView()
        {
            InitializeComponent();

            Loaded += Step2InspectionView_Loaded;
            BackButton.Click += BackButton_Click;
            NextButton.Click += NextButton_Click;
        }

        // بارگذاری اطلاعات قبلی در صورت وجود (برای زمانی که کاربر عقب-جلو می‌رود)
        private async void Step2InspectionView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using var db = new AppDbContext();
                var inspection = await db.Inspections
                    .FirstOrDefaultAsync(x => x.Id == CurrentSession.CurrentInspectionId);

                if (inspection == null) return;

                // بازیابی توضیحات کارشناس
                DescriptionTextBox.Text = inspection.Description ?? string.Empty;

                // بازیابی کارکرد
                MileageTextBox.Text = inspection.Mileage > 0 ? inspection.Mileage.ToString() : string.Empty;

                // بازیابی وضعیت کمبو باکس‌ها (بر اساس متن درون آن‌ها)
                SetComboValue(EngineStatusCombo, inspection.EngineStatus);
                SetComboValue(GearboxStatusCombo, inspection.GearboxStatus);
                SetComboValue(ChassisStatusCombo, inspection.ChassisStatus);
                SetComboValue(AirbagStatusCombo, inspection.AirbagStatus);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری اطلاعات مرحله قبل: {ex.Message}");
            }
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // ۱. اعتبارسنجی اولیه کارکرد خودرو
            int.TryParse(MileageTextBox.Text, out int mileage);

            try
            {
                // ۲. ذخیره اطلاعات در دیتابیس
                using var db = new AppDbContext();
                var inspection = await db.Inspections
                    .FirstOrDefaultAsync(x => x.Id == CurrentSession.CurrentInspectionId);

                if (inspection != null)
                {
                    // ذخیره متن کامپوننت توضیحات کارشناس (دقیقاً مشکلی که داشتی)
                    inspection.Description = DescriptionTextBox.Text?.Trim();

                    // ذخیره سایر فیلدها در جدول دیتابیس
                    inspection.Mileage = mileage;
                    inspection.EngineStatus = (EngineStatusCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    inspection.GearboxStatus = (GearboxStatusCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    inspection.ChassisStatus = (ChassisStatusCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    inspection.AirbagStatus = (AirbagStatusCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();

                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ذخیره‌سازی اطلاعات: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // اگر ذخیره نشد به صفحه بعد نرود
            }

            // ۳. هدایت به مرحله بعد (عکسبرداری)
            var wizard = Window.GetWindow(this) as InspectionWizardWindow;
            if (wizard == null) return;

            wizard.SetStep(3);
            wizard.WizardHost.Content = new Step3CameraView();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var wizard = Window.GetWindow(this) as InspectionWizardWindow;
            if (wizard == null) return;

            wizard.SetStep(1);
            wizard.WizardHost.Content = new Step1CustomerView();
        }

        // متد کمکی برای ست کردن مقدار کمبو باکس‌ها در هنگام لود مجدد صفحه
        private void SetComboValue(ComboBox combo, string? value)
        {
            if (string.IsNullOrEmpty(value)) return;
            foreach (ComboBoxItem item in combo.Items)
            {
                if (item.Content?.ToString() == value)
                {
                    combo.SelectedItem = item;
                    break;
                }
            }
        }
    }
}