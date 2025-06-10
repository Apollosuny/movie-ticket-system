using System.Globalization;

namespace MovieTicketSystem.Helpers
{
    public static class PriceFormatHelper
    {
        public static string FormatPrice(decimal price)
        {
            return price.ToString("#,### VNĐ", CultureInfo.InvariantCulture);
        }
        
        // Method specific for JavaScript values
        public static string FormatPriceForJavaScript(decimal price)
        {
            return price.ToString("#,###");
        }
    }
}
