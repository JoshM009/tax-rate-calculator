using incomeTax.Models;
using System.Threading.Tasks;

namespace incomeTax.Interfaces
{
  public interface ITaxService
  {
    public Task<IncomeTaxResponseModel> CalculateTotalMarginalIncomeTax(AnnualIncomeModel input);
  }
}