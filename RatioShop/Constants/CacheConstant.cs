namespace RatioShop.Constants
{
    public static class CacheConstant
    {        
        // constant key
        public const string SiteSettingKey = "site-setting";
        public const string AdminSiteSettingKey = "admin-site-setting";
        public const string PDPKey = "pdp-setting";
        public const string PLPKey = "plp-setting";
        public static CancellationTokenSource PDPCancellation = new CancellationTokenSource();
        public static CancellationTokenSource PLPCancellation = new CancellationTokenSource();

        // key name setting
        public static string GeneralSettingKey = "general-setting";
        public static string SEOSettingKey = "seo-setting";
        public static string HeaderSettingKey = "header-setting";
        public static string FooterSettingKey = "footer-setting";
        public static string HeaderSlidesSettingKey = "banner-setting";

        public static string AdminHeaderSettingKey = "admin-header-setting";
        public static string AdminFooterSettingKey = "admin-footer-setting";
        public static string AdminGeneralSettingKey = "admin-general-setting";

        // common cache
        public static string CacheCommonSetting = "cache-common-setting";

        // base cache
        public static string BaseCache = "cache";

        // apis
        public static string ProductsApi = "api-products";
        public static string ProductApi = "api-product";        

        // view
        public static string Products = "products";
        public static string Product = "product";
    }
}
