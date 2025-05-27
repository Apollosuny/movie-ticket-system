using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieTicketSystem.Data;
using Microsoft.AspNetCore.Identity;
using MovieTicketSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<MovieTicketContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieTicketContext") ?? throw new InvalidOperationException("Connection string 'MovieTicketContext' not found.")));

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
    .AddEntityFrameworkStores<MovieTicketContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Add Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole",
        policy => policy.RequireRole("Administrator"));
    options.AddPolicy("RequireUserRole",
        policy => policy.RequireRole("User"));
});

var app = builder.Build();

// Tạo roles và người dùng admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    
    // Tạo roles
    string[] roleNames = { "Administrator", "User" };
    IdentityResult roleResult;

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Tạo tài khoản admin mặc định
    var powerUser = new ApplicationUser 
    { 
        UserName = "admin@movieticket.com",
        Email = "admin@movieticket.com",
        FullName = "System Administrator",
        EmailConfirmed = true,
        CreatedAt = DateTime.Now
    };
    
    string userPassword = "Admin@123";
    var adminUser = await userManager.FindByEmailAsync(powerUser.Email);

    if (adminUser == null)
    {
        var createPowerUser = await userManager.CreateAsync(powerUser, userPassword);
        if (createPowerUser.Succeeded)
        {
            await userManager.AddToRoleAsync(powerUser, "Administrator");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
