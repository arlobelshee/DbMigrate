using DbMigrate.Model;
using DbMigrate.Model.Support.Database;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMigrate.Tests.CommonInfrastructureForAllMmfs
{
	[TestClass]
	public class ConnectToADatabase
	{
		private static string ExpectedConnectionString(string baseConnectionString)
		{
			return baseConnectionString + "Asynchronous Processing=True;MultipleActiveResultSets=true;";
		}

		[TestMethod]
		public void DatabaseShouldCommitItsTranection()
		{
			var tracer = new TrannectionTraceOnly();
			var testSubject = new DatabaseRemote(tracer);
			testSubject.Commit();
			tracer.IsCommitted.Should().BeTrue();
		}

		[TestMethod]
		public void DatabaseShouldCreateATranection()
		{
			const string connectionString = "some fake connection string;";
			var testSubject = new DatabaseRemote(connectionString);
			testSubject.Tranection.Should().BeOfType<SqlServerTranection>();
			testSubject.Tranection.ShouldHave()
				.Properties(t => t.IsOpen, t => t.ConnectionString)
				.EqualTo(new {IsOpen = false, ConnectionString = ExpectedConnectionString(connectionString)});
		}

		[TestMethod]
		public void DatabaseShouldDisposeItsTranection()
		{
			var tracer = new TrannectionTraceOnly();
			var testSubject = new DatabaseRemote(tracer);
			testSubject.Dispose();
			tracer.IsDisposed.Should().BeTrue();
		}

		[TestMethod]
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

		[TestMethod]
		public void TargetShouldConnectToNonProductionDatabasesCorrectly()
		{
			const string connectionString = "some fake connection string;";
			var testSubject = new Target(connectionString, true);
			testSubject.Database.Should().BeOfType<DatabaseRemote>();
			testSubject.Database.IsTestDatabase.Should().BeTrue();
		}

		[TestMethod]
		public void TargetShouldDisposeItsDatabase()
		{
			var tracer = new DatabaseLocalMemory();
			var testSubject = new Target(tracer);
			testSubject.Dispose();
			tracer.IsDisposed.Should().BeTrue();
		}
	}
}