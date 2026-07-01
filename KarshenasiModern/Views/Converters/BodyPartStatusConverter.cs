using System;
using System.Globalization;
using System.Windows.Data;
using KarshenasiModern.Models;

namespace KarshenasiModern.Converters
{
    public class BodyPartStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not BodyPartStatus status)
                return string.Empty;

            return status switch
            {
                BodyPartStatus.Healthy => "سالم",
                BodyPartStatus.Painted => "رنگ شدگی",
                BodyPartStatus.Replaced => "قطعه تعویضی",
                BodyPartStatus.TouchUp => "لیسه / آبرنگ",
                BodyPartStatus.Sunburned => "آفتاب سوختگی",
                BodyPartStatus.Damaged => "آسیب دیده / ضربه خورده",
                BodyPartStatus.Repaired => "تعمیر شده",
                BodyPartStatus.PDR => "صافکاری بی‌رنگ (PDR)",
                BodyPartStatus.Scratched => "خط و خش / غری",
                _ => string.Empty // تغییر یافت از ; به ,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string text)
                return BodyPartStatus.Healthy;

            return text.Trim() switch
            {
                "سالم" or "سالم (بدون رنگ)" => BodyPartStatus.Healthy,
                "رنگ شدگی" or "رنگ شده" or "رنگ" => BodyPartStatus.Painted,
                "قطعه تعویضی" or "تعویضی" or "تعویض شده" or "تعویض" => BodyPartStatus.Replaced,
                "لیسه / آبرنگ" or "لیسه" or "لکه گیری" or "لکه گیری / آبرنگ" => BodyPartStatus.TouchUp,
                "آفتاب سوختگی" => BodyPartStatus.Sunburned,
                "آسیب دیده / ضربه خورده" or "آسیب دیده" or "آسیب دیدگی" or "ضربه خورده" => BodyPartStatus.Damaged,
                "تعمیر شده" or "تعمیر" => BodyPartStatus.Repaired,
                "صافکاری بی‌رنگ (PDR)" or "صافکاری بی‌رنگ" => BodyPartStatus.PDR,
                "خط و خش / غری" or "خط و خش" => BodyPartStatus.Scratched,
                _ => BodyPartStatus.Healthy
            };
        }
    }
}