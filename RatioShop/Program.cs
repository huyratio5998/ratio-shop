using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using RatioShop.Apis.CustomizeApiAuthentication;
using RatioShop.Data;
using RatioShop.Data.HttpClientFactoryClientType.Payments;
using RatioShop.Data.Models;
using RatioShop.Data.Repository.Abstract;
using RatioShop.Data.Repository.Implement;
using RatioShop.Data.ViewModels.Layout;
using RatioShop.Helpers;
using RatioShop.Services.Abstract;
using RatioShop.Services.Implement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);    
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ShopUser,IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();
builder.Services.AddRazorPages();
builder.Services.AddMvc(option =>
{
    option.Filters.Add(new AuthorizeFilter());
});
builder.Services.AddDirectoryBrowser();
//Enable authentication
var myConfig = builder.Configuration;
builder.Services.AddAuthentication()
   .AddGoogle(options =>
   {
       options.ClientId = myConfig["Authentication:Google:ClientId"];
       options.ClientSecret = myConfig["Authentication:Google:ClientSecret"];       
   })
   .AddFacebook(options =>
   {
       options.AppId = myConfig["Authentication:Facebook:AppId"];
       options.AppSecret = myConfig["Authentication:Facebook:AppSecret"];       
   });
//.AddMicrosoftAccount(microsoftOptions =>
//{
//    microsoftOptions.ClientId = config["Authentication:Microsoft:ClientId"];
//    microsoftOptions.ClientSecret = config["Authentication:Microsoft:ClientSecret"];
//})
//.AddTwitter(twitterOptions =>
//{
//    twitterOptions.ConsumerKey = config["Authentication:Twitter:ConsumerAPIKey"];
//    twitterOptions.ConsumerSecret = config["Authentication:Twitter:ConsumerSecret"];
//    twitterOptions.RetrieveUserDetails = true;
//});

//Enable session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(config => { 
    config.IOTimeout = TimeSpan.FromSeconds(5000);        
});

//
builder.Services.Configure<RequestLocalizationOptions>(options => {
    options.SetDefaultCulture("vi-VN")
    .AddSupportedCultures(new[] { "vi-VN", "en-VN" })
    .AddSupportedUICultures(new[] { "vi-VN", "en-VN" }); ;
});

//
builder.Services.AddRouting(options => options.LowercaseUrls = true);
//
builder.Services.AddHttpClient<PaypalClient>();

// Inject auto mapper
builder.Services.AddAutoMapper(typeof(Program));

// Inject layout model
builder.Services.AddScoped<ILayoutSettingsViewModel, LayoutSettingsViewModel>();

// Inject api key filter
builder.Services.AddSingleton<ApiKeyAuthorizationFilter>();
builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();

// Inject repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IProductVariantCartRepository, ProductVariantCartRepository>();
builder.Services.AddScoped<IProductVariantStockRepository, ProductVariantStockRepository>();
builder.Services.AddScoped<IShopUserRepository, ShopUserRepository>();
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<ICartDiscountRepository, CartDiscountRepository>();
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IProductVariantPackageRepository, ProductVariantPackageRepository>();
builder.Services.AddScoped<ISiteSettingRepository, SiteSettingRepository>();

//Inject services

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductVariantService, ProductVariantService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IProductVariantCartService, ProductVariantCartService>();
builder.Services.AddScoped<IProductVariantStockService, ProductVariantStockService>();
builder.Services.AddScoped<IShopUserService, ShopUserService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<ICartDiscountService, CartDiscountService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IProductVariantPackageService, ProductVariantPackageService>();
builder.Services.AddScoped<ISiteSettingService, SiteSettingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseDeveloperExceptionPage();
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// localization
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("vi-VN")
    .AddSupportedCultures(new[] { "vi-VN", "en-VN" })
    .AddSupportedUICultures(new[] { "vi-VN", "en-VN" });
app.UseRequestLocalization(localizationOptions);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// use session
app.UseSession();

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );
app.MapDefaultControllerRoute();

app.MapRazorPages();

// Add user role
await app.CreateRolesAsync(builder.Configuration);
//
app.Run();
