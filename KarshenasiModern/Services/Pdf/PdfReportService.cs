using KarshenasiModern.Database;
using KarshenasiModern.Reports;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace KarshenasiModern.Services;

public static class PdfReportService
{
    public static async Task GenerateAsync(
        int inspectionId,
        string filePath)
    {
        using var db = new AppDbContext();

        var inspection = await db.Inspections
            .Include(x => x.Car)
            .Include(x => x.BodyParts)
            .Include(x => x.Photos)
            .FirstOrDefaultAsync(x => x.Id == inspectionId);

        if (inspection == null)
            throw new Exception("Inspection not found");

        var document =
            new InspectionReportDocument(inspection);

        document.GeneratePdf(filePath);
    }
}