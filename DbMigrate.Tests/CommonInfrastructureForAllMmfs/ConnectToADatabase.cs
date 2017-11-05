using DbMigrate.Model;
using DbMigrate.Model.Support.Database;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.CommonInfrastructureForAllMmfs
{
	[TestFixture]
	public class ConnectToADatabase
	{
		private static string ExpectedConnectionString(string baseConnectionString)
		{
			return baseConnectionString + "Asynchronous Processing=True;MultipleActiveResultSets=true;";
		}

		[Test]
		public void DatabaseShouldCommitItsTranection()
		{
			var tracer = new TrannectionTraceOnly();
			var testSubject = new DatabaseRemote(tracer, DbEngine.None);
			testSubject.Commit();
			tracer.IsCommitted.Should().BeTrue();
		}

		[Test]
		public void DatabaseShouldCreateATranection()
		{
			const string connectionString = "some fake connection string;";
			var testSubject = new DatabaseRemote(DbEngine.None, connectionString);
			testSubject.Tranection.Should().BeOfType<DbTranection>();
			testSubject.Tranection.ShouldBeEquivalentTo(
				new {IsOpen = false, ConnectionString = ExpectedConnectionString(connectionString)},
				options => options.ExcludingMissingMembers());
		}

		[Test]
		public void DatabaseShouldDisposeItsTranection()
		{
			var tracer = new TrannectionTraceOnly();
			var testSubject = new DatabaseRemote(tracer, DbEngine.None);
			testSubject.Dispose();
			tracer.IsDisposed.Should().BeTrue();
		}

		[Test]
		public void TargetShouldConnectToNonProductionDatabasesCorrectly()
		{
			const string connectionString = "some fake connection string;";
			var testSubject = new Target(DbEngine.None, connectionString, true);
			testSubject.Database.Should().BeOfType<DatabaseRemote>();
			testSubject.Database.IsTestDatabase.Should().BeTrue();
		}

		[Test]
		public void TargetShouldCreateADatabase()
		{
			const string connectionString = "some fake connection string;";
			var testSubject = new Target(DbEngine.None, connectionString, false);
			testSubject.Database.Should().BeOfType<DatabaseRemote>();
			((DatabaseRemote) testSubject.Database).ShouldBeEquivalentTo(
				new {ConnectionString = ExpectedConnectionString(connectionString)}, options => options.ExcludingMissingMembers());
			testSubject.Database.IsTestDatabase.Should().BeFalse();
		}

		[Test]
		public void TargetShouldDisposeItsDatabase()
		{
			var tracer = new DatabaseLocalMemory();
			var testSubject = new Target(tracer);
			testSubject.Dispose();
			tracer.IsDisposed.Should().BeTrue();
		}
	}
}