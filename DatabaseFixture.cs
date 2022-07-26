using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Kusachius.QueryMapper.Test
{

	[CollectionDefinition("Database collection")]
	public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
	{
		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}

	public class DatabaseFixture : IDisposable
	{
		public DbContextOptions<TestDbContext> Options;

		public DatabaseFixture()
		{

			if (!DbConfig.IsInitialized())
			{
				IConfigurationRoot configuration = new ConfigurationBuilder()
					.AddJsonFile("appsettings.Test.json", false, true)
					.Build();

				var dbUrl = configuration["DatabaseUrl"];

				DbConfig.SetDatabaseUrl(dbUrl);
				AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
			}

			DbContextOptionsBuilder<TestDbContext> optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
			DbConfig.InitializeDb(optionsBuilder);

			Options = optionsBuilder.Options;
		}

		public void Dispose()
		{
		}
	}
}
