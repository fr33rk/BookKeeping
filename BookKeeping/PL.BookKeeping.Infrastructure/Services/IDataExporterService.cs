namespace PL.BookKeeping.Infrastructure.Services
{
	/// <summary>Interface for data exporter services.
	/// 
	/// </summary>
	public interface IDataExporterService
	{
		/// <summary>Export the data in a JSON format to a file specified in fileName.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		void Export(string fileName);
	}
}