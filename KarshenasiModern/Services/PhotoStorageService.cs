using System;
using System.IO;
using System.Threading.Tasks;
using KarshenasiModern.Database;
using KarshenasiModern.Models;

namespace KarshenasiModern.Services;

public static class PhotoStorageService
{
    public static async Task SaveEncryptedAsync(string tempFile, string category)
    {
        // ۱. بررسی وجود فایل موقت
        if (!File.Exists(tempFile))
        {
            throw new FileNotFoundException("فایل موقت یافت نشد.", tempFile);
        }

        // ۲. بررسی معتبر بودن شناسه بازرسی جاری (جلوگیری از خطای Foreign Key)
        if (CurrentSession.CurrentInspectionId <= 0)
        {
            throw new InvalidOperationException("خطا: شناسه بازرسی جاری معتبر نیست! مطمئن شوید که در مراحل قبل، اطلاعات بازرسی ابتدا در دیتابیس ذخیره شده است.");
        }

        // ۳. ایجاد نام فایل جدید و مسیرهای ذخیره‌سازی
        var fileName = $"{Guid.NewGuid():N}.dat";
        var targetFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Images");
        var targetFile = Path.Combine(targetFolder, fileName);

        // ۴. اطمینان از وجود پوشه مقصد
        Directory.CreateDirectory(targetFolder);

        // ۵. رمزنگاری و ذخیره فایل
        ImageEncryptionService.EncryptFile(tempFile, targetFile);

        // ۶. پاک کردن فایل موقت
        try
        {
            File.Delete(tempFile);
        }
        catch (IOException)
        {
            // در صورتی که فایل درگیر پردازش دیگری باشد، از خطا چشم‌پوشی می‌کنیم
        }

        // ۷. ثبت اطلاعات در دیتابیس
        using var db = new AppDbContext();

        // یک بررسی ثانویه جهت اطمینان از وجود این شناسه در جدول Inspections
        var inspectionExists = db.Inspections.Any(x => x.Id == CurrentSession.CurrentInspectionId);
        if (!inspectionExists)
        {
            throw new InvalidOperationException($"خطا: بازرسی با شناسه {CurrentSession.CurrentInspectionId} در دیتابیس یافت نشد.");
        }

        var photo = new Photo
        {
            EncryptedFileName = fileName,
            Category = category,
            InspectionId = CurrentSession.CurrentInspectionId,
            CreatedAt = DateTime.UtcNow
        };

        db.Photos.Add(photo);
        await db.SaveChangesAsync();
    }
}