namespace KarshenasiModern.Models;

public class Photo
{
    public int Id { get; set; }

    public string EncryptedFileName { get; set; } = "";

    public string Category { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int InspectionId { get; set; }

    public Inspection? Inspection { get; set; }
}