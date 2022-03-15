using incomeTax.Interfaces;
using incomeTax.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace incomeTax.Clients
{
  public class MarginalTaxClient : IMarginalTaxClient
  {
    private readonly ILogger<MarginalTaxClient> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;
    private readonly string _marginalTaxUrl;
    private readonly bool _useCaching;
    private readonly TimeSpan _cacheTimeInSeconds;

    public MarginalTaxClient(ILogger<MarginalTaxClient> logger, IConfiguration configuration,
    IMemoryCache memoryCache)
    {
      _logger = logger;
      _configuration = configuration;
      _memoryCache = memoryCache;

      //TODO: Move configuration handling to an options snapshot class to take advantage of hot reloading
      _marginalTaxUrl = _configuration["IncomeTaxApi:MarginalTaxUrl"];
      _useCaching = bool.Parse(_configuration["IncomeTaxApi:UseCaching"]);
      _cacheTimeInSeconds = TimeSpan.
        FromSeconds(double.Parse(_configuration["IncomeTaxApi:CacheExpirationTimeInSeconds"]));
    }

    public async Task<MarginalTaxRatesResponseModel> GetMarginalTaxRates(int year)
    {
      if(!_useCaching)
      {
        return await GetMarginalTaxRatesFromServer(year);
      }

      if(!_memoryCache.TryGetValue(year, out MarginalTaxRatesResponseModel marginalTaxRatesCache))
      {
        _logger.LogInformation($"Cache miss, retrieving marginal tax rates for year: {year}");
        marginalTaxRatesCache = await GetMarginalTaxRatesFromServer(year);

        if(marginalTaxRatesCache.StatusCode == HttpStatusCode.OK)
        {
          _memoryCache.Set(year, marginalTaxRatesCache, _cacheTimeInSeconds);
        }
      }
      
      return marginalTaxRatesCache;
    }

    private async Task<MarginalTaxRatesResponseModel> GetMarginalTaxRatesFromServer(int year)
    {
      //TODO: Implement retries
      var result = new MarginalTaxRatesResponseModel();
      //Call service and handle response
      using (var httpClient = new HttpClient())
      {
        using (var response = await httpClient.GetAsync($"{_marginalTaxUrl}/{year}"))
        {
          if(response.IsSuccessStatusCode)
          {
            var responseRaw = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(responseRaw);
            
            try
            {
              result = JsonSerializer.Deserialize<MarginalTaxRatesResponseModel>(responseRaw);
              result.StatusCode = HttpStatusCode.OK;
            }
            //Received an unexpected response from the server, log it and mark the StatusCode
            catch(Exception e)
            {
              _logger.LogError(e, $"Received unexpected response from marginal tax rate server and could not deserialize. Response: {responseRaw}");
              result.StatusCode = HttpStatusCode.InternalServerError;
            }
          }
          else 
          {
            var statusCode = response.StatusCode;
            _logger.LogError($"Failed to retrieve marginal tax rates. Status code from server: {statusCode}");
            result.StatusCode = statusCode;
          }
        }
      }

      return result;
    }
  }
}