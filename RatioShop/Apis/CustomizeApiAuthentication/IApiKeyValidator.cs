namespace RatioShop.Apis.CustomizeApiAuthentication
{
    public interface IApiKeyValidator
    {
        bool IsValid(string apiKey);
    }
}
