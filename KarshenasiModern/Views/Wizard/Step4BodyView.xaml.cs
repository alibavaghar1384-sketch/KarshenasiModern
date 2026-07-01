using KarshenasiModern.Database;
using KarshenasiModern.Models;
using KarshenasiModern.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KarshenasiModern.Views.Wizard
{
    public partial class Step4BodyView : UserControl
    {
        private List<BodyPartItem> _items = new List<BodyPartItem>();
        private BodyPartItem _currentEditingPart;
        private Button _currentClickedButton;

        public Step4BodyView()
        {
            InitializeComponent();
            Loaded += Step4BodyView_Loaded;
            BackButton.Click += BackButton_Click;
            NextButton.Click += NextButton_Click;
        }

        private async void Step4BodyView_Loaded(object sender, RoutedEventArgs e)
        {
            var statuses = new List<StatusItem>
            {
                new StatusItem { Title = "سالم", Value = BodyPartStatus.Healthy },
                new StatusItem { Title = "رنگ شدگی", Value = BodyPartStatus.Painted },
                new StatusItem { Title = "قطعه تعویضی", Value = BodyPartStatus.Replaced },
                new StatusItem { Title = "لیسه / آبرنگ", Value = BodyPartStatus.TouchUp },
                new StatusItem { Title = "آفتاب سوختگی", Value = BodyPartStatus.Sunburned },
                new StatusItem { Title = "آسیب دیده / جوش", Value = BodyPartStatus.Damaged },
                new StatusItem { Title = "تعمیر شده", Value = BodyPartStatus.Repaired },
                new StatusItem { Title = "صافکاری بی رنگ", Value = BodyPartStatus.PDR },
                new StatusItem { Title = "خط و خش", Value = BodyPartStatus.Scratched }
            };

            PopupStatusCombo.ItemsSource = statuses;

            try
            {
                using (var db = new AppDbContext())
                {
                    int inspectionId = (int)CurrentSession.CurrentInspectionId;

                    var savedParts = await db.BodyPartInspections
                        .Where(x => x.InspectionId == inspectionId)
                        .ToListAsync();

                    var allPartNames = new List<string>
                    {
                        "سپر جلو", "کاپوت", "سقف", "صندوق عقب", "سپر عقب",
                        "درب جلو راننده", "درب جلو شاگرد", "درب عقب راننده", "درب عقب شاگرد",
                        "گلگیر جلو راننده", "گلگیر جلو شاگرد", "گلگیر عقب راننده", "گلگیر عقب شاگرد",
                        "رکاب راننده", "رکاب شاگرد", "شاسی جلو راننده", "شاسی جلو شاگرد",
                        "شاسی عقب راننده", "شاسی عقب شاگرد", "سینی جلو", "سینی عقب"
                    };

                    _items.Clear();
                    foreach (var name in allPartNames)
                    {
                        var saved = savedParts.FirstOrDefault(x => x.PartName == name);
                        _items.Add(new BodyPartItem
                        {
                            PartName = name,
                            Status = saved?.Status ?? BodyPartStatus.Healthy,
                            PaintThickness = saved?.PaintThickness
                        });
                    }

                    // اعمال وضعیت‌ها روی کنترل گرافیکی
                    foreach (var item in _items)
                    {
                        CarDiagram.SetPartStatus(item.PartName, item.Status);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری اطلاعات بدنه: {ex.Message}");
            }
        }

        private void PartButton_Click(object sender, RoutedEventArgs e)
        {
            _currentClickedButton = sender as Button;
            if (_currentClickedButton == null) return;

            string partName = _currentClickedButton.Content.ToString();
            _currentEditingPart = _items.FirstOrDefault(x => x.PartName == partName);

            if (_currentEditingPart == null) return;

            PopupPartNameText.Text = _currentEditingPart.PartName;
            PopupStatusCombo.SelectedValue = _currentEditingPart.Status;
            PopupThicknessText.Text = _currentEditingPart.PaintThickness?.ToString() ?? "";

            PartActionPopup.IsOpen = true;
        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            PartActionPopup.IsOpen = false;
        }

        private void SavePart_Click(object sender, RoutedEventArgs e)
        {
            if (_currentEditingPart == null) return;

            if (PopupStatusCombo.SelectedValue is BodyPartStatus selectedStatus)
            {
                _currentEditingPart.Status = selectedStatus;
            }

            if (int.TryParse(PopupThicknessText.Text, out int thickness))
            {
                _currentEditingPart.PaintThickness = thickness;
            }
            else
            {
                _currentEditingPart.PaintThickness = null;
            }

            // رنگ آمیزی مجدد قطعه کارشناسی شده روی نمودار
            CarDiagram.SetPartStatus(_currentEditingPart.PartName, _currentEditingPart.Status);

            PartActionPopup.IsOpen = false;
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            NextButton.IsEnabled = false;
            try
            {
                using (var db = new AppDbContext())
                {
                    int inspectionId = (int)CurrentSession.CurrentInspectionId;

                    var oldRecords = await db.BodyPartInspections
                        .Where(x => x.InspectionId == inspectionId)
                        .ToListAsync();

                    if (oldRecords.Any()) db.BodyPartInspections.RemoveRange(oldRecords);

                    foreach (var item in _items)
                    {
                        db.BodyPartInspections.Add(new BodyPartInspection
                        {
                            InspectionId = inspectionId,
                            PartName = item.PartName,
                            Status = item.Status,
                            PaintThickness = item.PaintThickness
                        });
                    }

                    await db.SaveChangesAsync();

                    var wizard = Window.GetWindow(this) as InspectionWizardWindow;
                    wizard?.ShowStep5();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در ذخیره‌سازی اطلاعات بدنه:\n{ex.Message}", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                NextButton.IsEnabled = true;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var wizard = Window.GetWindow(this) as InspectionWizardWindow;
            wizard?.ShowStep3();
        }
    }

    public class BodyPartItem
    {
        public string PartName { get; set; }
        public BodyPartStatus Status { get; set; }
        public int? PaintThickness { get; set; }
    }

    public class StatusItem
    {
        public string Title { get; set; }
        public BodyPartStatus Value { get; set; }
    }
}