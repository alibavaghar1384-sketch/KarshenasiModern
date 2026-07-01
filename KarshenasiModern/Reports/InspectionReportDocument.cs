using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using KarshenasiModern.Models;

namespace KarshenasiModern.Reports;

public class InspectionReportDocument : IDocument
{
    private readonly Inspection _inspection;

    public InspectionReportDocument(Inspection inspection)
    {
        _inspection = inspection;
    }

    public DocumentMetadata GetMetadata()
        => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(20);

            page.Header().Text("گزارش کارشناسی خودرو")
                .FontSize(22)
                .Bold()
                .AlignCenter();

            page.Content().Column(col =>
            {
                col.Spacing(10);

                col.Item().Border(1).Padding(10).Column(info =>
                {
                    info.Item().Text($"نام مشتری: {_inspection.Car?.CustomerName}");
                    info.Item().Text($"تلفن: {_inspection.Car?.CustomerPhone}");
                    info.Item().Text($"پلاک: {_inspection.Car?.Plate}");
                    info.Item().Text($"شاسی: {_inspection.Car?.ChassisNumber}");
                    info.Item().Text($"کارشناس: {_inspection.ExpertName}");
                });

                col.Item().Text("وضعیت قطعات")
                    .FontSize(16)
                    .Bold();

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("قطعه");
                        header.Cell().Text("وضعیت");
                        header.Cell().Text("ضخامت رنگ");
                    });

                    foreach (var part in _inspection.BodyParts)
                    {
                        table.Cell().Text(part.PartName);

                        table.Cell().Text(
                            part.Status switch
                            {
                                BodyPartStatus.Healthy => "سالم",
                                BodyPartStatus.Painted => "رنگ شده",
                                BodyPartStatus.Repaired => "تعمیر شده",
                                BodyPartStatus.Replaced => "تعویض شده",
                                BodyPartStatus.Damaged => "آسیب دیده",
                                _ => "-"
                            });

                        table.Cell().Text(
                            part.PaintThickness?.ToString() ?? "-");
                    }
                });

                col.Item().PaddingTop(20);

                col.Item().Text("توضیحات")
                    .Bold();

                col.Item().Text(
                    string.IsNullOrWhiteSpace(_inspection.Description)
                        ? "-"
                        : _inspection.Description);
            });

            page.Footer()
                .AlignCenter()
                .Text(x =>
                {
                    x.Span("Karshenasi Modern");
                });
        });
    }
}