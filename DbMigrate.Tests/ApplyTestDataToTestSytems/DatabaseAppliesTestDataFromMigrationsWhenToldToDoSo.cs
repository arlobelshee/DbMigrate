using DbMigrate.Model;
using DbMigrate.Model.Support.Database;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMigrate.Tests.ApplyTestDataToTestSytems
{
	[TestClass]
	public class DatabaseAppliesTestDataFromMigrationsWhenToldToDoSo
	{
		private static readonly MigrationSpecification Migration2 = new MigrationSpecification(2, "2_something", "do 2",
			"undo 2", "insert data 2", "remove data 2");

		private static readonly MigrationSpecification Migration3 = new MigrationSpecification(3, "3_something", "do 3",
			"undo 3", "insert data 3", "remove data 3");

		private static readonly MigrationSpecification MigrationWithoutTestData = new MigrationSpecification(4, "4_something",
			"do 4",
			"undo 4");

		[TestMethod]
		public void InMemoryDatabaseShouldDefaultToBeProductionDatabase()
		{
			var db = new DatabaseLocalMemory();
			db.IsTestDatabase.Should().BeFalse();
		}

		[TestMethod]
		public void RemoteDatabaseShouldDefaultToBeProductionDatabase()
		{
			var db = new DatabaseRemote(new TrannectionTraceOnly());
			db.IsTestDatabase.Should().BeFalse();
		}

		[TestMethod]
		public void ProductionRemoteDatabaseShouldNotApplyTestData()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection);
			testSubject.Apply(Migration2);
			testSubject.Apply(Migration3);
			tranection.SqlExecuted.Should().Equal(new[] {Migration2.Apply, Migration3.Apply});
		}

		[TestMethod]
		public void NonProductionRemoteDatabaseShouldApplyTestData()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection) {IsTestDatabase = true};
			testSubject.Apply(Migration2);
			testSubject.Apply(Migration3);
			tranection.SqlExecuted.Should().Equal(new[]
				{Migration2.Apply, Migration2.InsertTestData, Migration3.Apply, Migration3.InsertTestData});
		}

		[TestMethod]
		public void ShouldNeverInsertTestDataWhichIsNoOp()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection) {IsTestDatabase = true};
			testSubject.Apply(MigrationWithoutTestData);
			tranection.SqlExecuted.Should().Equal(new[] {MigrationWithoutTestData.Apply});
		}

		[TestMethod]
		public void ProductionRemoteDatabaseShouldNotRemoveTestData()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection);
			testSubject.Unapply(Migration3);
			testSubject.Unapply(Migration2);
			tranection.SqlExecuted.Should().Equal(new[] {Migration3.Unapply, Migration2.Unapply});
		}

		[TestMethod]
		public void NonProductionRemoteDatabaseShouldRemoveTestData()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection) {IsTestDatabase = true};
			testSubject.Unapply(Migration3);
			testSubject.Unapply(Migration2);
			tranection.SqlExecuted.Should().Equal(new[]
				{Migration3.DeleteTestData, Migration3.Unapply, Migration2.DeleteTestData, Migration2.Unapply});
		}

		[TestMethod]
		public void ShouldNeverDeleteTestDataWhichIsNoOp()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection) {IsTestDatabase = true};
			testSubject.Unapply(MigrationWithoutTestData);
			tranection.SqlExecuted.Should().Equal(new[] {MigrationWithoutTestData.Unapply});
		}
	}
}