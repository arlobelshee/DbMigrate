using DbMigrate.Model;
using DbMigrate.Model.Support.Database;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.CommonInfrastructureForAllMmfs
{
	[TestFixture]
	public class DatabaseAppliesAndUnappliesMigrations
	{
		private static readonly MigrationSpecification Migration2 = new MigrationSpecification(2, "2_something", "do 2",
			"undo 2");

		private static readonly MigrationSpecification Migration3 = new MigrationSpecification(3, "3_something", "do 3",
			"undo 3");

		private static readonly MigrationSpecification MigrationEmpty = new MigrationSpecification(4, "4_something",
			string.Empty, string.Empty);

		[Test]
		public void MemoryDatabaseShouldRecordMigrationsApplied()
		{
			var testSubject = new DatabaseLocalMemory();
			testSubject.BeginUpgrade(Migration2);
			testSubject.BeginUpgrade(Migration3);
			testSubject.AppliedMigrations.Should().Equal(Migration2, Migration3);
		}

		[Test]
		public void MemoryDatabaseShouldRecordMigrationsUnapplied()
		{
			var testSubject = new DatabaseLocalMemory();
			testSubject.BeginDowngrade(Migration3);
			testSubject.BeginDowngrade(Migration2);
			testSubject.UnappliedMigrations.Should().Equal(Migration3, Migration2);
		}

		[Test]
		public void RealDatabaseShouldApplyMigrationsByExecutingSql()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection, DbEngine.None);
			testSubject.BeginUpgrade(Migration2);
			testSubject.BeginUpgrade(Migration3);
			tranection.SqlExecuted.Should().Equal(new[] {Migration2.BeginUp, Migration3.BeginUp});
		}

		[Test]
		public void RealDatabaseShouldNoOpToApplyMigrationsWhichDoNothingWhenApplied()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection, DbEngine.None);
			testSubject.BeginUpgrade(MigrationEmpty);
			tranection.SqlExecuted.Should().BeEmpty();
		}

		[Test]
		public void RealDatabaseShouldNoOpToUnapplyMigrationsWhichDoNothingWhenUnapplied()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection, DbEngine.None);
			testSubject.BeginDowngrade(MigrationEmpty);
			tranection.SqlExecuted.Should().BeEmpty();
		}

		[Test]
		public void RealDatabaseShouldUnapplyMigrationsByExecutingSql()
		{
			var tranection = new TrannectionTraceOnly().BeginCapturing();
			var testSubject = new DatabaseRemote(tranection, DbEngine.None);
			testSubject.BeginDowngrade(Migration3);
			testSubject.BeginDowngrade(Migration2);
			tranection.SqlExecuted.Should().Equal(new[] {Migration3.BeginDown, Migration2.BeginDown});
		}
	}
}