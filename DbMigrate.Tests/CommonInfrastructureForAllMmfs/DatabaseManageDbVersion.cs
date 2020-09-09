﻿using DbMigrate.Model.Support;
using DbMigrate.Model.Support.Database;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace DbMigrate.Tests.CommonInfrastructureForAllMmfs
{
	[TestFixture]
	public class DatabaseManageDbVersion
	{
		private const string CreateVersionInfoTableSql =
            @"create table __database_info(
  min_version_number long,
  max_version_number long
);
insert into __database_info(min_version_number, max_version_number) values(0, 0);";

		private const string UpdateToVersion9Sql = "update __database_info set max_version_number = 9;";
		private const string DropVersionInfoTableSql = "drop table __database_info;";

		[Test]
		public void DatabaseShouldKnowItsMaxVersion()
        {
            var tracer = new TrannectionTraceOnly
            {
                ExecuteStructureHandler = sql =>
                    {
                        sql.Should().Be(DbEngine.SqlLite.RequestVersionSql[0]);
                        return new DatabaseVersion(3L, 6L);
                    }
            };
            using (var testSubject = new DatabaseRemote(tracer, DbEngine.SqlLite))
            {
                testSubject.GetMaxVersion().Result.Should().Be(6L);
            }
        }

        [Test]
		public void DatabaseSimulatorShouldBeAbleToSetItsMaxVersion()
        {
            using (var testSubject = new DatabaseLocalMemory())
            {
                testSubject.SetMaxVersionTo(33).Wait();
                testSubject.GetMaxVersion().Result.Should().Be(33);
            }
        }

        [Test]
		public void DatabaseSimulatorShouldStartVersionUnaware()
        {
            using (var testSubject = new DatabaseLocalMemory())
            {
                testSubject.GetMaxVersion().Result.Should().Be(-1);
            }
        }

        [Test]
		public void ShouldBeAbleToGoToNewVersion()
        {
            var tracer = new TrannectionTraceOnly();
            using (var testSubject = new DatabaseRemote(tracer, DbEngine.None))
            {
                var hasBeenCalled = false;
                tracer.ExecuteNonQueryHandler =
                    sql =>
                    {
                        sql.Should().Be(UpdateToVersion9Sql);
                        hasBeenCalled = true;
                    };
                testSubject.SetMaxVersionTo(9).Wait();
                hasBeenCalled.Should().BeTrue();
            }
        }

        [Test]
		public void SpecialMigrationShouldKnowHowToApply()
		{
			var testSubject = new MigrationRepoMakeDbVersionAware().LoadMigrationIfPresent(0);
			testSubject.Version.Should().Be(0);
			testSubject.InsertTestData.Should().Be(string.Empty);
			testSubject.DeleteTestData.Should().Be(string.Empty);
			testSubject.BeginUp.Should().Be(CreateVersionInfoTableSql);
			testSubject.BeginDown.Should().Be(DropVersionInfoTableSql);
		}
	}
}