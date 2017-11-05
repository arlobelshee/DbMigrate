using DbMigrate.Model.Support;
using DbMigrate.Model.Support.Database;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.CommonInfrastructureForAllMmfs
{
	[TestFixture]
	public class DatabaseManageDbVersion
	{
		private const string CreateVersionInfoTableSql =
			@"create table __database_info(
  version_number int
);
insert into __database_info(version_number) values(0);";

		private const string UpdateToVersion9Sql = "update __database_info set version_number = 9;";
		private const string DropVersionInfoTableSql = "drop table __database_info;";

		[Test]
		public void DatabaseShouldKnowItsCurrentVersion()
		{
			var tracer = new TrannectionTraceOnly();
			var testSubject = new DatabaseRemote(tracer, DbEngine.SqlLite);
			tracer.ExecuteScalarHandler =
				sql =>
				{
					sql.Should().Be(DbEngine.SqlLite.RequestVersionSql);
					return 6;
				};
			testSubject.CurrentVersion.Result.Should().Be(6);
		}

		[Test]
		public void DatabaseSimulatorShouldBeAbleToSetItsVersion()
		{
			var testSubject = new DatabaseLocalMemory();
			testSubject.SetCurrentVersionTo(33).Wait();
			testSubject.CurrentVersion.Result.Should().Be(33);
		}

		[Test]
		public void DatabaseSimulatorShouldStartVersionUnaware()
		{
			var testSubject = new DatabaseLocalMemory();
			testSubject.CurrentVersion.Result.Should().Be(-1);
		}

		[Test]
		public void ShouldBeAbleToGoToNewVersion()
		{
			var tracer = new TrannectionTraceOnly();
			var testSubject = new DatabaseRemote(tracer, DbEngine.None);
			var hasBeenCalled = false;
			tracer.ExecuteNonQueryHandler =
				sql =>
				{
					sql.Should().Be(UpdateToVersion9Sql);
					hasBeenCalled = true;
				};
			testSubject.SetCurrentVersionTo(9).Wait();
			hasBeenCalled.Should().BeTrue();
		}

		[Test]
		public void SpecialMigrationShouldKnowHowToApply()
		{
			var testSubject = new MigrationRepoMakeDbVersionAware().LoadMigrationIfPresent(0);
			testSubject.Version.Should().Be(0);
			testSubject.InsertTestData.Should().Be(string.Empty);
			testSubject.DeleteTestData.Should().Be(string.Empty);
			testSubject.Apply.Should().Be(CreateVersionInfoTableSql);
			testSubject.Unapply.Should().Be(DropVersionInfoTableSql);
		}
	}
}