using DbMigrate.Model;
using DbMigrate.Model.Support.Database;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.ApplyTestDataToTestSytems
{
	[TestFixture]
	public class DatabaseAppliesTestDataFromMigrationsWhenToldToDoSo
	{
		private static readonly MigrationSpecification Migration2 = new MigrationSpecification(2, "2_something",
			"start do 2", "finish do 2",
			"start undo 2", "finish undo 2",
			"insert data 2", "remove data 2");

		private static readonly MigrationSpecification Migration3 = new MigrationSpecification(3, "2_something",
			"do 3", "",
			"undo 3", "",
			"insert data 3", "remove data 3");

		private static readonly MigrationSpecification MigrationWithoutTestData = new MigrationSpecification(4, "4_something",
			"do 4",
			"undo 4");

		[Test]
		public void InMemoryDatabaseShouldDefaultToBeProductionDatabase()
		{
			var db = new DatabaseLocalMemory();
			db.IsTestDatabase.Should().BeFalse();
		}

		[Test]
		public void RemoteDatabaseShouldDefaultToBeProductionDatabase()
		{
			var db = new DatabaseRemote(new TrannectionTraceOnly(), DbEngine.None);
			db.IsTestDatabase.Should().BeFalse();
		}

		[Test]
		public void ProductionRemoteDatabaseShouldNotApplyTestData()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection, DbEngine.None);
			testSubject.Apply(Migration2);
			testSubject.Apply(Migration3);
			tranection.SqlExecuted.Should().Equal(new[] {Migration2.BeginUp, Migration3.BeginUp});
		}

		[Test]
		public void NonProductionRemoteDatabaseShouldApplyTestData()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection, DbEngine.None) {IsTestDatabase = true};
			testSubject.Apply(Migration2);
			testSubject.Apply(Migration3);
			tranection.SqlExecuted.Should().Equal(new[]
				{Migration2.BeginUp, Migration2.InsertTestData, Migration3.BeginUp, Migration3.InsertTestData});
		}

		[Test]
		public void ShouldNeverInsertTestDataWhichIsNoOp()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection, DbEngine.None) {IsTestDatabase = true};
			testSubject.Apply(MigrationWithoutTestData);
			tranection.SqlExecuted.Should().Equal(new[] {MigrationWithoutTestData.BeginUp});
		}

		[Test]
		public void ProductionRemoteDatabaseShouldNotRemoveTestData()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection, DbEngine.None);
			testSubject.Unapply(Migration3);
			testSubject.Unapply(Migration2);
			tranection.SqlExecuted.Should().Equal(new[] {Migration3.BeginDown, Migration2.BeginDown});
		}

		[Test]
		public void NonProductionRemoteDatabaseShouldRemoveTestData()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection, DbEngine.None) {IsTestDatabase = true};
			testSubject.Unapply(Migration3);
			testSubject.Unapply(Migration2);
			tranection.SqlExecuted.Should().Equal(new[]
				{Migration3.DeleteTestData, Migration3.BeginDown, Migration2.DeleteTestData, Migration2.BeginDown});
		}

		[Test]
		public void ShouldNeverDeleteTestDataWhichIsNoOp()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection, DbEngine.None) {IsTestDatabase = true};
			testSubject.Unapply(MigrationWithoutTestData);
			tranection.SqlExecuted.Should().Equal(new[] {MigrationWithoutTestData.BeginDown});
		}
	}
}