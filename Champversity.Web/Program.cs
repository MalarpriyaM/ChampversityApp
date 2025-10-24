using Champversity.DataAccess;
using Champversity.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=ChampversityDb;Trusted_Connection=True;"
    )
);

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Register file storage service
string baseDirectory = builder.Environment.ContentRootPath;
builder.Services.AddSingleton(new FileStorage(baseDirectory));

// Register application services
builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<ValidationService>();
builder.Services.AddScoped<IUniversityService, MockUniversityService>();
builder.Services.AddScoped<IManualTaskService, ManualTaskService>();

// Register background service for batch processing
builder.Services.AddHostedService<BatchProcessingService>();

var app = builder.Build();

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
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Initialize default data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
  var validationService = services.GetRequiredService<ValidationService>();
        
        await SeedDataAsync(context, userManager, roleManager, validationService);
    }
    catch (Exception ex)
    {
    var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();

static async Task SeedDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ValidationService validationService)
{
    // Ensure database is created
    context.Database.EnsureCreated();
    
    // Create roles
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    
  if (!await roleManager.RoleExistsAsync("Staff"))
    {
  await roleManager.CreateAsync(new IdentityRole("Staff"));
    }
    
    // Create default admin user
    if (await userManager.FindByEmailAsync("admin@champversity.com") == null)
    {
        var adminUser = new ApplicationUser
        {
          UserName = "admin@champversity.com",
       Email = "admin@champversity.com",
            FirstName = "Admin",
            LastName = "User",
      Role = "Admin",
            CreatedDate = DateTime.Now,
     IsActive = true,
      EmailConfirmed = true
        };
        
        await userManager.CreateAsync(adminUser, "Admin@123");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
  
    // Initialize validation rules
    await validationService.InitializeDefaultValidationRulesAsync();
}