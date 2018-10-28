using System;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.Services;

namespace PL.BookKeeping.Business.Services
{
	public class SettingsService : ISettingsService<Settings>
	{
		public void LoadSettings()
		{
			throw new NotImplementedException();
		}

		public void SaveSettings(bool silent = false)
		{
			throw new NotImplementedException();
		}

		public Settings Settings { get; set; } = new Settings();
	}
}