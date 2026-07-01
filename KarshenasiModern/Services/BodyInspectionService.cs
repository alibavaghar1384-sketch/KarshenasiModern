using KarshenasiModern.Database;
using KarshenasiModern.Models;

namespace KarshenasiModern.Services;

public static class BodyInspectionService
{
    public static void Save(
        int inspectionId,
        IEnumerable<BodyPartInspection> parts)
    {
        using var db =
            new AppDbContext();

        foreach (var part in parts)
        {
            part.InspectionId =
                inspectionId;

            db.BodyPartInspections
                .Add(part);
        }

        db.SaveChanges();
    }
}