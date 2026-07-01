namespace KarshenasiModern.Models;

public class Car
{
    public int Id { get; set; }

    public string ChassisNumber { get; set; } = "";

    public string Plate { get; set; } = "";

    public string Brand { get; set; } = "";

    public string Model { get; set; } = "";

    public string Color { get; set; } = "";

    public string CustomerName { get; set; } = "";

    public string CustomerPhone { get; set; } = "";

    public DateTime FirstVisitDate { get; set; } = DateTime.Now;

    public ICollection<Inspection> Inspections { get; set; }
        = new List<Inspection>();
}