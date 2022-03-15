using System.ComponentModel.DataAnnotations;

namespace incomeTax.Models
{
  public class AnnualIncomeModel 
  {
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "A real year must be supplied")]
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