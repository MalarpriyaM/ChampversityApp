using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Champversity.DataAccess
{
 public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
     public ApplicationDbContext CreateDbContext(string[] args)
        {
        // Get the directory of the current assembly
            var basePath = Directory.GetCurrentDirectory();
        
  // Find the Web project directory (going one level up if needed)
  string webProjectPath = Path.Combine(basePath);
         if (!File.Exists(Path.Combine(webProjectPath, "appsettings.json")))
  {
           // We might be in the DataAccess project, try to find Web project
         var parentDirectory = Directory.GetParent(basePath) ?? throw new InvalidOperationException("Unable to locate the repository root.");
         webProjectPath = Path.Combine(parentDirectory.FullName, "Champversity.Web");
      }

  // Build configuration
       IConfigurationRoot configuration = new ConfigurationBuilder()
      .SetBasePath(webProjectPath)
                .AddJsonFile("appsettings.json", optional: false)
      .Build();

        // Get connection string
          var connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            "Server=(localdb)\\mssqllocaldb;Database=ChampversityDb;Trusted_Connection=True;MultipleActiveResultSets=true";

       // Create DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        if (ShouldUseSqlite(connectionString))
        {
          var dataDirectory = Path.Combine(webProjectPath, "App_Data");
          Directory.CreateDirectory(dataDirectory);

          var sqlitePath = Path.Combine(dataDirectory, "Champversity.db");
          optionsBuilder.UseSqlite($"Data Source={sqlitePath}");
        }
        else
        {
          optionsBuilder.UseSqlServer(connectionString);
        }

          return new ApplicationDbContext(optionsBuilder.Options);
        }

        private static bool ShouldUseSqlite(string? connectionString)
        {
          if (string.IsNullOrWhiteSpace(connectionString))
          {
            return true;
          }

          return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
            connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase);
        }
}
}