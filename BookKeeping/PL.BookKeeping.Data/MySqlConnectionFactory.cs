using System.Data.Common;
using MySql.Data.MySqlClient;
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

		public DbConnection Create()
		{
			var settings = mSettingsService.Settings;
			return new MySqlConnection($@"Server={settings.ServerName};Database={settings.DatabaseName};Uid={settings.UserId};Password={settings.Password}");
		}
	}
}