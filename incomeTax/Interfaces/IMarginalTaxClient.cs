using incomeTax.Models;
using System.Threading.Tasks;

namespace incomeTax.Interfaces
{
  public interface IMarginalTaxClient
  {
    public Task<MarginalTaxRatesResponseModel> GetMarginalTaxRates(int year);
  }
}