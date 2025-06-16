using System.Globalization;

namespace MovieTicketSystem.Helpers
{
    public static class PriceFormatHelper
    {
        public static string FormatPrice(decimal price)
        {
            return price.ToString("0 VNĐ", CultureInfo.InvariantCulture);
        }
        
        public static string FormatPriceForJavaScript(decimal price)
        {
            return price.ToString("#,###");
        }
    }
}
