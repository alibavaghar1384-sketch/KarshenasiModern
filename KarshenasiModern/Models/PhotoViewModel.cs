using System.Windows.Media.Imaging;

namespace KarshenasiModern.Models
{
    public class PhotoViewModel
    {
        // دادن مقدار پیش‌فرض برای جلوگیری از اخطار Nullable
        public string Title { get; set; } = string.Empty;
        public BitmapImage? ImageSource { get; set; }
    }
}