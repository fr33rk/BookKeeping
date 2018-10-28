namespace PL.BookKeeping.Infrastructure.Services
{
	/// <summary>Interface for the SettingsService.</summary>
	public interface ISettingsService<TSettings>
		where TSettings : class
	{
		/// <summary>Loads the settings.</summary>
		void LoadSettings();

		/// <summary>Saves the settings.
		/// </summary>
		/// <param name="silent">If true no message to the log is written.</param>
		void SaveSettings(bool silent = false);

		TSettings Settings { get; set; }
	}
}