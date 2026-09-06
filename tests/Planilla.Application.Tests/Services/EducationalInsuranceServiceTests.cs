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

    // ====================================================================
    // Gastos de representación
    // ====================================================================

    [Fact]
    public async Task GastoDeRepresentacion__TambienPagaSeguroEducativo()
    {
        // Criterio confirmado por el contador de la empresa: el seguro educativo
        // corre sobre el gasto de representación igual que sobre el salario.
        //
        // En la planilla esto sale solo, porque el gasto de representación entra
        // dentro del bruto (para la Caja de Seguro Social es salario, Ley 51 de
        // 2005 Art. 91 num. 6). Este test fija que así sea: si alguien lo sacara
        // del bruto para separarlo del impuesto sobre la renta, el seguro
        // educativo dejaría de cobrarse sobre él sin que nadie lo notara.
        var service = new EducationalInsuranceServicePortable(new MockPayrollConfigProvider());

        var soloSalario = await service.CalculateEmployeeInsuranceAsync(
            DefaultCompanyId, 2_000m, true, _calculationDate);

        var conGastoDeRepresentacion = await service.CalculateEmployeeInsuranceAsync(
            DefaultCompanyId, 2_000m + 1_000m, true, _calculationDate);

        soloSalario.Should().Be(25m, "1.25% de 2,000");
        conGastoDeRepresentacion.Should().Be(37.50m, "1.25% de los 3,000 completos");
    }
}
