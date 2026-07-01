using KarshenasiModern.Views;
using KarshenasiModern.Views.Home;
using KarshenasiModern.Views.Wizard;
using System.Windows;
using System.Windows.Controls;

namespace KarshenasiModern;

public partial class MainWindow : Window
{
    public ContentControl MainContentControl => MainContent;

    public MainWindow()
    {
        InitializeComponent();

        DashboardButton.Click += DashboardButton_Click;
        NewInspectionButton.Click += NewInspectionButton_Click;
        ArchiveButton.Click += ArchiveButton_Click;
        SettingsButton.Click += SettingsButton_Click;

        MainContent.Content =
            new HomeView();
    }

    private void DashboardButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MainContent.Content =
            new HomeView();
    }

    private void NewInspectionButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var wizard =
            new InspectionWizardWindow();

        wizard.ShowDialog();
    }

    private void ArchiveButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MainContent.Content =
            new SearchView();
    }

    private void SettingsButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        MainContent.Content =
            new SettingsView();
    }
}