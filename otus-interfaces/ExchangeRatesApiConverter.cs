using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace otus_interfaces
{
    internal class ExchangeRatesApiConverter :ICurrencyConverter
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

        public class ExchangeRatesApiResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("timestamp")]
            public long Timestamp { get; set; }

            [JsonProperty("base")]
            public string Base { get; set; }

            [JsonProperty("date")]
            public DateTimeOffset Date { get; set; }

            [JsonProperty("rates")]
            public Dictionary<string, decimal> Rates { get; set; }

            public ExchangeRatesApiResponse(
                bool success, 
                long timestamp,
                string baseCode,
                DateTimeOffset date,
                Dictionary<string, decimal> rates)
            {
                Success   = success;
                Timestamp = timestamp;
                Base  = baseCode;
                Date  = date;
                Rates = rates.ToDictionary(entry => entry.Key,
                                           entry => entry.Value);
            }

            public ExchangeRatesApiResponse() { }

            /// <summary>
            /// Swapping the Base to Transaction's currency code and convert exchange rates
            /// </summary>
            /// <param name="currencyCode"></param>
            /// <returns></returns>
            public ExchangeRatesApiResponse SwapBase(string currencyCode)
            {
                var exchangeRatesApiResponse = new ExchangeRatesApiResponse(Success, Timestamp, Base, Date, Rates);
                exchangeRatesApiResponse.ConvertExchangeRates(currencyCode);

                return exchangeRatesApiResponse;
            }

            private void ConvertExchangeRates(string currencyCode)
            {
                if (Base != currencyCode &&
                    IsCurrencyCodeExists(currencyCode))
                {
                    var currencyRate = GetRate(currencyCode);
                    Rates[Base] = 1;

                    foreach (var key in Rates.Keys.ToList())
                    {
                        decimal oldRate = Rates[key];
                        decimal baseCurrencyRate = currencyRate != 0 ? currencyRate : 1;

                        Rates[key] = oldRate / baseCurrencyRate;
                    }

                    Base = currencyCode;
                }
            }

            public bool IsCurrencyCodeExists(string currencyCode)
            {
                return Rates.ContainsKey(currencyCode);
            }

            public decimal GetRate(string currencyCode)
            {
                if (IsCurrencyCodeExists(currencyCode))
                {
                    return Rates[currencyCode];
                }

                return 1;
            }
        }
    }
}