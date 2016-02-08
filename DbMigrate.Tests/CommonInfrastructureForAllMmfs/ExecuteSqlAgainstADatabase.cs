using System;
using DbMigrate.Model.Support.Database;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMigrate.Tests.CommonInfrastructureForAllMmfs
{
    [TestClass]
    public class ExecuteSqlAgainstADatabase
    {
        [TestInitialize]
        public void Init()
        {
            this._tableUid = Guid.NewGuid().ToString("N");
        }

        private const string LocalMasterDb = "Server=.\\SQLEXPRESS;Database=master;Trusted_Connection=True;";

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

        private void MakeJunkTable(SqlServerTranection testSubject)
        {
            testSubject.ExecuteNonQuery(string.Format(CreateJunkTable, this._tableUid))
                .Wait();
        }

        private void NumJunkTablesShouldBe(SqlServerTranection testSubject, int expected)
        {
            testSubject.ExecuteScalar<int>(string.Format(CountJunkTables, this._tableUid))
                .Result.Should().Be(expected);
        }

        private void RemoveJunkTable(SqlServerTranection testSubject)
        {
            testSubject.ExecuteNonQuery(string.Format(DropJunkTable, this._tableUid))
                .Wait();
        }

        [TestMethod]
        public void SqlTrannectionShouldBeCommittable()
        {
            try
            {
                using (var testSubject = new SqlServerTranection(LocalMasterDb))
                {
                    this.MakeJunkTable(testSubject);
                    this.NumJunkTablesShouldBe(testSubject, 1);
                    testSubject.Commit();
                }
                using (var testSubject = new SqlServerTranection(LocalMasterDb))
                {
                    this.NumJunkTablesShouldBe(testSubject, 1);
                }
            }
            finally
            {
                using (var testSubject = new SqlServerTranection(LocalMasterDb))
                {
                    this.RemoveJunkTable(testSubject);
                    testSubject.Commit();
                }
            }
        }

        [TestMethod]
        public void SqlTrannectionShouldCloseItsConnectionWhenItIsDisposed()
        {
            using (var testSubject = new SqlServerTranection(LocalMasterDb))
            {
                testSubject.IsOpen.Should().BeFalse();
                testSubject.ExecuteScalar<int>("select count(*) from sys.objects;").Result.Should().BeGreaterThan(10);
                testSubject.IsOpen.Should().BeTrue();
                testSubject.Dispose();
                testSubject.IsOpen.Should().BeFalse();
            }
        }

        [TestMethod]
        public void SqlTrannectionShouldExecuteNonQuery()
        {
            using (var testSubject = new SqlServerTranection(LocalMasterDb))
            {
                testSubject.ExecuteNonQuery(string.Format(CreateJunkTable, this._tableUid))
                    .Wait();
                testSubject.ExecuteScalar<int>(string.Format(CountJunkTables, this._tableUid))
                    .Result.Should().Be(1);
            }
        }

        [TestMethod]
        public void SqlTrannectionShouldExecuteSqlOnItsTarget()
        {
            using (var testSubject = new SqlServerTranection(LocalMasterDb))
            {
                testSubject.ExecuteScalar<int>("select count(*) from sys.objects;").Result.Should().BeGreaterThan(10);
            }
        }

        [TestMethod]
        public void SqlTrannectionShouldRollbackUnlessInstructedOtherwise()
        {
            using (var testSubject = new SqlServerTranection(LocalMasterDb))
            {
                this.MakeJunkTable(testSubject);
                this.NumJunkTablesShouldBe(testSubject, 1);
            }
            using (var testSubject = new SqlServerTranection(LocalMasterDb))
            {
                this.NumJunkTablesShouldBe(testSubject, 0);
            }
        }
    }
}