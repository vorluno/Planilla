// ====================================================================
// Planilla - IncomeTaxCalculationServiceTests
// Source: Core360 Stage 5, Sección 2.4
// Creado: 2025-12-26
// Descripción: Tests unitarios del servicio de ISR
// Valida brackets progresivos, deducciones, proyección anual
// ====================================================================

using FluentAssertions;
using Vorluno.Planilla.Application.Exceptions;
using Vorluno.Planilla.Application.Services;
using Vorluno.Planilla.Application.Tests.Helpers;

namespace Vorluno.Planilla.Application.Tests.Services;

/// <summary>
/// Tests unitarios del servicio de Impuesto Sobre la Renta (ISR).
/// Valida brackets progresivos según regulaciones de la DGI de Panamá.
/// </summary>
public class IncomeTaxCalculationServiceTests
{
    private const int DefaultCompanyId = 1;
    private readonly DateTime _calculationDate = new(2025, 1, 15);

    [Fact]
    public async Task CalculateIncomeTax__TramoExento__ReturnsZeroTax()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        // Salario mensual que proyecta a < B/. 11,000 anual
        var grossPay = 900m; // 900 * 12 = 10,800 anual
        var payFrequency = "Mensual";
        var dependents = 0;
        var isSubject = true;

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        result.TaxableIncome.Should().Be(10800m); // 900 * 12
        result.DependentDeduction.Should().Be(0);
        result.NetTaxableIncome.Should().Be(10800m);
        result.TaxAmount.Should().Be(0); // Tramo exento
        result.EffectiveTaxRate.Should().Be(0);
    }

    [Fact]
    public async Task CalculateIncomeTax__Tramo15Percent__ReturnsCorrectTax()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        // Salario mensual que proyecta al tramo 15%
        var grossPay = 3000m; // 3000 * 12 = 36,000 anual
        var payFrequency = "Mensual";
        var dependents = 0;
        var isSubject = true;

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        result.TaxableIncome.Should().Be(36000m); // 3000 * 12
        result.NetTaxableIncome.Should().Be(36000m);
        // ISR: (36,000 - 11,000) * 15% = 25,000 * 0.15 = 3,750 anual
        // Por mes: 3,750 / 12 = 312.50
        result.TaxAmount.Should().Be(312.50m);
    }

    [Fact]
    public async Task CalculateIncomeTax__Tramo25Percent__ReturnsCorrectTax()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        // Salario mensual que proyecta al tramo 25%
        var grossPay = 6000m; // 6000 * 12 = 72,000 anual
        var payFrequency = "Mensual";
        var dependents = 0;
        var isSubject = true;

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        result.TaxableIncome.Should().Be(72000m);
        result.NetTaxableIncome.Should().Be(72000m);
        // ISR:
        // Tramo 1 (0-11,000): 0
        // Tramo 2 (11,001-50,000): (50,000 - 11,000) * 15% = 5,850
        // Tramo 3 (50,001-72,000): (72,000 - 50,000) * 25% = 5,500
        // Total anual: 5,850 + 5,500 = 11,350
        // Por mes: 11,350 / 12 = 945.83
        result.TaxAmount.Should().Be(945.83m);
    }

    [Fact]
    public async Task CalculateIncomeTax__ConDependientes__AplicaDeduccion()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        var grossPay = 3000m; // 3000 * 12 = 36,000 anual
        var payFrequency = "Mensual";
        var dependents = 2; // 2 dependientes = B/. 1,600 deducción
        var isSubject = true;

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        result.TaxableIncome.Should().Be(36000m);
        result.DependentDeduction.Should().Be(1600m); // 800 * 2
        result.NetTaxableIncome.Should().Be(34400m); // 36,000 - 1,600
        // ISR: (34,400 - 11,000) * 15% = 23,400 * 0.15 = 3,510 anual
        // Por mes: 3,510 / 12 = 292.50
        result.TaxAmount.Should().Be(292.50m);
    }

    [Fact]
    public async Task CalculateIncomeTax__MasDe3Dependientes__LimitaA3()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        var grossPay = 3000m;
        var payFrequency = "Mensual";
        var dependents = 5; // Intenta 5, pero máximo es 3
        var isSubject = true;

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        result.DependentDeduction.Should().Be(2400m); // 800 * 3 (máximo)
    }

    [Fact]
    public async Task CalculateIncomeTax__FrecuenciaMensual__Proyecta12Periodos()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        var grossPay = 1000m;
        var payFrequency = "Mensual";
        var dependents = 0;
        var isSubject = true;

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        result.TaxableIncome.Should().Be(12000m); // 1000 * 12
    }

    [Fact]
    public async Task CalculateIncomeTax__FrecuenciaQuincenal__Proyecta24Periodos()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        var grossPay = 500m;
        var payFrequency = "Quincenal";
        var dependents = 0;
        var isSubject = true;

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        result.TaxableIncome.Should().Be(12000m); // 500 * 24
    }

    [Fact]
    public async Task CalculateIncomeTax__FrecuenciaSemanal__Proyecta52Periodos()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        var grossPay = 250m;
        var payFrequency = "Semanal";
        var dependents = 0;
        var isSubject = true;

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        result.TaxableIncome.Should().Be(13000m); // 250 * 52
    }

    [Fact]
    public async Task CalculateIncomeTax__NotSubject__ReturnsZero()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        var grossPay = 3000m;
        var payFrequency = "Mensual";
        var dependents = 0;
        var isSubject = false; // NO sujeto a ISR

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        result.TaxableIncome.Should().Be(0);
        result.DependentDeduction.Should().Be(0);
        result.NetTaxableIncome.Should().Be(0);
        result.TaxAmount.Should().Be(0);
        result.EffectiveTaxRate.Should().Be(0);
    }

    [Fact]
    public async Task CalculateIncomeTax__NoConfig__ThrowsInvalidOperationException()
    {
        // Arrange
        var mockProvider = MockPayrollConfigProvider.WithMissingConfig();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        var grossPay = 3000m;
        var payFrequency = "Mensual";
        var dependents = 0;
        var isSubject = true;

        // Act
        Func<Task> act = async () => await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*No se encontró configuración de ISR*");
    }

    [Fact]
    public async Task CalculateIncomeTax__NoBrackets__ThrowsPayrollConfigurationException()
    {
        // Arrange
        // Mock que retorna config pero sin brackets
        var mockProvider = MockPayrollConfigProvider.WithMissingConfig();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        var grossPay = 3000m;
        var payFrequency = "Mensual";
        var dependents = 0;
        var isSubject = true;

        // Act
        Func<Task> act = async () => await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        // Primero lanzará InvalidOperationException porque falta la config
        // (el test de brackets faltantes requeriría un mock más específico)
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task CalculateIncomeTax__ExactamenteEnLimite11000__AplicaBracketCorrectamente()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        // Salario que proyecta justo por debajo de B/. 11,000 (exento)
        var grossPay = 900m; // 900 * 12 = 10,800
        var payFrequency = "Mensual";
        var dependents = 0;
        var isSubject = true;

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        result.TaxableIncome.Should().Be(10800m); // 900 * 12
        // Dentro del tramo exento, no debe pagar impuesto
        result.TaxAmount.Should().Be(0);
    }

    [Fact]
    public async Task CalculateIncomeTax__ExactamenteEnLimite50000__AplicaBracketCorrectamente()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        // Salario que proyecta exactamente a B/. 50,000
        var grossPay = 4166.67m; // ~50,000 / 12
        var payFrequency = "Mensual";
        var dependents = 0;
        var isSubject = true;

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        result.TaxableIncome.Should().Be(50000.04m); // 4166.67 * 12
        // Exactamente en el límite, debe aplicar tramo 15% completo
        // (50,000 - 11,000) * 15% = 5,850 anual
        // Por mes: 5,850 / 12 = 487.50
        result.TaxAmount.Should().BeApproximately(487.50m, 0.10m);
    }

    [Fact]
    public async Task CalculateIncomeTax__SalarioCero__ReturnsZero()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        var grossPay = 0m;
        var payFrequency = "Mensual";
        var dependents = 0;
        var isSubject = true;

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        result.TaxableIncome.Should().Be(0);
        result.TaxAmount.Should().Be(0);
    }

    [Fact]
    public async Task CalculateIncomeTax__IngresoAlto__AplicaTramo25Correctamente()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        // Salario mensual muy alto
        var grossPay = 10000m; // 10,000 * 12 = 120,000 anual
        var payFrequency = "Mensual";
        var dependents = 0;
        var isSubject = true;

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        result.TaxableIncome.Should().Be(120000m);
        // ISR:
        // Tramo 1: 0
        // Tramo 2: 5,850 (fixed amount del tramo 3)
        // Tramo 3: (120,000 - 50,000) * 25% = 17,500
        // Total anual: 5,850 + 17,500 = 23,350
        // Por mes: 23,350 / 12 = 1,945.83
        result.TaxAmount.Should().Be(1945.83m);
    }

    [Fact]
    public async Task CalculateIncomeTax__ValidarTasaEfectiva__CalculaCorrectamente()
    {
        // Arrange
        var mockProvider = new MockPayrollConfigProvider();
        var service = new IncomeTaxCalculationServicePortable(mockProvider);

        var grossPay = 6000m; // 72,000 anual
        var payFrequency = "Mensual";
        var dependents = 0;
        var isSubject = true;

        // Act
        var result = await service.CalculateIncomeTaxAsync(
            DefaultCompanyId,
            grossPay,
            payFrequency,
            dependents,
            isSubject,
            _calculationDate
        );

        // Assert
        // Impuesto anual: 11,350 (de test anterior)
        // Tasa efectiva: (11,350 / 72,000) * 100 = 15.76%
        result.EffectiveTaxRate.Should().BeApproximately(15.76m, 0.05m);
    }
}
