using System.IO;
using System.Text.Json;
using KarshenasiModern.Models;

namespace KarshenasiModern.Services;

public static class SettingsService
{
    private static readonly string FilePath =
        Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Settings.json");

    public static CameraConfiguration Load()
    {
        if (!File.Exists(FilePath))
            return new CameraConfiguration();

        var json =
            File.ReadAllText(FilePath);

        return JsonSerializer.Deserialize<CameraConfiguration>(json)
               ?? new CameraConfiguration();
    }

    public static void Save(
        CameraConfiguration config)
    {
        var json =
            JsonSerializer.Serialize(
                config,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        File.WriteAllText(
            FilePath,
            json);
    }
}