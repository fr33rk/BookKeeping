using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.Services;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Logger;

namespace PL.BookKeeping.Business.Services
{
	/// <summary>Import transactions from one or more csv files.
	/// </summary>
	public class DataImporterService : IDataImporterService, IDisposable
	{
		#region Fields

		private readonly ITransactionDataService mTransactionDataService;
		private readonly ILogFile mLogFile;

		private int mImported;
		private int mDuplicate;
		private StreamReader mFileStream;

		#endregion Fields

		#region Constructor(s)

		/// <summary>Initializes a new instance of the <see cref="DataImporterService"/> class.
		/// </summary>
		/// <param name="transactionDataService">The transaction data service.</param>
		/// <param name="logFile"></param>
		public DataImporterService(ITransactionDataService transactionDataService, ILogFile logFile)
		{
			mTransactionDataService = transactionDataService;
			mLogFile = logFile;
		}

		/// <summary>Finalizes an instance of the <see cref="DataImporterService"/> class.
		/// </summary>
		~DataImporterService()
		{
			Dispose(false);
		}

		#endregion Constructor(s)

		#region ImportFiles

		/// <summary>Import the transaction data from a list of files.
		/// </summary>
		/// <param name="files">The files.</param>
		public IList<Transaction> ImportFiles(IEnumerable<string> files)
		{
			var retValue = new List<Transaction>();

			mImported = 0;
			mDuplicate = 0;

			foreach (var file in files)
			{
				retValue = retValue.Concat(Import(file)).ToList();
			}

			return retValue;
		}

		#endregion ImportFiles

		#region Event OnDataProcessed

		/// <summary>Occurs when a line in a imported file has been processed.</summary>
		public event EventHandler<DataImportedEventArgs> OnDataProcessed;

		private void SignalDataProcessed()
		{
			OnDataProcessed?.Invoke(this, new DataImportedEventArgs(mImported, mDuplicate));
		}

		#endregion Event OnDataProcessed

		#region Helper methods

		#region FileStream

		protected virtual void OpenFileStream(string fileName)
		{
			mFileStream = new StreamReader(fileName);
		}

		protected virtual string ReadLine()
		{
			return mFileStream.ReadLine();
		}

		protected virtual bool IsAtEndOfStream()
		{
			return mFileStream.EndOfStream;
		}

		#endregion FileStream

		private IEnumerable<Transaction> Import(string fileName)
		{
			var retValue = new List<Transaction>();
			var sepparators = new[] { "\",\"" };

			try
			{
				OpenFileStream(fileName);

				// First line is a header...
				ReadLine();

				while (!IsAtEndOfStream())
				{
					var values = ReadLine().ToUpper().Split(sepparators, 9, StringSplitOptions.None);
					var transaction = ProcessLine(values);
					if (mTransactionDataService.Add(transaction))
					{
						mImported++;
						retValue.Add(transaction);
					}
					else
					{
						mDuplicate++;
					}
					SignalDataProcessed();
				}
			}
			catch (Exception e)
			{
				mLogFile.Error($"Unable to import {fileName}. The following exception occurred: {e.Message}");
			}

			return retValue;
		}

		private static Transaction ProcessLine(IList<string> values)
		{
			var retValue = new Transaction();

			for (var i = 0; i < values.Count; i++)
			{
				values[i] = values[i].Replace("\"", "");
			}

			retValue.Date = DateTime.ParseExact(values[0], "yyyyMMdd", CultureInfo.InvariantCulture);
			retValue.Name = values[1];
			retValue.Account = values[2];
			retValue.CounterAccount = values[3];
			retValue.Code = values[4];
			retValue.MutationType = values[5].ToUpper() == "AF" ? MutationType.Debit : MutationType.Credit;
			retValue.Amount = Convert.ToDecimal(values[6], CultureInfo.GetCultureInfo("nl-NL")); // CultureInfo.InvariantCulture);
			retValue.MutationKind = values[7];
			retValue.Remarks = values[8];

			if (retValue.MutationType == MutationType.Debit)
			{
				retValue.Amount *= -1;
			}

			return retValue;
		}

		#endregion Helper methods

		#region IDisposable

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mFileStream")]
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				mFileStream?.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable
	}
}