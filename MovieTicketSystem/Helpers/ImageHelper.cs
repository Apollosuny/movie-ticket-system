using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace MovieTicketSystem.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// Kiểm tra xem hình ảnh có tồn tại trong wwwroot hay không, nếu không tồn tại thì trả về URL hình ảnh mặc định
        /// </summary>
        public static string GetImageUrl(string imagePath, string defaultImageUrl, IWebHostEnvironment webHostEnvironment = null)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return defaultImageUrl;
            }

            // Trả về đường dẫn tương đối, không cần kiểm tra vì đã xử lý lỗi bên client với JavaScript
            return imagePath;
        }

        /// <summary>
        /// Tạo placeholder image URL cho một movie
        /// </summary>
        public static string GetMoviePlaceholderUrl(string movieTitle)
        {
            return $"https://via.placeholder.com/400x600?text={Uri.EscapeDataString(movieTitle ?? "Movie")}";
        }
    }
}
