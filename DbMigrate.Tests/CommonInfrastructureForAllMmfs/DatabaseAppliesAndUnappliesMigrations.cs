using DbMigrate.Model;
using DbMigrate.Model.Support.Database;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMigrate.Tests.CommonInfrastructureForAllMmfs
{
	[TestClass]
	public class DatabaseAppliesAndUnappliesMigrations
	{
		private static readonly MigrationSpecification Migration2 = new MigrationSpecification(2, "2_something", "do 2",
			"undo 2");

		private static readonly MigrationSpecification Migration3 = new MigrationSpecification(3, "3_something", "do 3",
			"undo 3");

		private static readonly MigrationSpecification MigrationEmpty = new MigrationSpecification(4, "4_something",
			string.Empty, string.Empty);

		[TestMethod]
		public void MemoryDatabaseShouldRecordMigrationsApplied()
		{
			var testSubject = new DatabaseLocalMemory();
			testSubject.Apply(Migration2);
			testSubject.Apply(Migration3);
			testSubject.AppliedMigrations.Should().Equal(Migration2, Migration3);
		}

		[TestMethod]
		public void MemoryDatabaseShouldRecordMigrationsUnapplied()
		{
			var testSubject = new DatabaseLocalMemory();
			testSubject.Unapply(Migration3);
			testSubject.Unapply(Migration2);
			testSubject.UnappliedMigrations.Should().Equal(Migration3, Migration2);
		}

		[TestMethod]
		public void RealDatabaseShouldApplyMigrationsByExecutingSql()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection);
			testSubject.Apply(Migration2);
			testSubject.Apply(Migration3);
			tranection.SqlExecuted.Should().Equal(new[] {Migration2.Apply, Migration3.Apply});
		}

		[TestMethod]
		public void RealDatabaseShouldNoOpToApplyMigrationsWhichDoNothingWhenApplied()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection);
			testSubject.Apply(MigrationEmpty);
			tranection.SqlExecuted.Should().BeEmpty();
		}

		[TestMethod]
		public void RealDatabaseShouldNoOpToUnapplyMigrationsWhichDoNothingWhenUnapplied()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection);
			testSubject.Unapply(MigrationEmpty);
			tranection.SqlExecuted.Should().BeEmpty();
		}

		[TestMethod]
		public void RealDatabaseShouldUnapplyMigrationsByExecutingSql()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection);
			testSubject.Unapply(Migration3);
			testSubject.Unapply(Migration2);
			tranection.SqlExecuted.Should().Equal(new[] {Migration3.Unapply, Migration2.Unapply});
		}
	}
}