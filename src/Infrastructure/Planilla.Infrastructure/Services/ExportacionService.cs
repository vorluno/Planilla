// ====================================================================
// Planilla - ExportacionService
// Creado: 2025-12-28
// Descripción: Servicio para exportar reportes a Excel y PDF
// Usa ClosedXML para Excel y QuestPDF para PDF
// ====================================================================

using ClosedXML.Excel;
using Vorluno.Planilla.Application.DTOs.Reportes;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Vorluno.Planilla.Infrastructure.Services;

/// <summary>
/// Servicio para exportar reportes a diferentes formatos (Excel, PDF)
/// </summary>
public class ExportacionService
{
    public ExportacionService()
    {
        // Configurar licencia de QuestPDF (Community es gratuita)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    #region Excel - CSS

    /// <summary>
    /// Exporta el reporte de CSS a formato Excel
    /// </summary>
    public byte[] ExportarExcelCss(ReporteCssDto reporte)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Planilla CSS");

        // Encabezado
        worksheet.Cell("A1").Value = reporte.NombreEmpresa;
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 14;
        worksheet.Range("A1:H1").Merge();

        worksheet.Cell("A2").Value = $"RUC: {reporte.Ruc}";
        worksheet.Cell("A3").Value = $"Período: {reporte.Periodo}";
        worksheet.Cell("A4").Value = "REPORTE PLANILLA CSS";
        worksheet.Cell("A4").Style.Font.Bold = true;
        worksheet.Cell("A5").Value = $"Generado: {reporte.FechaGeneracion:dd/MM/yyyy HH:mm}";

        // Headers de tabla
        var headerRow = 7;
        worksheet.Cell(headerRow, 1).Value = "Cédula";
        worksheet.Cell(headerRow, 2).Value = "Nombre";
        worksheet.Cell(headerRow, 3).Value = "Salario Bruto";
        worksheet.Cell(headerRow, 4).Value = "Base CSS";
        worksheet.Cell(headerRow, 5).Value = "CSS Empleado";
        worksheet.Cell(headerRow, 6).Value = "CSS Patrono";
        worksheet.Cell(headerRow, 7).Value = "Riesgo Prof.";
        worksheet.Cell(headerRow, 8).Value = "Total CSS";

        var headerRange = worksheet.Range(headerRow, 1, headerRow, 8);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Datos
        var row = headerRow + 1;
        foreach (var emp in reporte.Empleados)
        {
            worksheet.Cell(row, 1).Value = emp.Cedula;
            worksheet.Cell(row, 2).Value = emp.NombreCompleto;
            worksheet.Cell(row, 3).Value = emp.SalarioBruto;
            worksheet.Cell(row, 4).Value = emp.BaseCss;
            worksheet.Cell(row, 5).Value = emp.CssEmpleado;
            worksheet.Cell(row, 6).Value = emp.CssPatrono;
            worksheet.Cell(row, 7).Value = emp.RiesgoProfesional;
            worksheet.Cell(row, 8).Value = emp.TotalCss;

            // Formato moneda
            worksheet.Range(row, 3, row, 8).Style.NumberFormat.Format = "#,##0.00";
            row++;
        }

        // Totales
        worksheet.Cell(row, 2).Value = "TOTALES";
        worksheet.Cell(row, 2).Style.Font.Bold = true;
        worksheet.Cell(row, 3).Value = reporte.Totales.TotalSalarios;
        worksheet.Cell(row, 5).Value = reporte.Totales.TotalCssEmpleado;
        worksheet.Cell(row, 6).Value = reporte.Totales.TotalCssPatrono;
        worksheet.Cell(row, 7).Value = reporte.Totales.TotalRiesgo;
        worksheet.Cell(row, 8).Value = reporte.Totales.GranTotal;

        var totalRange = worksheet.Range(row, 1, row, 8);
        totalRange.Style.Font.Bold = true;
        totalRange.Style.Fill.BackgroundColor = XLColor.LightYellow;
        worksheet.Range(row, 3, row, 8).Style.NumberFormat.Format = "#,##0.00";

        // Ajustar anchos
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    #endregion

    #region PDF - CSS

    /// <summary>
    /// Exporta el reporte de CSS a formato PDF
    /// </summary>
    public byte[] ExportarPdfCss(ReporteCssDto reporte)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter.Landscape());
                page.Margin(1, Unit.Centimetre);

