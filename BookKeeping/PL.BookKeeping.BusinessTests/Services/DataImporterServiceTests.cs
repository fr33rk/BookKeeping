using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Services.DataServices;
using PL.Logger;

namespace PL.BookKeeping.Business.Services.Tests
{
	[TestFixture()]
	public class DataImporterServiceTests
	{
		private class DataImporterService_ImportFileTest : DataImporterService
		{
			public DataImporterService_ImportFileTest(ITransactionDataService transactionDataService, ILogFile logFile)
				: base(transactionDataService, logFile)
			{
			}

			private List<TestFile> mTestFiles = new List<TestFile>();
			private TestFile mCurrentTestFile;

			public void AddTestFile(TestFile testFile)
			{
				mTestFiles.Add(testFile);
			}

			protected override void openFileStream(string fileName)
			{
				mCurrentTestFile = mTestFiles.Where(t => t.Name == fileName)
								.FirstOrDefault();

				if (mCurrentTestFile == null)
					throw new System.IO.FileNotFoundException();
			}

			protected override string readLine()
			{
				return mCurrentTestFile.GetNextLine();				
			}

			protected override bool isAtEndOfStream()
			{
				return mCurrentTestFile.IsEndOfFile;
			}
		}

		private class TestFile
		{
			private List<string> mLines = new List<string>();
			private List<Transaction> mTransactions = new List<Transaction>();
			private int mLineCounter = 0;

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

			public bool IsEndOfFile
			{
				get
				{
					return mLineCounter >= mLines.Count;
				}
			}

			public string Name { get; private set; }


		}

		#region ImportFilesTest_ErrorOpeningTheFile

		private TestFile mFileWithOnlyHeader;
		private TestFile mFileWithOneRecord;
		private TestFile mFileWithMultipleRecords;

