using System.Collections.ObjectModel;
using System.Windows.Controls;
using KarshenasiModern.Models;

namespace KarshenasiModern.Views;

public partial class BodyInspectionView : UserControl
{
    public ObservableCollection<
        BodyPartInspection> Parts
    { get; set; }
        = new();

    public BodyInspectionView()
    {
        InitializeComponent();

        BodyGrid.ItemsSource = Parts;

        LoadParts();
    }

    private void LoadParts()
    {
        string[] names =
        {
            "سقف",
            "کاپوت",
            "گلگیر جلو راست",
            "گلگیر جلو چپ",
            "در جلو راست",
            "در جلو چپ",
            "در عقب راست",
            "در عقب چپ",
            "گلگیر عقب راست",
            "گلگیر عقب چپ",
            "صندوق",
            "سپر جلو",
            "سپر عقب"
        };

        foreach (var item in names)
        {
            Parts.Add(
                new BodyPartInspection
                {
                    PartName = item
                });
        }
    }
    public IEnumerable<BodyPartInspection>
    GetData()
    {
        return Parts;
    }
}