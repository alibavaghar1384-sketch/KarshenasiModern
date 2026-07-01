using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using KarshenasiModern.Models;

namespace KarshenasiModern.Services.Pdf;

public class InspectionReportDocument : IDocument
{
    private readonly Inspection _inspection;

    public InspectionReportDocument(
        Inspection inspection)
    {
        _inspection = inspection;
    }

    public DocumentMetadata GetMetadata()
        => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);

            page.Margin(20);

            page.DefaultTextStyle(x =>
                x.FontSize(11));

            page.Header()
                .Text("گزارش کارشناسی خودرو")
                .Bold()
                .FontSize(20)
                .AlignCenter();

            page.Content()
                .Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Text($"مشتری: {_inspection.Car?.CustomerName}");

                    column.Item().Text($"تلفن: {_inspection.Car?.CustomerPhone}");

                    column.Item().Text($"پلاک: {_inspection.Car?.Plate}");

                    column.Item().Text($"شاسی: {_inspection.Car?.ChassisNumber}");

                    column.Item().Text($"برند: {_inspection.Car?.Brand}");

                    column.Item().Text($"مدل: {_inspection.Car?.Model}");

                    column.Item().PaddingTop(20);

                    column.Item().Text("وضعیت قطعات")
                        .Bold();

                    column.Item()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("قطعه");
                                header.Cell().Text("وضعیت");
                            });

                            foreach (var part in _inspection.BodyParts)
                            {
                                table.Cell().Text(part.PartName);

                                table.Cell().Text(
                                    GetStatus(part.Status));
                            }
                        });

                    column.Item()
                        .PaddingTop(15)
                        .Text("توضیحات")
                        .Bold();

                    column.Item()
                        .Text(_inspection.Description ?? "");
                });

            page.Footer()
                .AlignCenter()
                .Text(x =>
                {
                    x.Span("کارشناسی مدرن");
                });
        });
    }

    private string GetStatus(
        BodyPartStatus status)
    {
        return status switch
        {
            BodyPartStatus.Healthy => "سالم",
            BodyPartStatus.Painted => "رنگ شده",
            BodyPartStatus.Repaired => "تعمیر شده",
            BodyPartStatus.Replaced => "تعویض شده",
            BodyPartStatus.Damaged => "آسیب دیده",
            _ => "-"
        };
    }
}