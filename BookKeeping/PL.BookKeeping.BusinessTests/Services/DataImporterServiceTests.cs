using NSubstitute;
using NUnit.Framework;
using PL.BookKeeping.Business.Services;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PL.BookKeeping.Business.Tests.Services
{
	[TestFixture]
	public class DataImporterServiceTests
	{
		#region DataImportService test class

		/// <inheritdoc />
		/// <summary>Child class for the DataImporterService (the unit under test). This class
		/// stubs the file I/O stream. Real file I/O is not allowed in a unit test.
		/// </summary>
		private class DataImporterServiceImportFileTest : DataImporterService
		{
			#region Constructor(s)

			/// <summary>Initializes a new instance of the <see cref="DataImporterServiceImportFileTest"/> class.
			/// </summary>
			/// <param name="transactionDataService">The transaction data service.</param>
			/// <param name="logFile">The log file.</param>
			public DataImporterServiceImportFileTest(ITransactionDataService transactionDataService, ILogFile logFile)
				: base(transactionDataService, logFile)
			{
			}

			#endregion Constructor(s)

			#region Definitions

			private readonly List<TestFile> mTestFiles = new List<TestFile>();
			private TestFile mCurrentTestFile;

			#endregion Definitions

			#region Add test file

			public void AddTestFile(TestFile testFile)
			{
				mTestFiles.Add(testFile);
			}

			#endregion Add test file

			#region DataImporterService

			protected override void OpenFileStream(string fileName)
			{
				mCurrentTestFile = mTestFiles.FirstOrDefault(t => t.Name == fileName);

				if (mCurrentTestFile == null)
				{
					throw new System.IO.FileNotFoundException();
				}
			}

			protected override string ReadLine()
			{
				return mCurrentTestFile.GetNextLine();
			}

			protected override bool IsAtEndOfStream()
			{
				return mCurrentTestFile.IsEndOfFile;
			}

			#endregion DataImporterService
		}

		#endregion DataImportService test class

		#region Test files

		private class TestFile
		{
			private readonly List<string> mLines = new List<string>();
			private readonly List<Transaction> mTransactions = new List<Transaction>();
			private int mLineCounter;

			public TestFile(string name, string header)
			{
				Name = name;
				mLines.Add(header);
			}

			public void AddRecord(string record, Transaction transaction)
			{
				mLines.Add(record);
				mTransactions.Add(transaction);
			}

			public string GetNextLine()
			{
				return mLines[mLineCounter++];
			}

			public Transaction GetTransactionByIndex(int index)
			{
				// First line is the header.
				return mTransactions[index];
			}

			public bool IsEndOfFile => mLineCounter >= mLines.Count;

			public string Name { get; }
		}

		private TestFile mFileWithOnlyHeader;
		private TestFile mFileWithOneRecord;
		private TestFile mFileWithMultipleRecords;
		private TestFile mFileWithInvalidRecords;

		#endregion Test files

		#region ImportFilesTest_ErrorOpeningTheFile

		[OneTimeSetUp]
		public void InitializeTests()
		{
			const string header = "DATE,NAME,ACCOUNT,COUNTERACCOUNT,CODE,KIND,AMOUNT,KIND,REMARKS";

			mFileWithOnlyHeader = new TestFile("FileWithOnlyHeader", header);

			mFileWithOneRecord = new TestFile("FileWithOneRecord", header);
			mFileWithOneRecord.AddRecord("\"19780921\",\"Freerk\",\"1234567890\",\"0987654321\",\"A\",\"AF\",\"2109,78\",\"PIN\",\"Remarks\"",
				new Transaction()
				{
					Date = new DateTime(1978, 09, 21),
					Name = "FREERK",
					Account = "1234567890",
					CounterAccount = "0987654321",
					Code = "A",
					MutationType = MutationType.Debit,
					Amount = -2109.78m,
					MutationKind = "PIN",
					Remarks = "REMARKS"
				});

			mFileWithMultipleRecords = new TestFile("FileWithMultipleRecords", header);
			mFileWithMultipleRecords.AddRecord("\"19780921\",\"Freerk\",\"1234567890\",\"0987654321\",\"A\",\"AF\",\"2109,78\",\"PIN\",\"Remarks\"",
				new Transaction()
				{
					Date = new DateTime(1978, 09, 21),
					Name = "FREERK",
					Account = "1234567890",
					CounterAccount = "0987654321",
					Code = "A",
					MutationType = MutationType.Debit,
					Amount = -2109.78m,
					MutationKind = "PIN",
					Remarks = "REMARKS"
				});

			// Because the remark is 'Duplicate' the data service stub will report this record as a duplicate transaction.
			mFileWithMultipleRecords.AddRecord("\"19780921\",\"Freerk\",\"1234567890\",\"0987654321\",\"A\",\"AF\",\"2109,78\",\"PIN\",\"Duplicate\"",
				new Transaction()
				{
					Date = new DateTime(1978, 09, 21),
					Name = "FREERK",
					Account = "1234567890",
					CounterAccount = "0987654321",
					Code = "A",
					MutationType = MutationType.Debit,
					Amount = -2109.78m,
					MutationKind = "PIN",
					Remarks = "REMARKS"
				});

			mFileWithMultipleRecords.AddRecord("\"19790713\",\"Djuke\",\"2345678901\",\"9876543210\",\"B\",\"BIJ\",\"19797,13\",\"ACC\",\"\"",
				new Transaction()
				{
					Date = new DateTime(1979, 07, 13),
					Name = "DJUKE",
					Account = "2345678901",
					CounterAccount = "9876543210",
					Code = "B",
					MutationType = MutationType.Credit,
					Amount = 19797.13m,
					MutationKind = "ACC",
					Remarks = ""
				});

			mFileWithInvalidRecords = new TestFile("FileWithInvalidRecord", header);
			mFileWithInvalidRecords.AddRecord("\"19780921\",\"Freerk\",\"1234567890\",\"0987654321\",\"A\",\"AF\",\"2109,78\",\"PIN\",\"Remarks\"", null);
			// Invalid date
			mFileWithInvalidRecords.AddRecord("\"190713\",\"Djuke\",\"2345678901\",\"9876543210\",\"B\",\"BIJ\",\"19797,13\",\"ACC\",\"\"", null);
			// Invalid kind
			mFileWithInvalidRecords.AddRecord("\"19780921\",\"Djuke\",\"2345678901\",\"9876543210\",\"B\",\"WITH\",\"19797,13\",\"ACC\",\"\"", null);
		}

		private DataImporterServiceImportFileTest CreateUnitUnderTest(ITransactionDataService transactionDataService, ILogFile logFile)
		{
			var retValue = new DataImporterServiceImportFileTest(transactionDataService, logFile);

			retValue.AddTestFile(mFileWithOnlyHeader);
			retValue.AddTestFile(mFileWithOneRecord);
			retValue.AddTestFile(mFileWithMultipleRecords);
			retValue.AddTestFile(mFileWithInvalidRecords);

			return retValue;
		}

		// Error opening the file
		// Expected: no transactions returned, entry in the log file.
		[Test]
		public void ImportFilesTest_ErrorOpeningTheFile()
		{
			var mockLogFile = Substitute.For<ILogFile>();
			var transactionDataService = Substitute.For<ITransactionDataService>();

			var unitUnderTest = CreateUnitUnderTest(transactionDataService, mockLogFile);
			unitUnderTest.ImportFiles(new List<string>() { "InvalidFileName" });

			mockLogFile.Received(1).Error(Arg.Is<string>(x => x.Contains("Unable to import")));
			transactionDataService.DidNotReceive().Add(Arg.Any<Transaction>());
		}

		#endregion ImportFilesTest_ErrorOpeningTheFile

		[Test]
		public void ImportFilesTest_ImportFileWithOnlyHeader()
		{
			var mockLogFile = Substitute.For<ILogFile>();
			var transactionDataService = Substitute.For<ITransactionDataService>();

			var unitUnderTest = CreateUnitUnderTest(transactionDataService, mockLogFile);

			unitUnderTest.ImportFiles(new List<string>() { mFileWithOnlyHeader.Name });

			mockLogFile.DidNotReceive().Error(Arg.Any<string>());
			transactionDataService.DidNotReceive().Add(Arg.Any<Transaction>());
		}

		[Test]
		public void ImportFilesTest_ImportFileWithOneRecord()
		{
			// Creation and initialization.
			var mockLogFile = Substitute.For<ILogFile>();
			var transactionDataService = Substitute.For<ITransactionDataService>();

			transactionDataService.Add(Arg.Any<Transaction>()).Returns(true);

			var unitUnderTest = CreateUnitUnderTest(transactionDataService, mockLogFile);
			// Start test
			unitUnderTest.ImportFiles(new List<string>() { mFileWithOneRecord.Name });

			// Check test results
			mockLogFile.DidNotReceive().Error(Arg.Any<string>());
			// Test that the correct transaction has been added.
			transactionDataService.Received(1).Add(Arg.Is<Transaction>(t => t.IsEqual(mFileWithOneRecord.GetTransactionByIndex(0))));
		}

		[Test]
		public void ImportFilesTest_ImportFileWithMultipleRecords()
		{
			// Given
			var mockLogFile = Substitute.For<ILogFile>();
			var transactionDataService = Substitute.For<ITransactionDataService>();

			transactionDataService.Add(Arg.Any<Transaction>()).Returns(true);

			var unitUnderTest = CreateUnitUnderTest(transactionDataService, mockLogFile);

			// When
			unitUnderTest.ImportFiles(new List<string>() { mFileWithMultipleRecords.Name });

			// Then
			mockLogFile.DidNotReceive().Error(Arg.Any<string>());

			// Test that the correct transaction has been added.
			transactionDataService.Received(3).Add(Arg.Any<Transaction>());
			transactionDataService.Received(1).Add(Arg.Is<Transaction>(t => t.IsEqual(mFileWithMultipleRecords.GetTransactionByIndex(0))));
			transactionDataService.Received(1).Add(Arg.Is<Transaction>(t => t.IsEqual(mFileWithMultipleRecords.GetTransactionByIndex(1))));
			transactionDataService.Received(1).Add(Arg.Is<Transaction>(t => t.IsEqual(mFileWithMultipleRecords.GetTransactionByIndex(2))));
		}

		#region ImportFilesTest_TriggeredUpdateImported

		public interface IDataProcessedSubscriber
		{
			void React(object sender, DataImportedEventArgs e);
		}

		[Test]
		public void ImportFilesTest_TriggeredUpdateImported()
		{
			// Given
			var stubLogFile = Substitute.For<ILogFile>();
			var transactionDataService = Substitute.For<ITransactionDataService>();
			var mockSubscriber = Substitute.For<IDataProcessedSubscriber>();

			transactionDataService.Add(Arg.Is<Transaction>(t => t.Remarks == "DUPLICATE")).Returns(false);
			transactionDataService.Add(Arg.Is<Transaction>(t => t.Remarks != "DUPLICATE")).Returns(true);

			var unitUnderTest = CreateUnitUnderTest(transactionDataService, stubLogFile);
			unitUnderTest.OnDataProcessed += mockSubscriber.React;

			// When
			unitUnderTest.ImportFiles(new List<string> { mFileWithMultipleRecords.Name });

			// Then
			mockSubscriber.Received(3).React(Arg.Any<object>(), Arg.Any<DataImportedEventArgs>());
			mockSubscriber.Received(1).React(Arg.Any<object>(), Arg.Is<DataImportedEventArgs>(e => e.Imported == 1 && e.Duplicate == 0));
			mockSubscriber.Received(1).React(Arg.Any<object>(), Arg.Is<DataImportedEventArgs>(e => e.Imported == 1 && e.Duplicate == 1));
			mockSubscriber.Received(1).React(Arg.Any<object>(), Arg.Is<DataImportedEventArgs>(e => e.Imported == 2 && e.Duplicate == 1));
		}

		#endregion ImportFilesTest_TriggeredUpdateImported

		[Test]
		public void ImportFilesTest_ImportCorruptFile()
		{
			// Given
			var mockLogFile = Substitute.For<ILogFile>();
			var transactionDataService = Substitute.For<ITransactionDataService>();

			transactionDataService.Add(Arg.Any<Transaction>()).Returns(true);

			var unitUnderTest = CreateUnitUnderTest(transactionDataService, mockLogFile);

			// When
			unitUnderTest.ImportFiles(new List<string>() { mFileWithInvalidRecords.Name });

			// Then
			mockLogFile.Received(1).Error(Arg.Is<string>(x => x.Contains("Unable to import")));
			transactionDataService.Received(1).Add(Arg.Any<Transaction>());
		}

		[Test]
		public void ImportFilesTest_ImportMultipleFiles()
		{
			Assert.Ignore("Not implemented");
		}

		[Test]
		public void ImportFilesTest_ImportMultipleFilesIncludingCorrupted()
		{
			Assert.Ignore("Not implemented");
		}

		[Test]
		public void ImportFilesTest_ImportMultipleFilesIncludingMissingFile()
		{
			Assert.Ignore("Not implemented");
		}
	}
}