using Microsoft.AspNetCore.Mvc;

namespace RatioShop.Apis.CustomizeApiAuthentication
{
    public class RatioApiKeyAttribute : ServiceFilterAttribute
    {
        public RatioApiKeyAttribute() : base(typeof(ApiKeyAuthorizationFilter))
        {
        }
    }
}
