using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KarshenasiModern.ViewModels;

public class NewInspectionViewModel : INotifyPropertyChanged
{
    private string _chassisNumber = "";

    public string ChassisNumber
    {
        get => _chassisNumber;
        set
        {
            _chassisNumber = value;
            OnPropertyChanged();
        }
    }

    private string _statusMessage = "";

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}