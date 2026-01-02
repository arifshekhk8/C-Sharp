using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=library.db"));

builder.Services.AddScoped<DashboardService>();

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()          
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!db.Authors.Any())
    {
        db.Authors.AddRange(
            new Author { Name = "Robert C. Martin" },
            new Author { Name = "Andrew Hunt" },
            new Author { Name = "Erich Gamma" }
        );
        db.SaveChanges();
    }

    if (!db.Categories.Any())
    {
        db.Categories.AddRange(
            new Category { Name = "Software Engineering" },
            new Category { Name = "Programming" },
            new Category { Name = "Design Patterns" }
        );
        db.SaveChanges();
    }

    if (!db.Books.Any())
    {
        var author1 = db.Authors.First(a => a.Name == "Robert C. Martin");
        var author2 = db.Authors.First(a => a.Name == "Andrew Hunt");
        var author3 = db.Authors.First(a => a.Name == "Erich Gamma");

        var cat1 = db.Categories.First(c => c.Name == "Software Engineering");
        var cat2 = db.Categories.First(c => c.Name == "Programming");
        var cat3 = db.Categories.First(c => c.Name == "Design Patterns");

        db.Books.AddRange(
            new Book { Title = "Clean Code", PublicationYear = 2008, AuthorId = author1.Id, CategoryId = cat1.Id },
            new Book { Title = "The Pragmatic Programmer", PublicationYear = 1999, AuthorId = author2.Id, CategoryId = cat2.Id },
            new Book { Title = "Design Patterns", PublicationYear = 1994, AuthorId = author3.Id, CategoryId = cat3.Id }
        );

        db.SaveChanges();
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // Ensure roles exist
    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Ensure admin user
    var adminEmail = "admin@library.com";
    var adminPassword = "Admin@123";

        // Ensure ONLY ONE admin (remove Admin role from others)
    var arifUser = await userManager.FindByEmailAsync("arif@gmail.com");
    if (arifUser != null && await userManager.IsInRoleAsync(arifUser, "Admin"))
    {
        await userManager.RemoveFromRoleAsync(arifUser, "Admin");
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, adminPassword);
    }

    // Ensure Admin role assignment
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        // Auto-redirect admin ONLY when landing on home without exit flag
        if (context.Request.Path == "/" &&
            context.User.IsInRole("Admin") &&
            !context.Request.Query.ContainsKey("exitAdmin"))
        {
            context.Response.Redirect("/Admin/Dashboard");
            return;
        }
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();



app.Run();