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
			var testSubject = new DatabaseRemote(tracer);
			testSubject.Commit();
			tracer.IsCommitted.Should().BeTrue();
		}

		[Test]
		public void DatabaseShouldCreateATranection()
		{
			const string connectionString = "some fake connection string;";
			var testSubject = new DatabaseRemote(connectionString);
			testSubject.Tranection.Should().BeOfType<DbTranection>();
			testSubject.Tranection.ShouldHave()
				.Properties(t => t.IsOpen, t => t.ConnectionString)
				.EqualTo(new {IsOpen = false, ConnectionString = ExpectedConnectionString(connectionString)});
		}

		[Test]
		public void DatabaseShouldDisposeItsTranection()
		{
			var tracer = new TrannectionTraceOnly();
			var testSubject = new DatabaseRemote(tracer);
			testSubject.Dispose();
			tracer.IsDisposed.Should().BeTrue();
		}

		[Test]
		public void TargetShouldCreateADatabase()
		{
			const string connectionString = "some fake connection string;";
			var testSubject = new Target(connectionString, false);
			testSubject.Database.Should().BeOfType<DatabaseRemote>();
			((DatabaseRemote) testSubject.Database).ShouldHave()
				.Properties(t => t.ConnectionString)
				.EqualTo(new {ConnectionString = ExpectedConnectionString(connectionString)});
			testSubject.Database.IsTestDatabase.Should().BeFalse();
		}

		[Test]
		public void TargetShouldConnectToNonProductionDatabasesCorrectly()
		{
			const string connectionString = "some fake connection string;";
			var testSubject = new Target(connectionString, true);
			testSubject.Database.Should().BeOfType<DatabaseRemote>();
			testSubject.Database.IsTestDatabase.Should().BeTrue();
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