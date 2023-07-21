using RatioShop.Constants;
using RatioShop.Data.ViewModels.SearchViewModel;
using RatioShop.Enums;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RatioShop.Helpers
{
    public class Browser
    {
        public string userAgent { get; set; }
        public Regex OS { get; set; }
        public Regex device { get; set; }
    }
    public static class CommonHelper
    {
        public static string CurrencyFormat(decimal? price, string language)
        {
            decimal priceValue = price ?? 0;

            return priceValue.ToString("C", new CultureInfo(language));
        }

        public static string BuildOrderNumberByDate(string shopSignature)
        {
            var random = new Random();
            var result = $"{shopSignature}{DateTime.UtcNow.ToString("yyMMddHHmmss")}{random.Next(1, 100)}";
            return result;

        }

        public static DeviceType GetClientDevice(HttpRequest request)
        {
            string device_info = string.Empty;
            var browserinfo = new Browser()
            {
                userAgent = request.Headers["User-Agent"].ToString(),
                OS = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline),
                device = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline)
            };
            if (browserinfo.OS.IsMatch(browserinfo.userAgent))
            {
                device_info = browserinfo.OS.Match(browserinfo.userAgent).Groups[0].Value;
            }
            if (browserinfo.device.IsMatch(browserinfo.userAgent.Substring(0, 4)))
            {
                device_info += browserinfo.device.Match(browserinfo.userAgent).Groups[0].Value;
            }
            if (!string.IsNullOrEmpty(device_info))
            {
                return DeviceType.Mobile;
            }
            else
            {
                return DeviceType.Desktop;
            }
        }

        public static T SessionGet<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }

        public static void SessionSet<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static string GetStatusBadgeClass(string status, int bootstrapVersion = 4)
        {
            var badgeClass = "";
            string BadgeSuccess = bootstrapVersion == 4 ? "badge-success" : "bg-success";
            string BadgeLight = bootstrapVersion == 4 ? "badge-light" : "bg-light text-dark";
            string BadgeInfo = bootstrapVersion == 4 ? "badge-info" : "bg-light text-dark";
            string BadgeDanger = bootstrapVersion == 4 ? "badge-danger" : "bg-danger";
            string BadgeWarning = bootstrapVersion == 4 ? "badge-warning" : "bg-warning text-dark";            

            switch (status)
            {
                case CommonStatus.Success:
                case CommonStatus.OrderStatus.Complete:
                case CommonStatus.ShipmentStatus.Delivered:
                    badgeClass = BadgeSuccess;
                    break;
                case CommonStatus.OrderStatus.Created:
                case CommonStatus.ShipmentStatus.Pending:
                    badgeClass = BadgeLight;
                    break;
                case CommonStatus.OrderStatus.PaymentRecieved:
                case CommonStatus.OrderStatus.Delivering:
                    badgeClass = BadgeInfo;
                    break;
                case CommonStatus.OrderStatus.Canceled:
                case CommonStatus.OrderStatus.Closed:
                case CommonStatus.ShipmentStatus.Expired:
                case CommonStatus.ShipmentStatus.Failure:
                    badgeClass = BadgeDanger;
                    break;
                case CommonStatus.OrderStatus.PendingPayment:
                case CommonStatus.ShipmentStatus.Returned:

                    badgeClass = BadgeWarning;
                    break;
                default:
                    badgeClass = BadgeLight;
                    break;
            }
            return badgeClass;
        }

        public static DateTime? GetCorrectUTC(this DateTime? dateTime)
        {            
            var isParseSuccess = DateTime.TryParse(dateTime?.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"), out var dateTimeUTC);            
            if(isParseSuccess) return dateTimeUTC.ToUniversalTime();
            return dateTime;            
        }

        public static IEnumerable<FacetFilterItem>? CleanDefaultFilter(this IEnumerable<FacetFilterItem>? filters)
        {
            if (filters == null || !filters.Any()) return null;

            var result = new List<FacetFilterItem>();
            foreach (var filter in filters)
            {
                if (filter == null || filter.Value.Equals("Default", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(filter.Value)) continue;
                result.Add(filter);
            }

            return result;
        }

        public static string? FilterItemToJson(this IEnumerable<FacetFilterItem>? filters)
        {
            if (filters == null || !filters.Any()) return null;
            filters = filters.Where(x => x != null);

            return JsonSerializer.Serialize(filters);
        }
    }
}
