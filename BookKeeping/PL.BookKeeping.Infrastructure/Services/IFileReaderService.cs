namespace PL.BookKeeping.Infrastructure.Services
{
	public interface IFileReaderService
	{
		bool EndOfStream { get; }

		void Open(string fileName);
		string ReadLine();
	}
}
