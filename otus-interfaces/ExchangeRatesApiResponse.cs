using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace otus_interfaces
{
    internal partial class ExchangeRatesApiConverter
    {
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
                //TODO: Лучше бы эту логику вынести в отдельный класс, чтобы отделить
                //      ExchangeRatesApiResponse от класса который содердит в себе значения курсов валют
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