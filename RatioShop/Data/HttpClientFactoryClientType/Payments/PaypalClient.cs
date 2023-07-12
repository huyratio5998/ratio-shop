using Newtonsoft.Json;
using RatioShop.Data.ViewModels;
using System.Text;
using System.Text.Json;

namespace RatioShop.Data.HttpClientFactoryClientType.Payments
{
    public class PaypalClient
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _options;

        public PaypalClient(HttpClient client)
        {
            _client = client;

            _client.BaseAddress = new Uri("https://paypal/api/");
            _client.Timeout = new TimeSpan(0, 0, 30);
            _client.DefaultRequestHeaders.Clear();

            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<PaypalApiResponse> ProceedPayment(OrderViewModel order)
        {
            HttpContent data = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8,"application/json");

            using(var response = await _client.PostAsync("proceedpayment", data))
            {
                response.EnsureSuccessStatusCode();                
                var responseObject = JsonConvert.DeserializeObject<PaypalApiResponse>(response.Content.ReadAsStringAsync().Result);
                return responseObject;
            }
        }

        public async Task<PaypalApiResponse> ProceedRefundPayment(OrderViewModel order)
        {
            HttpContent data = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");

            using (var response = await _client.PostAsync("proceedpayment", data))
            {
                response.EnsureSuccessStatusCode();
                var responseObject = JsonConvert.DeserializeObject<PaypalApiResponse>(response.Content.ReadAsStringAsync().Result);
                return responseObject;
            }
        }
    }
}
