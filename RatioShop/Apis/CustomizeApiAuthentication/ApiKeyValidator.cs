namespace RatioShop.Apis.CustomizeApiAuthentication
{
    public class ApiKeyValidator : IApiKeyValidator
    {
        private readonly IConfiguration _configuration;
        private const string ApiKey = "Ratio-API-Key";

        public ApiKeyValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsValid(string apiKey)
        {
            var apiKeySetting = _configuration.GetValue<string>(ApiKey);
            if (!apiKeySetting.Equals(apiKey)) return false;

            return true;
        }
    }
}
