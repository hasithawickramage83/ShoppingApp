using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configure SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Add Identity with Role support
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // Required for role-based access
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 🔹 Add MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 🔹 Seed data (Items + Admin user/role)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    // Seed sample items if DB is empty
    if (!db.Items.Any())
    {
        db.Items.AddRange(
            new Item { Name = "Laptop", Price = 1200, ImageUrl = "/images/laptop.jpg" },
            new Item { Name = "Mouse", Price = 25, ImageUrl = "/images/mouse.jpg" },
            new Item { Name = "Keyboard", Price = 50, ImageUrl = "/images/keyboard.jpg" }
        );
        db.SaveChanges();
    }

    // 🔐 Seed Admin Role and User
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    const string adminEmail = "admin@example.com";
    const string adminPassword = "Admin@123";

    // Create Admin role
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    // Create Admin user
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

// 🔹 Configure HTTP Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 🔹 Default Route (go to Items/Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Items}/{action=Index}/{id?}");

app.MapRazorPages(); // for Identity UI (Login/Register)

app.Run();
