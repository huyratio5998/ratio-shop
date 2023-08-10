using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RatioShop.Apis.CustomizeApiAuthentication
{
    public class ApiKeyAuthorizationFilter : IAuthorizationFilter
    {
        private const string ApiKey = "Ratio-API-Key";
        private readonly IApiKeyValidator _apiKeyValidator;
        public ApiKeyAuthorizationFilter(IApiKeyValidator apiKeyValidator)
        {
            _apiKeyValidator = apiKeyValidator;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string apiKey = context.HttpContext.Request.Headers[ApiKey];
            if (!_apiKeyValidator.IsValid(apiKey))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
