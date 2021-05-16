using Microsoft.EntityFrameworkCore;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services;

namespace PL.BookKeeping.Data
{
	public class MySqlConnectionFactory : IDbConnectionFactory
	{
		private readonly ISettingsService<Settings> mSettingsService;

		public MySqlConnectionFactory(ISettingsService<Settings> settingsService)
		{
			mSettingsService = settingsService;
		}

		public DbContextOptions Create()
		{
			var builder = new DbContextOptionsBuilder();
			var settings = mSettingsService.Settings;

			builder.UseMySql($@"Server={settings.ServerName};Database={settings.DatabaseName};Uid={settings.UserId};Password={settings.Password}");

			return builder.Options;
		}
	}
}