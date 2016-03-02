using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Logger;

namespace PL.BookKeeping.Business.Services.Tests
{
	[TestFixture()]
	public class DataImporterServiceTests
	{
		static List<string> ImportAllFiles = new List<string>()
		{
			"FileX", "FileA", "FileB", "FileC"
		};

		static List<string> ImportInvalidFile = new List<string>()
		{
			"FileX"
		};

		static List<string> ImportFileWithOnlyHeader = new List<string>()
		{
			"FileA"
		};


		static List<string> ImportFileWithOneRecord = new List<string>()
		{
			"FileB"
		};

		private class TestTransactionService : ITransactionDataService
		{
			public IList<Transaction> AddedTransactions { get; }

			public TestTransactionService()
			{
				AddedTransactions = new List<Transaction>();
			}

			public bool Add(BookKeeping.Entities.Transaction transaction)
			{
				AddedTransactions.Add(transaction);
				return true;
			}

			public BookKeeping.Entities.Transaction AttachEntities(IUnitOfWork unitOfWork, BookKeeping.Entities.Transaction entity)
			{
				throw new NotImplementedException();
			}

			public void Delete(BookKeeping.Entities.Transaction entity)
			{
				throw new NotImplementedException();
			}

			public IList<BookKeeping.Entities.Transaction> GetAll(bool complete = false)
			{
				throw new NotImplementedException();
			}

			public IEnumerable<BookKeeping.Entities.Transaction> GetByEntryPeriod(BookKeeping.Entities.EntryPeriod entryPeriod)
			{
				throw new NotImplementedException();
			}

			public BookKeeping.Entities.Transaction GetByKey(int key, bool complete = false)
			{
				throw new NotImplementedException();
			}

			public IList<BookKeeping.Entities.Transaction> GetOfPeriod(DateTime startDate, DateTime endDate)
			{
				throw new NotImplementedException();
			}

			public void ResetPeriodEntryLinks()
			{
				throw new NotImplementedException();
			}

			public void Update(BookKeeping.Entities.Transaction entity)
			{
				throw new NotImplementedException();
			}

			void IBaseTraceableObjectDataService<BookKeeping.Entities.Transaction>.Add(BookKeeping.Entities.Transaction entity)
			{
				throw new NotImplementedException();
			}
		}

		private class TestLogFile : ILogFile
		{
			public IList<string> Errors { get; }

			public TestLogFile()
			{
				Errors = new List<string>();
			}

			public event EventHandler<LogEventArgs> OnLog;

			public void Critical(string message)
			{
				throw new NotImplementedException();
			}

			public void Debug(string message)
			{
				throw new NotImplementedException();
			}

			public void Error(string message)
			{
				Errors.Add(message);
			}

			public void Info(string message)
			{
				throw new NotImplementedException();
			}

			public void Warning(string message)
			{
				throw new NotImplementedException();
			}

			public void WriteLogEnd()
			{
				throw new NotImplementedException();
			}

			public void WriteLogStart()
			{
				throw new NotImplementedException();
			}
		}

		private class DataImporterService_ErrorOpeningTheFile : DataImporterService
		{
			public DataImporterService_ErrorOpeningTheFile(ITransactionDataService transactionDataService, ILogFile logFile)
				: base(transactionDataService, logFile)
			{
				RaiseFileNotFoundException = false;
			}

			public enum TestFile
			{
				/// <summary>File A contains only a header.</summary>
				FileWithOnlyHeader,
				/// <summary>File B contains 1 transaction.</summary>
				FileWithOneRecord,
				/// <summary>File C contains 10 transactions.</summary>
				FileC,
				/// <summary>File X contains faulty configured transactions</summary>
				FileX,
			}

			private TestFile mUsedTestFile;

			public bool RaiseFileNotFoundException { get; set; }

			protected override void openFileStream(string fileName)
			{
				if (RaiseFileNotFoundException)
					throw new System.IO.FileNotFoundException();
				else
				{
					if (fileName == "FileA")
					{
						mUsedTestFile = TestFile.FileWithOnlyHeader;
						mLineCounter = fileWithOnlyHeader.Length-1;
					}
					else if (fileName == "FileB")
					{
						mUsedTestFile = TestFile.FileWithOneRecord;
						mLineCounter = fileWithOneRecord.Length-1;
					}
				}
			}

			protected override string readLine()
			{
				if (mUsedTestFile == TestFile.FileWithOnlyHeader)
				{
					return fileWithOnlyHeader[mLineCounter--];
				}
				else if (mUsedTestFile == TestFile.FileWithOneRecord)
				{
					return fileWithOneRecord[mLineCounter--];
				}

				return "";
			}

			private string[] fileWithOnlyHeader = new string[1]
			{
				"DATE,NAME,ACCOUNT,OUNTERACCOUT,CODE,KIND,AMOUNT,KIND,REMARKS",
			};

			public string[] fileWithOneRecord = new string[2]
			{
				"DATE,NAME,ACCOUNT,COUNTERACCOUT,CODE,KIND,AMOUNT,KIND,REMARKS",
				@"19780921,Freerk,1234567890,0987654321,A,AF,2109.78,PIN,Remarks",
			};

			private int mLineCounter = -1;

			protected override bool isAtEndOfStream()
			{
				return mLineCounter == -1;
			}
		}


		// Error opening the file
		// Expected: no transactions returned, entry in the log file.
		[Test()]
		public void ImportFilesTest_ErrorOpeningTheFile()
		{
			var mockLogFile = Substitute.For<ILogFile>();
			var mockTransactionDataSerice = Substitute.For<ITransactionDataService>();

			var unitUnderTest = new DataImporterService_ErrorOpeningTheFile(mockTransactionDataSerice, mockLogFile);
			unitUnderTest.ImportFiles(ImportInvalidFile);

			mockLogFile.Received(1).Error(Arg.Is<string>(x => x.Contains("Unable to import")));
			mockTransactionDataSerice.DidNotReceive().Add(Arg.Any<Transaction>());
		}

		[Test()]
		public void ImportFilesTest_ImportFileWithOnlyHeader()
		{
			var stubLogFile = Substitute.For<ILogFile>();
			var mockTransactionDataSerice = Substitute.For<ITransactionDataService>();

			var unitUnderTest = new DataImporterService_ErrorOpeningTheFile(mockTransactionDataSerice, stubLogFile);
			unitUnderTest.ImportFiles(ImportFileWithOnlyHeader);

			mockTransactionDataSerice.DidNotReceive().Add(Arg.Any<Transaction>());			
		}

		[Test]
		public void ImportFilesTest_ImportFileWithOneRecord()
		{
			var stubLogFile = Substitute.For<ILogFile>();
			var mockTransactionDataSerice = Substitute.For<ITransactionDataService>();

			var unitUnderTest = new DataImporterService_ErrorOpeningTheFile(mockTransactionDataSerice, stubLogFile);
			unitUnderTest.ImportFiles(ImportFileWithOneRecord);

			mockTransactionDataSerice.Received(1).Add(Arg.Is<Transaction>(t => t.Name == "FREERK"));
		}
	}
}