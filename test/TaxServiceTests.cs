using incomeTax.Interfaces;
using incomeTax.Models;
using incomeTax.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace incomeTaxTests
{
    public class TaxServiceTests
    {
      private readonly Mock<ILogger<TaxService>> _loggerMoq;
      private readonly Mock<IMarginalTaxClient> _marginalTaxClientMoq;

      public TaxServiceTests()
      {
          _loggerMoq = new Mock<ILogger<TaxService>>();
          _marginalTaxClientMoq = new Mock<IMarginalTaxClient>();

          var marginalTaxRates = GetMarginalTaxRates();

          _marginalTaxClientMoq.Setup(client => client.GetMarginalTaxRates(2020))
            .ReturnsAsync(marginalTaxRates);
      }

      [Theory]
      [InlineData(2020, 30000, 999.9)]
      [InlineData(2020, 10000, 0)]
      [InlineData(2020, 50000, 3999.7)]
      [InlineData(2020, 90000, 11999.4)]
      [InlineData(2020, 110000, 15999.4)]
      [InlineData(2020, 100000, 11999.4)]
      [InlineData(2020, 0, 0)]
      [InlineData(2020, -100000, 0)]
      [InlineData(2020, 20002, 0.1)]
      [InlineData(2020, 20010, 0.9)]
      public async Task TaxService_CalculatesMarginalTaxRate(int taxYear, decimal income, decimal expectedResult)
      {
        //arrange
        var annualIncome = new AnnualIncomeModel
        {
            TaxYear = taxYear,
            Income = income
        };

        var taxService = new TaxService(_loggerMoq.Object, _marginalTaxClientMoq.Object);

        //act
        var result = await taxService.CalculateTotalMarginalIncomeTax(annualIncome);

        //assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(expectedResult, result.TotalIncomeTaxRaw);
      }

      [Theory]
      [InlineData(2021,100000,HttpStatusCode.NotFound,0)]
      [InlineData(2021,100000,HttpStatusCode.InternalServerError,0)]
      public async Task TaxService_CalculateMarginalTaxRate_ReturnsErrorStatusCodesWhenMarginalRatesNotFound(int taxYear, decimal income, HttpStatusCode statusCode, decimal expectedIncome)
      {
        //arrange
        var annualIncome = new AnnualIncomeModel
        {
            TaxYear = taxYear,
            Income = income
        };

        var marginalTaxRates = new MarginalTaxRatesResponseModel
        {
          StatusCode = statusCode
        };

        _marginalTaxClientMoq.Setup(client => client.GetMarginalTaxRates(2021))
          .ReturnsAsync(marginalTaxRates);

        var taxService = new TaxService(_loggerMoq.Object, _marginalTaxClientMoq.Object);

        var expectedStatusCode = statusCode;
        var expectedTotalIncome = expectedIncome;

        //act
        var result = await taxService.CalculateTotalMarginalIncomeTax(annualIncome);

        //assert
        Assert.Equal(expectedStatusCode, result.StatusCode);
        Assert.Equal(expectedTotalIncome, result.TotalIncomeTaxRaw);
      }

      private static MarginalTaxRatesResponseModel GetMarginalTaxRates()
      {
        return new MarginalTaxRatesResponseModel 
        {
            TaxBrackets = new List<TaxBracketModel>
            {
                new TaxBracketModel 
                {
                    MaxIncome = 20000,
                    MinIncome = 0,
                    Rate = 0
                },
                new TaxBracketModel
                {
                    MaxIncome = 40000,
                    MinIncome = 20001,
                    Rate = 0.1m
                },
                new TaxBracketModel
                {
                  MaxIncome = 60000,
                  MinIncome = 40001,
                  Rate = 0.2m
                },
                new TaxBracketModel
                {
                  MaxIncome = 80000,
                  MinIncome = 60001,
                  Rate = 0.3m
                },
                new TaxBracketModel
                {
                  MinIncome = 100000,
                  Rate = 0.4m
                }
            },
            StatusCode = HttpStatusCode.OK
        };
      }
  }
}