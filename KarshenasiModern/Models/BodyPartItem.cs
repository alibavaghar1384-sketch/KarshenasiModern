using KarshenasiModern.Models;

namespace KarshenasiModern.Models;

public class BodyPartItem
{
    public string PartName { get; set; } = "";

    public BodyPartStatus Status { get; set; }
        = BodyPartStatus.Healthy;

    public int? PaintThickness { get; set; }
}