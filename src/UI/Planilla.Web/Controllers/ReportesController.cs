// ====================================================================
// Planilla - ReportesController
// Creado: 2025-12-28
// Descripción: Controller para generar y exportar reportes de planilla
// ====================================================================

using Microsoft.AspNetCore.Mvc;
using Vorluno.Planilla.Application.DTOs.Reportes;
using Vorluno.Planilla.Infrastructure.Services;

namespace Vorluno.Planilla.Web.Controllers;

/// <summary>
/// Controller para gestionar reportes de planilla
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly ReportesService _reportesService;
    private readonly ExportacionService _exportacionService;

    public ReportesController(ReportesService reportesService, ExportacionService exportacionService)
    {
        _reportesService = reportesService ?? throw new ArgumentNullException(nameof(reportesService));
        _exportacionService = exportacionService ?? throw new ArgumentNullException(nameof(exportacionService));
    }

    #region Visualización JSON

    /// <summary>
    /// Obtiene el reporte de CSS en formato JSON
    /// </summary>
    [HttpGet("css/{planillaId}")]
    public async Task<ActionResult<ReporteCssDto>> GetReporteCss(int planillaId)
    {
        try
        {
            var reporte = await _reportesService.GenerarReporteCss(planillaId);
            return Ok(reporte);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al generar reporte: {ex.Message}" });
        }
    }

    /// <summary>
    /// Obtiene el reporte de Seguro Educativo en formato JSON
    /// </summary>
    [HttpGet("seguro-educativo/{planillaId}")]
    public async Task<ActionResult<ReporteSeDto>> GetReporteSe(int planillaId)
    {
        try
        {
            var reporte = await _reportesService.GenerarReporteSe(planillaId);
            return Ok(reporte);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al generar reporte: {ex.Message}" });
        }
    }

    /// <summary>
    /// Obtiene el reporte de ISR en formato JSON
    /// </summary>
    [HttpGet("isr/{planillaId}")]
    public async Task<ActionResult<ReporteIsrDto>> GetReporteIsr(int planillaId)
    {
        try
        {
            var reporte = await _reportesService.GenerarReporteIsr(planillaId);
            return Ok(reporte);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al generar reporte: {ex.Message}" });
        }
    }

    /// <summary>
    /// Obtiene el reporte de planilla detallado en formato JSON
    /// </summary>
    [HttpGet("planilla-detallada/{planillaId}")]
    public async Task<ActionResult<ReportePlanillaDetalladoDto>> GetReportePlanilla(int planillaId)
    {
        try
        {
            var reporte = await _reportesService.GenerarReportePlanillaDetallada(planillaId);
            return Ok(reporte);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al generar reporte: {ex.Message}" });
        }
    }

    #endregion

    #region Exportación Excel

    /// <summary>
    /// Exporta el reporte de CSS a Excel
    /// </summary>
    [HttpGet("css/{planillaId}/excel")]
    public async Task<IActionResult> ExportarCssExcel(int planillaId)
    {
        try
        {
            var reporte = await _reportesService.GenerarReporteCss(planillaId);
            var bytes = _exportacionService.ExportarExcelCss(reporte);
            var fileName = $"PlanillaCSS_{planillaId}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al exportar: {ex.Message}" });
        }
    }

    /// <summary>
    /// Exporta el reporte de Seguro Educativo a Excel
    /// </summary>
    [HttpGet("seguro-educativo/{planillaId}/excel")]
    public async Task<IActionResult> ExportarSeExcel(int planillaId)
    {
        try
        {
            var reporte = await _reportesService.GenerarReporteSe(planillaId);
            var bytes = _exportacionService.ExportarExcelSe(reporte);
            var fileName = $"SeguroEducativo_{planillaId}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al exportar: {ex.Message}" });
        }
    }

    #endregion

    #region Exportación PDF

    /// <summary>
    /// Exporta el reporte de CSS a PDF
    /// </summary>
    [HttpGet("css/{planillaId}/pdf")]
    public async Task<IActionResult> ExportarCssPdf(int planillaId)
    {
        try
        {
            var reporte = await _reportesService.GenerarReporteCss(planillaId);
            var bytes = _exportacionService.ExportarPdfCss(reporte);
            var fileName = $"PlanillaCSS_{planillaId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return File(bytes, "application/pdf", fileName);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al exportar: {ex.Message}" });
        }
    }

    /// <summary>
    /// Exporta el reporte de Seguro Educativo a PDF
    /// </summary>
    [HttpGet("seguro-educativo/{planillaId}/pdf")]
    public async Task<IActionResult> ExportarSePdf(int planillaId)
    {
        try
        {
            var reporte = await _reportesService.GenerarReporteSe(planillaId);
            var bytes = _exportacionService.ExportarPdfSe(reporte);
            var fileName = $"SeguroEducativo_{planillaId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return File(bytes, "application/pdf", fileName);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al exportar: {ex.Message}" });
        }
    }

    #endregion
}
