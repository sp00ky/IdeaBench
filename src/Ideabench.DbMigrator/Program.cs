using Ideabench.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
	.SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
	.AddEnvironmentVariables()
	.Build();

var connectionString = configuration.GetConnectionString("DefaultConnection")
	?? "Data Source=ideabench.db";

var options = new DbContextOptionsBuilder<AppDbContext>()
	.UseSqlite(connectionString)
	.Options;

await using var dbContext = new AppDbContext(options);
await dbContext.Database.MigrateAsync();

Console.WriteLine("Database migration complete.");
