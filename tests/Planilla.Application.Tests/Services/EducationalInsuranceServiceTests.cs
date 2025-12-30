// ====================================================================
// Planilla - EducationalInsuranceServiceTests
// Source: Core360 Stage 5, Sección 2.3
// Creado: 2025-12-26
// Descripción: Tests unitarios del servicio de Seguro Educativo
// CRÍTICO: SE NO tiene tope máximo, se aplica sobre salario completo
// ====================================================================

using FluentAssertions;
using Vorluno.Planilla.Application.Services;
using Vorluno.Planilla.Application.Tests.Helpers;

namespace Vorluno.Planilla.Application.Tests.Services;

/// <summary>
/// Tests unitarios del servicio de Seguro Educativo.
/// NOTA CRÍTICA: El Seguro Educativo NO tiene tope máximo, se aplica sobre el salario total.
/// </summary>
public class EducationalInsuranceServiceTests
{
    private const int DefaultCompanyId = 1;
    private readonly DateTime _calculationDate = new(2025, 1, 15);

    [Fact]
    public async Task CalculateEmployeeInsurance__SalarioNormal__ReturnsCorrectAmount()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new EducationalInsuranceServicePortable(mockProvider);

        var grossPay = 1500m;
        var isSubject = true;

        // Act
        var result = await service.CalculateEmployeeInsuranceAsync(
            DefaultCompanyId,
            grossPay,
            isSubject,
            _calculationDate
        );

        // Assert
        result.Should().Be(18.75m); // 1500 * 1.25% = 18.75
    }

    [Fact]
    public async Task CalculateEmployerInsurance__SalarioNormal__ReturnsCorrectAmount()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new EducationalInsuranceServicePortable(mockProvider);

        var grossPay = 1500m;
        var isSubject = true;

        // Act
        var result = await service.CalculateEmployerInsuranceAsync(
            DefaultCompanyId,
            grossPay,
            isSubject,
            _calculationDate
        );

        // Assert
        result.Should().Be(22.50m); // 1500 * 1.50% = 22.50
    }

    [Fact]
    public async Task CalculateFullInsurance__SalarioNormal__ReturnsCorrectResult()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new EducationalInsuranceServicePortable(mockProvider);

        var grossPay = 1500m;
        var isSubject = true;

        // Act
        var result = await service.CalculateFullInsuranceAsync(
            DefaultCompanyId,
            grossPay,
            isSubject,
            _calculationDate
        );

        // Assert
        result.EmployeeRate.Should().Be(1.25m);
        result.EmployerRate.Should().Be(1.50m);
        result.EmployeeDeduction.Should().Be(18.75m); // 1500 * 1.25%
        result.EmployerContribution.Should().Be(22.50m); // 1500 * 1.50%
        result.Total.Should().Be(41.25m); // 18.75 + 22.50
    }

    [Fact]
    public async Task CalculateEmployeeInsurance__SinTopeMaximo__AplicaSobreSalarioCompleto()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new EducationalInsuranceServicePortable(mockProvider);

        // Salario alto (mayor que cualquier tope CSS)
        var grossPay = 5000m;
        var isSubject = true;

        // Act
        var result = await service.CalculateEmployeeInsuranceAsync(
            DefaultCompanyId,
            grossPay,
            isSubject,
            _calculationDate
        );

        // Assert
        // CRÍTICO: SE NO tiene tope, se aplica sobre los B/. 5,000 completos
        result.Should().Be(62.50m); // 5000 * 1.25% = 62.50
    }

    [Fact]
    public async Task CalculateEmployeeInsurance__NotSubject__ReturnsZero()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new EducationalInsuranceServicePortable(mockProvider);

        var grossPay = 1500m;
        var isSubject = false; // NO sujeto a Seguro Educativo

        // Act
        var result = await service.CalculateEmployeeInsuranceAsync(
            DefaultCompanyId,
            grossPay,
            isSubject,
            _calculationDate
        );

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task CalculateEmployeeInsurance__NoConfig__ThrowsInvalidOperationException()
    {
        // Arrange
        var mockProvider = MockPayrollConfigProvider.WithMissingConfig();
        var service = new EducationalInsuranceServicePortable(mockProvider);

        var grossPay = 1500m;
        var isSubject = true;

        // Act
        Func<Task> act = async () => await service.CalculateEmployeeInsuranceAsync(
            DefaultCompanyId,
            grossPay,
            isSubject,
            _calculationDate
        );

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*No se encontró configuración de Seguro Educativo*");
    }

    [Fact]
    public async Task CalculateEmployeeInsurance__SalarioCero__ReturnsZero()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new EducationalInsuranceServicePortable(mockProvider);

        var grossPay = 0m;
        var isSubject = true;

        // Act
        var result = await service.CalculateEmployeeInsuranceAsync(
            DefaultCompanyId,
            grossPay,
            isSubject,
            _calculationDate
        );

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task CalculateEmployeeInsurance__SalarioMinimo__ReturnsCorrectAmount()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new EducationalInsuranceServicePortable(mockProvider);

        var grossPay = 1000m; // Salario mínimo aproximado
        var isSubject = true;

        // Act
        var result = await service.CalculateEmployeeInsuranceAsync(
            DefaultCompanyId,
            grossPay,
            isSubject,
            _calculationDate
        );

        // Assert
        result.Should().Be(12.50m); // 1000 * 1.25% = 12.50
    }
}