                page.Header().Column(column =>
                {
                    column.Item().Text(reporte.NombreEmpresa).FontSize(16).Bold();
                    column.Item().Text($"RUC: {reporte.Ruc}").FontSize(10);
                    column.Item().Text($"PLANILLA CSS - {reporte.Periodo}").FontSize(12).Bold();
                    column.Item().PaddingBottom(10);
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // Cédula
                        columns.RelativeColumn(3); // Nombre
                        columns.RelativeColumn(2); // Salario
                        columns.RelativeColumn(2); // Base CSS
                        columns.RelativeColumn(2); // CSS Emp
                        columns.RelativeColumn(2); // CSS Pat
                        columns.RelativeColumn(2); // Riesgo
                        columns.RelativeColumn(2); // Total
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Cédula").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Nombre").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Salario").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Base CSS").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("CSS Emp.").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("CSS Pat.").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Riesgo").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Total").Bold();
                    });

                    // Data rows
                    foreach (var emp in reporte.Empleados)
                    {
                        table.Cell().Padding(3).Text(emp.Cedula);
                        table.Cell().Padding(3).Text(emp.NombreCompleto);
                        table.Cell().Padding(3).AlignRight().Text($"${emp.SalarioBruto:N2}");
                        table.Cell().Padding(3).AlignRight().Text($"${emp.BaseCss:N2}");
                        table.Cell().Padding(3).AlignRight().Text($"${emp.CssEmpleado:N2}");
                        table.Cell().Padding(3).AlignRight().Text($"${emp.CssPatrono:N2}");
                        table.Cell().Padding(3).AlignRight().Text($"${emp.RiesgoProfesional:N2}");
                        table.Cell().Padding(3).AlignRight().Text($"${emp.TotalCss:N2}");
                    }

                    // Totals row
                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).Text("");
                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).Text("TOTALES").Bold();
                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).AlignRight().Text($"${reporte.Totales.TotalSalarios:N2}").Bold();
                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).Text("");
                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).AlignRight().Text($"${reporte.Totales.TotalCssEmpleado:N2}").Bold();
                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).AlignRight().Text($"${reporte.Totales.TotalCssPatrono:N2}").Bold();
                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).AlignRight().Text($"${reporte.Totales.TotalRiesgo:N2}").Bold();
                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).AlignRight().Text($"${reporte.Totales.GranTotal:N2}").Bold();
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span($"Generado: {reporte.FechaGeneracion:dd/MM/yyyy HH:mm} | Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    #endregion

    #region Excel - Seguro Educativo

    /// <summary>
    /// Exporta el reporte de Seguro Educativo a formato Excel
    /// </summary>
    public byte[] ExportarExcelSe(ReporteSeDto reporte)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Seguro Educativo");

        // Encabezado
        worksheet.Cell("A1").Value = reporte.NombreEmpresa;
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 14;
        worksheet.Range("A1:F1").Merge();

        worksheet.Cell("A2").Value = $"RUC: {reporte.Ruc}";
        worksheet.Cell("A3").Value = $"Período: {reporte.Periodo}";
        worksheet.Cell("A4").Value = "REPORTE SEGURO EDUCATIVO";
        worksheet.Cell("A4").Style.Font.Bold = true;

        // Headers
        var headerRow = 6;
        worksheet.Cell(headerRow, 1).Value = "Cédula";
        worksheet.Cell(headerRow, 2).Value = "Nombre";
        worksheet.Cell(headerRow, 3).Value = "Salario Bruto";
        worksheet.Cell(headerRow, 4).Value = "SE Empleado";
        worksheet.Cell(headerRow, 5).Value = "SE Patrono";
        worksheet.Cell(headerRow, 6).Value = "Total SE";

        var headerRange = worksheet.Range(headerRow, 1, headerRow, 6);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Datos
        var row = headerRow + 1;
        foreach (var emp in reporte.Empleados)
        {
            worksheet.Cell(row, 1).Value = emp.Cedula;
            worksheet.Cell(row, 2).Value = emp.NombreCompleto;
            worksheet.Cell(row, 3).Value = emp.SalarioBruto;
            worksheet.Cell(row, 4).Value = emp.SeEmpleado;
            worksheet.Cell(row, 5).Value = emp.SePatrono;
            worksheet.Cell(row, 6).Value = emp.TotalSe;

            worksheet.Range(row, 3, row, 6).Style.NumberFormat.Format = "#,##0.00";
            row++;
        }

        // Totales
        worksheet.Cell(row, 2).Value = "TOTALES";
        worksheet.Cell(row, 2).Style.Font.Bold = true;
        worksheet.Cell(row, 3).Value = reporte.Totales.TotalSalarios;
        worksheet.Cell(row, 4).Value = reporte.Totales.TotalSeEmpleado;
        worksheet.Cell(row, 5).Value = reporte.Totales.TotalSePatrono;
        worksheet.Cell(row, 6).Value = reporte.Totales.GranTotal;

        var totalRange = worksheet.Range(row, 1, row, 6);
        totalRange.Style.Font.Bold = true;
        totalRange.Style.Fill.BackgroundColor = XLColor.LightYellow;
        worksheet.Range(row, 3, row, 6).Style.NumberFormat.Format = "#,##0.00";

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    #endregion

    #region PDF - Seguro Educativo

    public byte[] ExportarPdfSe(ReporteSeDto reporte)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(1, Unit.Centimetre);

                page.Header().Column(column =>
                {
                    column.Item().Text(reporte.NombreEmpresa).FontSize(16).Bold();
                    column.Item().Text($"RUC: {reporte.Ruc}").FontSize(10);
                    column.Item().Text($"SEGURO EDUCATIVO - {reporte.Periodo}").FontSize(12).Bold();
                    column.Item().PaddingBottom(10);
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Cédula").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Nombre").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Salario").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("SE Emp.").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("SE Pat.").Bold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Total").Bold();
                    });

                    foreach (var emp in reporte.Empleados)
                    {
                        table.Cell().Padding(3).Text(emp.Cedula);
                        table.Cell().Padding(3).Text(emp.NombreCompleto);
                        table.Cell().Padding(3).AlignRight().Text($"${emp.SalarioBruto:N2}");
                        table.Cell().Padding(3).AlignRight().Text($"${emp.SeEmpleado:N2}");
                        table.Cell().Padding(3).AlignRight().Text($"${emp.SePatrono:N2}");
                        table.Cell().Padding(3).AlignRight().Text($"${emp.TotalSe:N2}");
                    }

                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).Text("");
                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).Text("TOTALES").Bold();
                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).AlignRight().Text($"${reporte.Totales.TotalSalarios:N2}").Bold();
                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).AlignRight().Text($"${reporte.Totales.TotalSeEmpleado:N2}").Bold();
                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).AlignRight().Text($"${reporte.Totales.TotalSePatrono:N2}").Bold();
                    table.Cell().Background(Colors.Yellow.Lighten3).Padding(3).AlignRight().Text($"${reporte.Totales.GranTotal:N2}").Bold();
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span($"Generado: {reporte.FechaGeneracion:dd/MM/yyyy HH:mm} | Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    #endregion

    // Los métodos para ISR y Planilla Detallada siguen el mismo patrón
    // Se pueden implementar según necesidad
}
