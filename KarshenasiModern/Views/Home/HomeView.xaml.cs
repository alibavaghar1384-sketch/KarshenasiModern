using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KarshenasiModern.Views;
using KarshenasiModern.Views.Wizard;
using KarshenasiModern.Database;
using KarshenasiModern.Services;
using Microsoft.EntityFrameworkCore;

namespace KarshenasiModern.Views.Home
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();

            NewInspectionButton.Click += NewInspectionButton_Click;
            ArchiveButton.Click += ArchiveButton_Click;
            EditButton.Click += EditButton_Click;
            Loaded += HomeView_Loaded;
        }

        private void NewInspectionButton_Click(object sender, RoutedEventArgs e)
        {
            // سشن را صفر می‌کنیم تا اطلاعات کارشناسی‌های قبلی پاک شود
            CurrentSession.CurrentInspectionId = 0;

            // باز کردن پنجره ویزارد کارشناسی جدید
            var wizard = new InspectionWizardWindow();
            wizard.ShowDialog();
        }

        private void ArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainContentControl.Content = new SearchView();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("بخش ویرایش کارشناسی در مراحل بعدی تکمیل می‌شود.");
        }

        private async void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using var db = new AppDbContext();
                var today = DateTime.Today;

                // دریافت آمار روزانه و کل کارشناسی‌ها از دیتابیس
                var todayCount = await db.Inspections.CountAsync(x => x.CreatedAt.Date == today);
                var totalCars = await db.Cars.CountAsync();

                TodayCountText.Text = todayCount.ToString();
                TotalCarsText.Text = totalCars.ToString();

                var lastInspection = await db.Inspections
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                if (lastInspection != null)
                {
                    LastInspectionText.Text = lastInspection.CreatedAt.ToString("yyyy/MM/dd");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database error on Home load: {ex.Message}");
            }
        }
    }
}