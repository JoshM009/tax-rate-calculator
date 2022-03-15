using incomeTax.Interfaces;
using incomeTax.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace incomeTax.Services
{
  public class TaxService : ITaxService
  {
    private readonly ILogger<ITaxService> _logger;
    private readonly IMarginalTaxClient _marginalTaxClient;

    public TaxService(ILogger<TaxService> logger, IMarginalTaxClient marginalTaxClient)
    {
      _logger = logger;
      _marginalTaxClient = marginalTaxClient;
    }

    public async Task<IncomeTaxResponseModel> CalculateTotalMarginalIncomeTax(AnnualIncomeModel annualIncome)
    {
      var marginalTaxRates = await _marginalTaxClient.GetMarginalTaxRates(annualIncome.TaxYear);

      var incomeTaxModel = new IncomeTaxResponseModel();

      if(marginalTaxRates.StatusCode == System.Net.HttpStatusCode.OK)
      {
        //Foreach tax bracket, calculate marginal tax
        var totalIncomeTax = 0.0m;
        var income = annualIncome.Income > 0 ? annualIncome.Income : 0;
        var taxableIncome = 0.0m;

        foreach(var taxBracket in marginalTaxRates.TaxBrackets)
        {
          taxableIncome = CalculateTaxableAmountForTaxBracket(income, taxBracket);

          totalIncomeTax += taxableIncome * taxBracket.Rate;
        }

        _logger.LogInformation($"Calculated total tax for tax year {annualIncome.TaxYear} and annual income {income}: {totalIncomeTax}");
        incomeTaxModel.StatusCode = System.Net.HttpStatusCode.OK;
        incomeTaxModel.TotalIncomeTaxRaw = totalIncomeTax;
      }
      else
      {
        _logger.LogInformation($"Failed to calculate total income tax because marginal tax is unavailable for tax year {annualIncome.TaxYear}");
        incomeTaxModel.StatusCode = marginalTaxRates.StatusCode;
      }

      return incomeTaxModel;
    }

    private decimal CalculateTaxableAmountForTaxBracket(decimal income, TaxBracketModel taxBracket)
    {
          var taxableIncome = 0.0m;

          //Max tax bracket case, we just want whatever income is left on the top end
          if(taxBracket.MaxIncome == 0)
          {
            if(income > taxBracket.MinIncome)
            {
              taxableIncome = income - taxBracket.MinIncome;
            }
          }
          else if(income >= taxBracket.MaxIncome)
          {
            taxableIncome = taxBracket.MaxIncome - taxBracket.MinIncome;
          }
          else if(income > taxBracket.MinIncome)
          {
            taxableIncome = income - taxBracket.MinIncome;
          }

          return taxableIncome;
    }
  }
}