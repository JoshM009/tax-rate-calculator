using incomeTax.Constants;
using incomeTax.Models;
using incomeTax.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace incomeTax.Controllers
{
    [ApiController]
    [Route(MarginalTaxApi.TotalIncomeTaxRoute)]
  public class TotalIncomeTaxController : ControllerBase
  {
    private readonly ILogger<TotalIncomeTaxController> _logger;
    private readonly ITaxService _taxService;

    public TotalIncomeTaxController(ILogger<TotalIncomeTaxController> logger, ITaxService taxService)
    {
        _logger = logger;
        _taxService = taxService;
    }

    [HttpPost]
    [Route(MarginalTaxApi.MarginalApiRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IncomeTaxResponseModel>> CalculateIncomeTax(AnnualIncomeModel incomeTax)
    {
      _logger.LogInformation($"Received total income tax request: {incomeTax}");

      var result = await _taxService.CalculateTotalMarginalIncomeTax(incomeTax);

      if(result.StatusCode == System.Net.HttpStatusCode.OK)
      {
        return Ok(result);
      }
      else if(result.StatusCode == System.Net.HttpStatusCode.NotFound)
      {
        return NotFound($"Marginal tax information not found for the tax year provided: {incomeTax.TaxYear}");
      }
      //Assume an internal server error occurred and let end user know
      else
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
    }
  }

}