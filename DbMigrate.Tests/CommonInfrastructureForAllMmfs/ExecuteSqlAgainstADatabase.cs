using System;
using DbMigrate.Model.Support.Database;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.CommonInfrastructureForAllMmfs
{
	public abstract class ExecuteSqlAgainstADatabase
	{
		private const string CreateJunkTable =
			@"CREATE TABLE some_junk_table{0} (
    Id [int] IDENTITY(1,1) NOT NULL,
    name [nvarchar](50) NOT NULL
 CONSTRAINT [PK_JunkTable{0}] PRIMARY KEY CLUSTERED (Id ASC)
);
";

		private const string DropJunkTable =
			@"if exists(select * from sys.objects o where o.name = 'some_junk_table{0}')
drop table some_junk_table{0};";

		private const string CountJunkTables = "select COUNT(*) from sys.objects o where o.name = 'some_junk_table{0}';";

		private string _tableUid;
		protected string ConnectionStringToUse;
		protected DbEngine DbToUse;

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
			testSubject.ExecuteScalar<int>(string.Format(CountJunkTables, _tableUid))
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
				testSubject.ExecuteScalar<int>("select count(*) from sys.objects;").Result.Should().BeGreaterThan(10);
				testSubject.IsOpen.Should().BeTrue();
				testSubject.Dispose();
				testSubject.IsOpen.Should().BeFalse();
			}
		}

		[Test]
		public void SqlTrannectionShouldExecuteNonQuery()
		{
			using (var testSubject = new DbTranection(DbToUse, ConnectionStringToUse))
			{
				testSubject.ExecuteNonQuery(string.Format(CreateJunkTable, _tableUid))
					.Wait();
				testSubject.ExecuteScalar<int>(string.Format(CountJunkTables, _tableUid))
					.Result.Should().Be(1);
			}
		}

		[Test]
		public void SqlTrannectionShouldExecuteSqlOnItsTarget()
		{
			using (var testSubject = new DbTranection(DbToUse, ConnectionStringToUse))
			{
				testSubject.ExecuteScalar<int>("select count(*) from sys.objects;").Result.Should().BeGreaterThan(10);
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
			ConnectionStringToUse = "Data Source=:memory:;Version=3;New=true;";
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
		}
	}
}