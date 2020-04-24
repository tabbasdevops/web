using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace web
{
    [Route("api/prices")]
    public class PricesController : Controller
    {
        private static readonly string priceUrlTemplate = "http://repo:8888/price/search/findBySymbol?symbol={0}&cutoff={1:yyyy-MM-ddTHH:mm:ss}";
        private static readonly string stockUrlTemplate = "http://repo:8888/entity/search/findBySymbol?symbol={0}";
        private static readonly HttpClient client = new HttpClient();
        private static readonly string symbol = "RUB";

        // GET: api/values
        [HttpGet]
        public async Task<Response> GetAsync()
        {
            var priceUrl = string.Format(priceUrlTemplate, symbol, DateTime.UtcNow.AddHours(-5));
            var streamTask = client.GetStreamAsync(priceUrl);
            var pricesResponse = await JsonSerializer.DeserializeAsync<PriceRepoResponse>(await streamTask);

            var stockUrl = string.Format(stockUrlTemplate, symbol);
            streamTask = client.GetStreamAsync(stockUrl);
            var stockResponse = await JsonSerializer.DeserializeAsync<StockItem>(await streamTask);
        
            return new Response()
            {
                CurrencyCode = stockResponse.Symbol,
                CurrencyName = stockResponse.Name,
                Prices = pricesResponse.Wrapper.Prices
            };
        }

     }

    public class PriceRepoResponse
    {
        public class Embedded
        {
            [JsonPropertyName("price")]
            public List<PriceItem> Prices { get; set; }
        }

        [JsonPropertyName("_embedded")]
        public Embedded Wrapper { get; set; }
    }

    public class PriceItem
    {
        //[JsonPropertyName("symbol")]
        //public string Symbol { get; set; }
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [JsonPropertyName("created")]
        public DateTime Created { get; set; }
    }

    public class StockItem
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Response
    {
        public string Operation { get; set; } = "Exchange rate from USD to foreign currency";
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public List<PriceItem> Prices { get; set; }
    } 
}
