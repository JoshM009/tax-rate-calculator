using System.Text.Json.Serialization;

namespace incomeTax.Models
{
  public class TaxBracketModel 
  {
    [JsonPropertyName("max")]
    public decimal MaxIncome { get; set; }

    [JsonPropertyName("min")]
    public decimal MinIncome { get; set; }
    
    [JsonPropertyName("rate")]
    public decimal Rate { get; set; }
  }
}