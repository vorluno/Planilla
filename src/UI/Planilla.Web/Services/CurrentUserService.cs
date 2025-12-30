// ====================================================================
// Planilla - CurrentUserService
// Source: Phase E - Hardening
// Creado: 2025-12-26
// Descripción: Implementación de ICurrentUserService
// Lee claims del HttpContext para multi-tenancy y auditoría
// ====================================================================

using System.Security.Claims;
using Vorluno.Planilla.Application.Interfaces;

namespace Vorluno.Planilla.Web.Services;

/// <summary>
/// Implementación de ICurrentUserService que obtiene información del usuario
/// desde el HttpContext.User (ClaimsPrincipal).
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// CompanyId del usuario actual desde el claim "CompanyId".
    /// IMPORTANTE: Este claim debe asignarse durante el login/registro.
    /// </summary>
    public int? CompanyId
    {
        get
        {
            var companyIdClaim = _httpContextAccessor.HttpContext?.User
                ?.FindFirst("CompanyId")?.Value;

            if (string.IsNullOrWhiteSpace(companyIdClaim))
            {
                return null;
            }

            if (int.TryParse(companyIdClaim, out int companyId))
            {
                return companyId;
            }

            return null;
        }
    }

    /// <summary>
    /// UserId del usuario actual desde el claim NameIdentifier (Identity default).
    /// </summary>
    public string? UserId
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User
                ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }

    /// <summary>
    /// Email del usuario actual desde el claim Email.
    /// </summary>
    public string? Email
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User
                ?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }

    /// <summary>
    /// Indica si hay un usuario autenticado en el contexto actual.
    /// </summary>
    public bool IsAuthenticated
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
    }
}
