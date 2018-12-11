using Newtonsoft.Json;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.EventAggregatorEvents;
using PL.BookKeeping.Infrastructure.Services;
using PL.Logger;
using Prism.Events;
using System;
using System.Diagnostics;
using System.IO;

namespace PL.BookKeeping.Business.Services
{
	public class SettingsService : ISettingsService<Settings>
	{
		protected internal string mFileName;
		private readonly ILogFile mLogFile;
		private IEventAggregator mEventAggregator;

		public SettingsService(ILogFile logFile, IEventAggregator eventAggregator)
		{
			DetermineFileName();
			mLogFile = logFile;
			mEventAggregator = eventAggregator;
		}

		private void DetermineFileName()
		{
			var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();

			if (entryAssembly != null)
			{
				var fileVersionInfo = FileVersionInfo.GetVersionInfo(entryAssembly.Location);
				var companyName = fileVersionInfo.CompanyName;
				var productName = fileVersionInfo.ProductName;

				if (companyName != string.Empty && productName != string.Empty)
				{
					mFileName = $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\{companyName}\{productName}\{productName}.Setting.json";
				}
				else
				{
					throw new InvalidOperationException("Unable to determine log file location due to missing company name or product name.");
				}
			}
			else
			{
				throw new InvalidOperationException("Unable to determine log file location due to missing FileVersionInfo");
			}
		}

		public void LoadSettings()
		{
			try
			{
				// deserialize JSON directly from a file
				using (var file = File.OpenText(mFileName))
				{
					var serializer = new JsonSerializer();
					Settings = (Settings)serializer.Deserialize(file, typeof(Settings));
				}
			}
			catch (FileNotFoundException)
			{
				mLogFile.Info("Settings file not found. Using defaults and trying to create a file.");
				Settings = new Settings();
				SaveSettings();
			}
			catch (Exception e)
			{
				mLogFile.Error($"Unable to load settings file. Error:{e.Message}");
				mLogFile.Info("Loading default settings instead.");
				Settings = new Settings();
			}
		}

		public void SaveSettings(bool silent = false)
		{
			try
			{
				// serialize JSON directly to a file
				using (var file = File.CreateText(mFileName))
				{
					var serializer = new JsonSerializer();
					serializer.Serialize(file, Settings);
				}
				mEventAggregator.GetEvent<OptionsChangedEvent>().Publish(Settings);
			}
			catch (Exception e)
			{
				mLogFile.Error($"Unable to save the settings file. Error: {e.Message}");
				throw;
			}
		}

		public Settings Settings { get; set; }
	}
}