		[TestFixtureSetUp]
		public void InitializeTests()
		{

			string header = "DATE,NAME,ACCOUNT,COUNTERACCOUT,CODE,KIND,AMOUNT,KIND,REMARKS";

			mFileWithOnlyHeader = new TestFile("FileWithOnlyHeader", header);			

			mFileWithOneRecord = new TestFile("FileWithOneRecord",  header);
			mFileWithOneRecord.AddRecord("\"19780921\",\"Freerk\",\"1234567890\",\"0987654321\",\"A\",\"AF\",\"2109.78\",\"PIN\",\"Remarks\"",
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
			mFileWithMultipleRecords.AddRecord("\"19780921\",\"Freerk\",\"1234567890\",\"0987654321\",\"A\",\"AF\",\"2109.78\",\"PIN\",\"Remarks\"",
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

			mFileWithMultipleRecords.AddRecord("\"19790713\",\"Djuke\",\"2345678901\",\"9876543210\",\"B\",\"BIJ\",\"19797.13\",\"ACC\",\"\"",
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
		}

		private DataImporterService_ImportFileTest createUnitUnderTest(ITransactionDataService transactionDataService, ILogFile logFile)
		{
			var retValue = new DataImporterService_ImportFileTest(transactionDataService, logFile);

			retValue.AddTestFile(mFileWithOnlyHeader);
			retValue.AddTestFile(mFileWithOneRecord);
			retValue.AddTestFile(mFileWithMultipleRecords);

			return retValue;
		}


		// Error opening the file
		// Expected: no transactions returned, entry in the log file.
		[Test]
		public void ImportFilesTest_ErrorOpeningTheFile()
		{
			var mockLogFile = Substitute.For<ILogFile>();
			var mockTransactionDataSerice = Substitute.For<ITransactionDataService>();

			var unitUnderTest = createUnitUnderTest(mockTransactionDataSerice, mockLogFile);
			unitUnderTest.ImportFiles(new List<string>() { "InvalidFileName" } );

			mockLogFile.Received(1).Error(Arg.Is<string>(x => x.Contains("Unable to import")));
			mockTransactionDataSerice.DidNotReceive().Add(Arg.Any<Transaction>());
		}

		#endregion ImportFilesTest_ErrorOpeningTheFile


		[Test]
		public void ImportFilesTest_ImportFileWithOnlyHeader()
		{
			var mockLogFile = Substitute.For<ILogFile>();
			var mockTransactionDataSerice = Substitute.For<ITransactionDataService>();

			var unitUnderTest = createUnitUnderTest(mockTransactionDataSerice, mockLogFile);

			unitUnderTest.ImportFiles(new List<string>() { mFileWithOnlyHeader.Name });

			mockLogFile.DidNotReceive().Error(Arg.Any<string>());
			mockTransactionDataSerice.DidNotReceive().Add(Arg.Any<Transaction>());
		}

		[Test]
		public void ImportFilesTest_ImportFileWithOneRecord()
		{
			// Creation and initialization.
			var mockLogFile = Substitute.For<ILogFile>();
			var mockTransactionDataSerice = Substitute.For<ITransactionDataService>();

			mockTransactionDataSerice.Add(Arg.Any<Transaction>()).Returns(true);

			var unitUnderTest = createUnitUnderTest(mockTransactionDataSerice, mockLogFile);
			// Start test
			unitUnderTest.ImportFiles(new List<string>() { mFileWithOneRecord.Name });

			// Check test results
			mockLogFile.DidNotReceive().Error(Arg.Any<string>());
			// Test that the correct transaction has been added.
			mockTransactionDataSerice.Received(1).Add(Arg.Is<Transaction>(t => t.IsEqual(mFileWithOneRecord.GetTransactionByIndex(0))));
		}

		[Test]
		public void ImportFilesTest_ImportFileWithMultipleRecords()
		{
			// Creation and initialization.
			var mockLogFile = Substitute.For<ILogFile>();
			var mockTransactionDataSerice = Substitute.For<ITransactionDataService>();

			mockTransactionDataSerice.Add(Arg.Any<Transaction>()).Returns(true);

			var unitUnderTest = createUnitUnderTest(mockTransactionDataSerice, mockLogFile);
			// Start test
			unitUnderTest.ImportFiles(new List<string>() { mFileWithMultipleRecords.Name });

			// Check test results
			mockLogFile.DidNotReceive().Error(Arg.Any<string>());

			// Test that the correct transaction has been added.
			mockTransactionDataSerice.Received(2).Add(Arg.Any<Transaction>());
			mockTransactionDataSerice.Received(1).Add(Arg.Is<Transaction>(t => t.IsEqual(mFileWithMultipleRecords.GetTransactionByIndex(0))));
			mockTransactionDataSerice.Received(1).Add(Arg.Is<Transaction>(t => t.IsEqual(mFileWithMultipleRecords.GetTransactionByIndex(1))));
		}

		[Test]
		public void ImportFilesTest_TriggeredUpdateImported()
		{
			var stubLogFile = Substitute.For<ILogFile>();
			var stubTransactionDataSerice = Substitute.For<ITransactionDataService>();

			stubTransactionDataSerice.Add(Arg.Any<Transaction>()).Returns(true);

			var unitUnderTest = createUnitUnderTest(stubTransactionDataSerice, stubLogFile);

			unitUnderTest.ImportFiles(new List<string>() { mFileWithMultipleRecords.Name });

			//unitUnderTest.Received(2).OnDataProcessed

		}



		[Test]
		public void ImportFilesTest_ImportCorruptFile()
		{
			Assert.Fail();
		}


		[Test]
		public void ImportFilesTest_ImportMulitpleFiles()
		{
			Assert.Fail();
		}

		[Test]
		public void ImportFilesTest_ImportMultipleFilesIncludingCorrupted()
		{
			Assert.Fail();
		}

		[Test]
		public void ImportFilesTest_ImportMultipleFilesIncludingMissingFile()
		{
			Assert.Fail();
		}
	}
}