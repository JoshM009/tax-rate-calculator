using System.ComponentModel.DataAnnotations;

namespace incomeTax.Models
{
  public class AnnualIncomeModel 
  {
    [Required]
    public int TaxYear { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Only positive salaries are allowed")]
    public decimal Income { get; set; }

    public override string ToString()
    {
      return $"Tax Year: {TaxYear}, Income: {Income}";
    }
  }
}