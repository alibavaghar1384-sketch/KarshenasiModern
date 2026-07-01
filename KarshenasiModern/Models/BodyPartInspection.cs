using KarshenasiModern.Models;

public class BodyPartInspection
{
    public int Id { get; set; }

    public int InspectionId { get; set; }

    public Inspection? Inspection { get; set; }

    public string PartName { get; set; } = "";

    public BodyPartStatus Status { get; set; }

    public int? PaintThickness { get; set; }
}