using incomeTax.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using Xunit;

namespace incomeTaxTests
{
  /*This class is used to test the marginal tax API. For the purposes of the assignment it was used
  as a sanity check device. For a real project a suite of integration tests would be used to test 
  the real API for correct and incorrect input along with security and performance tests.
  */
  public class TaxServiceIntegrationTests
  {

    private string testUrl = "http://localhost:5050/api/tax/total/marginal";

    //This is commented out to allow the unit tests to run. In a production environment these would
    //be separted from the unit tests entirely.
    //[Fact]
    public async Task MarginalTaxApi_ShouldReturnCorrectIncomeForFoundTaxYear()
    {
      //arrange
      var annualIncome = new AnnualIncomeModel 
      {
        TaxYear = 2021,
        Income = 50000
      };

      var expectedResult = new IncomeTaxResponseModel 
      {
        TotalIncomeTax = "7553.9"
      };

      var postObject = JsonSerializer.Serialize(annualIncome);
      var postContent = new StringContent(postObject, Encoding.UTF8, "application/json");
      var actualResult = new IncomeTaxResponseModel();


      //act
      using (var httpClient = new HttpClient())
      {
        using (var response = await httpClient.PostAsync(testUrl, postContent))
        {
            var responseRaw = await response.Content.ReadAsStringAsync();

            actualResult = JsonSerializer.Deserialize<IncomeTaxResponseModel>(responseRaw);
            actualResult.StatusCode = HttpStatusCode.OK;
        }
      }

      //assert
      Assert.Equal(expectedResult.StatusCode, expectedResult.StatusCode);
      Assert.Equal(expectedResult.TotalIncomeTax, actualResult.TotalIncomeTax);
    }
  }
}