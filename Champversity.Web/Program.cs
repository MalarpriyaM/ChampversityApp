using Champversity.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(
   builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=ChampversityDb;Trusted_Connection=True;"
  )
);

// Register file storage service
string baseDirectory = builder.Environment.ContentRootPath;
builder.Services.AddSingleton(new FileStorage(baseDirectory));

// Register Excel service
builder.Services.AddScoped<ExcelService>();

// Register background service for batch processing
builder.Services.AddHostedService<BatchProcessingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}")
  .WithStaticAssets();

app.Run();