namespace RatioShop.Helpers.CookieHelpers
{
    public static class CookieHelpers
    {
        public static CookieOptions DefaultOptionByDays(double days)
        {
            return new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(days),
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                HttpOnly = true,
            };
        }
    }
}
