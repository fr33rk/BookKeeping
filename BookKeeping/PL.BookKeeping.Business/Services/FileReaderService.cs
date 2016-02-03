using System.IO;
using PL.BookKeeping.Infrastructure.Services;

namespace PL.BookKeeping.Business.Services
{
	/// <summary>Wrapper around a StreamReader to make IoC possible.
	/// </summary>
	public class FileReaderService : IFileReaderService
	{
		#region Fields

		private StreamReader mReader;

		#endregion Fields


		/// <summary>Gets a value indicating whether the current stream is at the end of the stream.
		/// </summary>
		/// <value><c>true</c> if [end of stream]; otherwise, <c>false</c>.</value>
		/// <exception cref="System.ObjectDisposedException"></exception>
		/// <exception cref="System.NullReferenceException"></exception>
		public bool EndOfStream
		{
			get
			{
				return mReader.EndOfStream;
			}
		}

		/// <summary>Opens the specified file name.</summary>
		/// <param name="fileName">Name of the file.</param>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.IO.FileNotFoundException"></exception>
		/// <exception cref="System.IO.DirectoryNotFoundException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public void Open(string fileName)
		{
			mReader = new StreamReader(fileName);
		}

		/// <summary>Reads the line.</summary>
		/// <returns>The next line read from the stream.</returns>
		/// <exception cref="System.OutOfMemoryException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="System.NullReferenceException"></exception>
		public string ReadLine()
		{
			return mReader.ReadLine();

		}
	}
}
