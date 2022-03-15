using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Net;

namespace incomeTax.Models
{
  public class MarginalTaxRatesResponseModel
  {
    [JsonPropertyName("tax_brackets")]
    public IReadOnlyList<TaxBracketModel> TaxBrackets { get; set; }

    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }
  }
}