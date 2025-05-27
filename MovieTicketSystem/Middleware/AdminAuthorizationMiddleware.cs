using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MovieTicketSystem.Models;
using System.Threading.Tasks;

namespace MovieTicketSystem.Middleware
{
    public class AdminAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context, 
            UserManager<ApplicationUser> userManager)
        {
            var path = context.Request.Path.ToString().ToLower();
            
            // Kiểm tra nếu đường dẫn URL thuộc về phần quản trị
            if (path.StartsWith("/admin"))
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                if (!context.User.Identity.IsAuthenticated)
                {
                    // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
                    context.Response.Redirect($"/Account/Login?returnUrl={context.Request.Path}");
                    return;
                }

                // Kiểm tra xem người dùng có quyền Admin hay không
                var user = await userManager.GetUserAsync(context.User);
                var isAdmin = user != null && await userManager.IsInRoleAsync(user, "Administrator");

                if (!isAdmin)
                {
                    // Chuyển hướng đến trang từ chối nếu không phải Admin
                    context.Response.Redirect("/Account/AccessDenied");
                    return;
                }
            }
            
            // Tiếp tục đến middleware tiếp theo trong pipeline
            await _next(context);
        }
    }

    // Extension method để đơn giản hóa việc đăng ký middleware
    public static class AdminAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminAuthorization(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AdminAuthorizationMiddleware>();
        }
    }
}
