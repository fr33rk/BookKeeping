﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
	public class DataImporterService : IDataImporterService
	{
		#region Fields

		private ITransactionDataService mTransactionDataService;
		private ILogFile mLogFile;
		private IFileReaderService mFileReaderService;

		private int mImported;
		private int mDuplicate;

		#endregion Fields

		#region Constructor(s)

		/// <summary>Initializes a new instance of the <see cref="DataImporterService"/> class.
		/// </summary>
		/// <param name="transactionDataService">The transaction data service.</param>
		public DataImporterService(ITransactionDataService transactionDataService, ILogFile logFile,
			IFileReaderService fileReaderService)
		{
			mTransactionDataService = transactionDataService;
			mLogFile = logFile;
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
				retValue = retValue.Concat(import(file)).ToList();
			}

			return retValue;
		}

		#endregion ImportFiles

		#region Event OnDataProcessed

		/// <summary>Occurs when a line in a imported file has been processed.</summary>
		public event EventHandler<DataImportedEventArgs> OnDataProcessed;

		private void signalDataProcessed()
		{
			OnDataProcessed?.Invoke(this, new DataImportedEventArgs(mImported, mDuplicate));
		}

		#endregion Event OnDataProcessed

		#region Helper methods

		private IList<Transaction> import(string fileName)
		{
			var retValue = new List<Transaction>();
			try
			{ 
				mFileReaderService.Open(fileName);

				// First line is a header...
				mFileReaderService.ReadLine();
				var sepparators = new string[] { "\",\"" };

				Transaction transaction;

			
				while (!mFileReaderService.EndOfStream)
				{
					var values = mFileReaderService.ReadLine()
						.ToUpper()
						.Split(sepparators, 9, StringSplitOptions.None);

					transaction = processLine(values);
					if (mTransactionDataService.Add(transaction))
					{
						mImported++;
						retValue.Add(transaction);
					}
					else
					{
						mDuplicate++;
					}
					signalDataProcessed();
				}
			}
			catch (Exception e)
			{
				mLogFile.Error(string.Format("Unable to import {0}. The following exception occurred: {1}", fileName, e.Message));
			}

			return retValue;
		}

		private Transaction processLine(string[] values)
		{
			var retValue = new Transaction();

			for (int i = 0; i < values.Count(); i++)
			{
				values[i] = values[i].Replace("\"", "");
			}

			retValue.Date = DateTime.ParseExact(values[0], "yyyyMMdd", CultureInfo.InvariantCulture);
			retValue.Name = values[1];
			retValue.Account = values[2];
			retValue.CounterAccount = values[3];
			retValue.Code = values[4];
			retValue.MutationType = values[5].ToUpper() == "AF" ? MutationType.Debit : MutationType.Credit;
			retValue.Amount = Convert.ToDecimal(values[6]);
			retValue.MutationKind = values[7];
			retValue.Remarks = values[8];

			if (retValue.MutationType == MutationType.Debit)
			{
				retValue.Amount *= -1;
			}

			return retValue;
		}

		#endregion Helper methods
	}
}