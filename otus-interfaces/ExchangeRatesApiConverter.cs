using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace otus_interfaces
{
    internal partial class ExchangeRatesApiConverter :ICurrencyConverter
    {
        private readonly HttpClient _httpClient;
        private readonly MemoryCache _memoryCache;
        private readonly string _apiKey;
        private const string RATES_CACHING_COMMON_KEY = "RATES_CACHING_COMMON_KEY";

        public ExchangeRatesApiConverter(HttpClient httpClient, MemoryCache memoryCache, string apiKey)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _apiKey = apiKey;
        }

        ICurrencyAmount ICurrencyConverter.ConvertCurrency(ICurrencyAmount amount, string currencyCode)
        {
            decimal convertedAmount = amount.Amount * GetExchangeRateValue(amount.CurrencyCode, currencyCode);

            return new CurrencyAmount(currencyCode, convertedAmount);
        }

        private decimal GetExchangeRateValue(string currencyCodeFrom, string currencyCodeTo)
        {
            var exchangeRatesApiResponse = CacheGetOrCreateRatesAsynchronous(currencyCodeFrom).Result;

            // Retrieving rate for currency code 
            return exchangeRatesApiResponse.GetRate(currencyCodeTo);
        }

        private async Task<ExchangeRatesApiResponse> CacheGetOrCreateRatesAsynchronous(string currencyCode)
        {
            var ratesEntry = await _memoryCache.GetOrCreateAsync(currencyCode, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                return Task.FromResult<ExchangeRatesApiResponse>(CacheGetOrCreateCommonRatesAsynchronous().Result.SwapBase(currencyCode));
            });

            return ratesEntry;
        }

        private async Task<ExchangeRatesApiResponse> CacheGetOrCreateCommonRatesAsynchronous()
        {
            var commonRatesEntry = await _memoryCache.GetOrCreateAsync(RATES_CACHING_COMMON_KEY, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                return Task.FromResult<ExchangeRatesApiResponse>(GetExchangeRatesApiResponse().Result);
            });

            return commonRatesEntry;
        }

        private async Task<ExchangeRatesApiResponse> GetExchangeRatesApiResponse()
        {
            return await GetExchangeRatesHttpAsync();
        }

        private async Task<ExchangeRatesApiResponse> GetExchangeRatesHttpAsync()
        {
            //Trace.TraceInformation("Request to exchangeratesapi is sending");

            var response = await _httpClient.GetAsync($"http://api.exchangeratesapi.io/v1/latest?access_key={_apiKey}");

            //Trace.TraceInformation("Response from exchangeratesapi is received");

            response = response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ExchangeRatesApiResponse>(json);
        }
    }
}