using System.Text.Json.Serialization;
using System.Globalization;
using System.Net;

namespace incomeTax.Models
{
  public class IncomeTaxResponseModel 
  {
    [JsonPropertyName("totalIncomeTax")]
    public string TotalIncomeTax { get; set; }

    private decimal _totalIncomeTaxRaw;

    [JsonIgnore]
    public decimal TotalIncomeTaxRaw 
    { 
      get
      {
        return _totalIncomeTaxRaw;
      } 
      set
      {
        _totalIncomeTaxRaw = value;
        //TODO: Take in and use Accept-Language header from requests for formatting like this
        TotalIncomeTax = _totalIncomeTaxRaw.ToString("F", CultureInfo.InvariantCulture);
      }
    }

    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }
  }
}