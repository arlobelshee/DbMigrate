﻿using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using DbMigrate.Model.Support;
using DbMigrate.Model.Support.Database;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.CommonInfrastructureForAllMmfs
{
	public abstract class ExecuteSqlAgainstADatabase
	{
		private const string CreateJunkTable =
			"CREATE TABLE some_junk_table{0} (name varchar(50));";

		private string _tableUid;
		protected string ConnectionStringToUse;
		protected string CountJunkTables;
		protected DbEngine DbToUse;
		protected string DropJunkTable;

		[SetUp]
		public void CommonInit()
		{
			_tableUid = Guid.NewGuid().ToString("N");
		}

		private void MakeJunkTable(DbTranection testSubject)
		{
			testSubject.ExecuteNonQuery(string.Format(CreateJunkTable, _tableUid))
				.Wait();
		}

		private void NumJunkTablesShouldBe(DbTranection testSubject, int expected)
		{
			testSubject.ExecuteScalar<long>(string.Format(CountJunkTables, _tableUid))
				.Result.Should().Be(expected);
		}

		private void RemoveJunkTable(DbTranection testSubject)
		{
			testSubject.ExecuteNonQuery(string.Format(DropJunkTable, _tableUid))
				.Wait();
		}

		[Test]
		public void SqlTrannectionShouldBeCommittable()
		{
			try
			{
				using (var testSubject = new DbTranection(DbToUse, ConnectionStringToUse))
				{
					MakeJunkTable(testSubject);
					NumJunkTablesShouldBe(testSubject, 1);
					testSubject.Commit();
				}
				using (var testSubject = new DbTranection(DbToUse, ConnectionStringToUse))
				{
					NumJunkTablesShouldBe(testSubject, 1);
				}
			}
			finally
			{
				using (var testSubject = new DbTranection(DbToUse, ConnectionStringToUse))
				{
					RemoveJunkTable(testSubject);
					testSubject.Commit();
				}
			}
		}

		[Test]
		public void SqlTrannectionShouldCloseItsConnectionWhenItIsDisposed()
		{
			using (var testSubject = new DbTranection(DbToUse, ConnectionStringToUse))
			{
				testSubject.IsOpen.Should().BeFalse();
				testSubject.ExecuteScalar<long>("select 1;").Result.Should().Be(1);
				testSubject.IsOpen.Should().BeTrue();
				testSubject.Dispose();
				testSubject.IsOpen.Should().BeFalse();
			}
		}

		[Test]
		public void SqlTrannectionShouldExecuteStructuredQuery()
		{
			using (var testSubject = new DbTranection(DbToUse, ConnectionStringToUse))
			{
				testSubject.ExecuteStructure("select 3, 4",
					values => new DatabaseVersion((long)values[0], (long)values[1]))
					.Result.Should().Be(new DatabaseVersion(3, 4));
			}
		}

		[Test]
		public void SqlTrannectionShouldExecuteNonQuery()
		{
			using (var testSubject = new DbTranection(DbToUse, ConnectionStringToUse))
			{
				testSubject.ExecuteNonQuery(string.Format(CreateJunkTable, _tableUid))
					.Wait();
				testSubject.ExecuteScalar<long>(string.Format(CountJunkTables, _tableUid))
					.Result.Should().Be(1);
			}
		}

		[Test]
		public void SqlTrannectionShouldExecuteScalarOnItsTarget()
		{
			using (var testSubject = new DbTranection(DbToUse, ConnectionStringToUse))
			{
				testSubject.ExecuteScalar<long>("select 1;").Result.Should().Be(1);
			}
		}

		[Test]
		public void DatabaseShouldExecuteEachOfTheBuiltInCommandsCorrectly()
		{
			var tranection = new DbTranection(DbToUse, ConnectionStringToUse);
			using (var testSubject = new DatabaseRemote(tranection, DbToUse))
			{
				testSubject.GetVersion().Result.Should().Be(new DatabaseVersion(-1, -1));
                var becomeVersionAware = new MigrationRepoMakeDbVersionAware().LoadMigrationIfPresent(0);
                testSubject.BeginUpgrade(becomeVersionAware);
                testSubject.GetVersion().Result.Should().Be(new DatabaseVersion(0, 0));
                testSubject.SetMaxVersionTo(3);
                testSubject.GetVersion().Result.Should().Be(new DatabaseVersion(0, 3));
            }
		}

		[Test]
		public void SqlTrannectionShouldRollbackUnlessInstructedOtherwise()
		{
			using (var testSubject = new DbTranection(DbToUse, ConnectionStringToUse))
			{
				MakeJunkTable(testSubject);
				NumJunkTablesShouldBe(testSubject, 1);
			}
			using (var testSubject = new DbTranection(DbToUse, ConnectionStringToUse))
			{
				NumJunkTablesShouldBe(testSubject, 0);
			}
		}
	}

	[TestFixture]
	public class ExecuteSqlAgainstSqlLite : ExecuteSqlAgainstADatabase
	{
		[SetUp]
		public void Init()
		{
			DbToUse = DbEngine.SqlLite;
			ConnectionStringToUse = "Data Source=test_db.sqlite;Version=3;New=true;";
			DropJunkTable = "drop table if exists some_junk_table{0};";
			CountJunkTables = "select count(*) from sqlite_master where type='table' and name='some_junk_table{0}' collate nocase;";
		}
	}

	[TestFixture]
	[Ignore("Turn on if you have Sql Server Express installed locally.")]
	public class ExecuteSqlAgainstSqlServer : ExecuteSqlAgainstADatabase
	{
		[SetUp]
		public void Init()
		{
			DbToUse = DbEngine.SqlServer;
			ConnectionStringToUse = "Server=.\\SQLEXPRESS;Database=master;Trusted_Connection=True;";
			DropJunkTable =
				@"if exists(select * from sys.objects o where o.name = 'some_junk_table{0}')
drop table some_junk_table{0};";
			CountJunkTables = "select COUNT(*) from sys.objects o where o.name = 'some_junk_table{0}';";
		}
	}
}