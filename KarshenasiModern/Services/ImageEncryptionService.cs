using System;
using System.IO;
using System.Security.Cryptography;

namespace KarshenasiModern.Services;

public static class ImageEncryptionService
{
    /// <summary>
    /// رمزنگاری فایل و ذخیره آن در مسیر مشخص شده (با استفاده از DPAPI ویندوز)
    /// </summary>
    public static void EncryptFile(string sourceFile, string encryptedFile)
    {
        var fileBytes = File.ReadAllBytes(sourceFile);

        // رمزنگاری در سطح سیستم‌عامل (فقط روی همین سیستم قابل رمزگشایی است)
        var encryptedBytes = ProtectedData.Protect(
            fileBytes,
            null,
            DataProtectionScope.LocalMachine);

        File.WriteAllBytes(encryptedFile, encryptedBytes);
    }

    /// <summary>
    /// رمزگشایی فایل و تبدیل آن به آرایه‌ای از بایت‌ها در حافظه
    /// </summary>
    public static byte[] DecryptToBytes(string encryptedFile)
    {
        var encryptedBytes = File.ReadAllBytes(encryptedFile);

        return ProtectedData.Unprotect(
            encryptedBytes,
            null,
            DataProtectionScope.LocalMachine);
    }

    /// <summary>
    /// رمزگشایی فایل و ذخیره مستقیم آن در یک فایل جدید
    /// </summary>
    public static void DecryptFile(string encryptedFile, string outputFile)
    {
        var bytes = DecryptToBytes(encryptedFile);
        File.WriteAllBytes(outputFile, bytes);
    }
}