using KarshenasiModern.Database;
using QuestPDF.Infrastructure;
using System.IO;
using System.Windows;

namespace KarshenasiModern;

public partial class App : Application
{
    protected override void OnStartup(
        StartupEventArgs e)
    {

        QuestPDF.Settings.License = LicenseType.Community;
        
        base.OnStartup(e);  

        Directory.CreateDirectory(
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "Images"));

        using (var db = new AppDbContext())
        {
            db.Database.EnsureCreated();
        }

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}