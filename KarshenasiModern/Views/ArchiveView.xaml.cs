using System.Windows.Controls;
using KarshenasiModern.Database;
using Microsoft.EntityFrameworkCore;

namespace KarshenasiModern.Views;

public partial class ArchiveView : UserControl
{
    public ArchiveView()
    {
        InitializeComponent();

        Loaded += ArchiveView_Loaded;
        SearchBox.TextChanged += SearchBox_TextChanged;
    }

    private async void ArchiveView_Loaded(
        object sender,
        System.Windows.RoutedEventArgs e)
    {
        await LoadCars();
    }

    private async Task LoadCars()
    {
        using var db = new AppDbContext();

        CarsGrid.ItemsSource =
            await db.Cars
                .OrderByDescending(x => x.Id)
                .ToListAsync();
    }

    private async void SearchBox_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        using var db = new AppDbContext();

        var text = SearchBox.Text.Trim();

        CarsGrid.ItemsSource =
            await db.Cars
                .Where(x =>
                    x.ChassisNumber.Contains(text) ||
                    x.Plate.Contains(text) ||
                    x.CustomerName.Contains(text))
                .ToListAsync();
    }
}