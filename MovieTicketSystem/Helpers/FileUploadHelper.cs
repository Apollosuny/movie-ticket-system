using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MovieTicketSystem.Helpers
{
    public static class FileUploadHelper
    {
        public static async Task<string> SaveImageAsync(IFormFile imageFile, string subFolder)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }

            try
            {
                // Lấy đường dẫn thư mục gốc của ứng dụng
                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                
                // Tạo đường dẫn tới thư mục con
                var uploadsFolder = Path.Combine(webRootPath, subFolder);
                
                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(uploadsFolder))
                {
                    try 
                    {
                        Directory.CreateDirectory(uploadsFolder);
                        
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Console.WriteLine($"Không thể tạo thư mục: {uploadsFolder}");
                            uploadsFolder = Path.Combine(Path.GetTempPath(), "movie-images");
                            Directory.CreateDirectory(uploadsFolder);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi tạo thư mục: {ex.Message}");
                        uploadsFolder = Path.Combine(Path.GetTempPath(), "movie-images");
                        Directory.CreateDirectory(uploadsFolder);
                    }
                }
                
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
                
                return $"/{subFolder.Replace("\\", "/")}/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
                return null;
            }
        }
    }
}
