using KarshenasiModern.Database;
using Microsoft.EntityFrameworkCore; // برای متدهای Async دیتابیس
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KarshenasiModern.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            this.Loaded += DashboardView_Loaded;
        }

        private async void DashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using var db = new AppDbContext();

                var todayCount = await db.Inspections.CountAsync(
                    x => x.CreatedAt.Date == DateTime.Today);
                TodayCountText.Text = todayCount.ToString();

                var monthCount = await db.Inspections.CountAsync(
                    x => x.CreatedAt.Month == DateTime.Now.Month &&
                         x.CreatedAt.Year == DateTime.Now.Year);
                MonthCountText.Text = monthCount.ToString();

                var carsCount = await db.Cars.CountAsync();
                CarsCountText.Text = carsCount.ToString();

                var photosCount = await db.Photos.CountAsync();
                PhotosCountText.Text = photosCount.ToString();
            }
            catch (Exception ex)
            {
                // اصلاح اصلی اینجاست: استفاده از MessageBoxImage به جای MessageBoxIcon
                MessageBox.Show($"خطا در بارگذاری اطلاعات دیتابیس: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}