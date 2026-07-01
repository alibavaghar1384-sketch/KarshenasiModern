using System;
using System.IO;
using HelixToolkit.SharpDX.Assimp; // نیاز به این فضای نام برای HelixToolkitScene داریم

namespace KarshenasiModern.Services
{
    public static class ModelCacheManager
    {
        // نوع متغیر را به HelixToolkitScene تغییر دادیم تا با خروجی امپورتر همخوانی داشته باشد
        private static HelixToolkitScene _cachedCarModel;
        private static readonly object _lock = new object();

        public static HelixToolkitScene GetCarModel()
        {
            // اگر مدل قبلاً لود شده است، همان را بدون پردازش دوباره برگردان
            if (_cachedCarModel != null)
            {
                return _cachedCarModel;
            }

            lock (_lock)
            {
                if (_cachedCarModel == null)
                {
                    try
                    {
                        string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "3DModels", "Audi.fbx");

                        if (File.Exists(modelPath))
                        {
                            var importer = new Importer();

                            // خروجی این متد مستقیم در متغیر کش قرار می‌گیرد و دیگر خطای تایپ نمی‌دهد
                            _cachedCarModel = importer.Load(modelPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error Caching Model: {ex.Message}");
                    }
                }
            }

            return _cachedCarModel;
        }
    }
}