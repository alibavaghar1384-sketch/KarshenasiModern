namespace KarshenasiModern.Models;

public class Inspection
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }
        = DateTime.Now;

    public int CarId { get; set; }

    public Car? Car { get; set; }

    public string ExpertName { get; set; } = "";

    public string Description { get; set; } = "";

    public int Mileage { get; set; }

    public string EngineStatus { get; set; } = "";

    public string GearboxStatus { get; set; } = "";

    public string BrakeStatus { get; set; } = "";

    public string SuspensionStatus { get; set; } = "";

    public string TireStatus { get; set; } = "";

    public string ChassisStatus { get; set; } = "";

    public string AirbagStatus { get; set; } = "";

    public bool AirbagWarning { get; set; }

    public bool ChassisDamage { get; set; }

    public bool FloodDamage { get; set; }

    public ICollection<Photo> Photos { get; set; }
        = new List<Photo>();

    public ICollection<BodyPartInspection> BodyParts { get; set; }
        = new List<BodyPartInspection>();
}