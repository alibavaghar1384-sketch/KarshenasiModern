using KarshenasiModern.Database;
using KarshenasiModern.Models;
using KarshenasiModern.Services; // اضافه شده برای دسترسی به سشن جاری سیستم
using KarshenasiModern.Views.Wizard;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace KarshenasiModern.Views
{
    public partial class SearchView : UserControl
    {
        // متغیرهای مدیریت صفحه‌ب بندی کارشناسی‌ها
        private int _loadedCount = 0;
        private const int PageSize = 10;
        private readonly List<Inspection> _displayedInspections = new();

        public SearchView()
        {
            InitializeComponent();

            Loaded += SearchView_Loaded;
            SearchBox.TextChanged += SearchBox_TextChanged;
            SearchButton.Click += SearchButton_Click;
            InspectionsGrid.MouseDoubleClick += InspectionsGrid_MouseDoubleClick;

            // مدیریت تغییر سطر انتخاب شده برای به‌روزرسانی کارت اطلاعات بالایی
            InspectionsGrid.SelectionChanged += InspectionsGrid_SelectionChanged;
        }

        private async void SearchView_Loaded(object sender, RoutedEventArgs e)
        {
            await ResetAndLoadFirstPage();
        }

        // ریست کردن لیست و بارگذاری ۱۰ کارشناسی اول
        private async Task ResetAndLoadFirstPage()
        {
            _loadedCount = 0;
            _displayedInspections.Clear();
            LoadMoreButton.Visibility = Visibility.Visible;
            ClearDetailsCard();
            await LoadNextPage();
        }

        // بارگذاری تکه‌ای کارشناسی‌ها از دیتابیس به همراه اطلاعات خودرو و عکس‌ها
        private async Task LoadNextPage()
        {
            try
            {
                using var db = new AppDbContext();

                var nextInspections = await db.Inspections
                    .Include(x => x.Car)
                    .Include(x => x.Photos)
                    .OrderByDescending(x => x.Id)
                    .Skip(_loadedCount)
                    .Take(PageSize)
                    .ToListAsync();

                if (nextInspections.Any())
                {
                    _displayedInspections.AddRange(nextInspections);
                    _loadedCount += nextInspections.Count;

                    InspectionsGrid.ItemsSource = null;
                    InspectionsGrid.ItemsSource = _displayedInspections;
                }

                if (nextInspections.Count < PageSize)
                {
                    LoadMoreButton.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری اطلاعات: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadNextPage();
        }

        // موتور جستجوی هوشمند و چندگانه کارشناسی‌ها
        private async Task ExecuteSearch()
        {
            try
            {
                using var db = new AppDbContext();
                var text = SearchBox.Text.Trim();

                if (string.IsNullOrEmpty(text))
                {
                    await ResetAndLoadFirstPage();
                    return;
                }

                LoadMoreButton.Visibility = Visibility.Collapsed;
                ClearDetailsCard();

                // فیلتر همزمان روی نام مشتری، شماره شاسی و پلاک از طریق ارتباط جدول‌ها
                var filteredResults = await db.Inspections
                    .Include(x => x.Car)
                    .Include(x => x.Photos)
                    .Where(x => x.Car!.CustomerName.Contains(text) ||
                                x.Car.ChassisNumber.Contains(text) ||
                                x.Car.Plate.Contains(text))
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                InspectionsGrid.ItemsSource = null;
                InspectionsGrid.ItemsSource = filteredResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در اجرای جستجو: {ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await ExecuteSearch();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteSearch();
        }

        // بروزرسانی خودکار کارت جزئیات هنگام کلیک کاربر روی هر سطر جدول
        private void InspectionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InspectionsGrid.SelectedItem is Inspection inspection && inspection.Car != null)
            {
                CustomerText.Text = $"مشتری: {inspection.Car.CustomerName}";
                PhoneText.Text = $"تلفن: {inspection.Car.CustomerPhone}";
                PlateText.Text = $"پلاک: {inspection.Car.Plate}";
                BrandText.Text = $"برند: {inspection.Car.Brand}";
                ModelText.Text = $"مدل: {inspection.Car.Model}";
            }
            else
            {
                ClearDetailsCard();
            }
        }

        // متد کمکی برای پاکسازی کارت مشخصات خودرو
        private void ClearDetailsCard()
        {
            CustomerText.Text = "مشتری: -";
            PhoneText.Text = "تلفن: -";
            PlateText.Text = "پلاک: -";
            BrandText.Text = "برند: -";
            ModelText.Text = "مدل: -";
        }

        // تغییر یافته: دابل‌کلیک برای بارگذاری اطلاعات ذخیره شده در پنجره اصلی ویزارد کارشناسی
        private void InspectionsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (InspectionsGrid.SelectedItem is not Inspection inspection)
                return;

            // ۱. تنظیم آی‌دی کارشناسی انتخاب شده در سشن سراسری برنامه
            CurrentSession.CurrentInspectionId = inspection.Id;

            // ۲. باز کردن پنجره ویزارد کارشناسی (که حالا به لطف کدهای قبلی هوشمند شده و اطلاعات را لود می‌کند)
            var wizardWindow = new InspectionWizardWindow();

            // تنظیم پنجره والد برای مدیریت تمرکز و نمایش تمیزتر زوایای ویندوز
            var parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                wizardWindow.Owner = parentWindow;
            }

            wizardWindow.ShowDialog();

            // پس از بستن ویزارد، در صورت تمایل لیست جستجو را رفرش می‌کنیم تا تغییرات احتمالی اعمال شود
            _ = ResetAndLoadFirstPage();
        }
    }
}