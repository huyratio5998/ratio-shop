namespace RatioShop.Data.ViewModels.Layout
{
    public interface ILayoutSettingsViewModel
    {        
        string StoreName { get;}
        string StoreIcon { get; }        
        HeaderSettingsViewModel HeaderSettings();
        CommonSettingsViewModel CommonSettings();
        FooterSettingsViewModel FooterSettings();        
    }
}
