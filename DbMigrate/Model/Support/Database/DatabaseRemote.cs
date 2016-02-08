using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbMigrate.Model.Support.Database
{
    public class DatabaseRemote : IDatabase
    {
        private const string RequestVersionSql =
            @"if exists(select * from sys.objects where name = '__database_info' and type in ('U'))
begin
    select top 1 version_number from __database_info;
end
else
begin
    select -1;
end";

        // format strings in sql == really bad idea. However, the values aren't coming from a user and aren't
        // strings, so not as bad. And I don't want to take the time right now to support parameters in
        // my fully-encapsulated commands.
        private const string UpdateVersionSqlFormat = "update __database_info set version_number = {0};";

        public DatabaseRemote(string connectionString)
            : this(new SqlServerTranection(connectionString))
        {
        }

        public DatabaseRemote(ITranection tranection)
        {
            this.Tranection = tranection;
        }

        public ITranection Tranection { get; private set; }

        public string ConnectionString
        {
            get { return this.Tranection.ConnectionString; }
        }

        public bool IsTestDatabase { get; set; }

        public Task<int> CurrentVersion
        {
            get { return this.Tranection.ExecuteScalar<int>(RequestVersionSql); }
        }

        public void Commit()
        {
            this.Tranection.Commit();
        }

        public void Dispose()
        {
            this.Tranection.Dispose();
        }

        public Task SetCurrentVersionTo(int targetVersion)
        {
            return this.Tranection.ExecuteNonQuery(UpdateVersionSqlFormat.Format(targetVersion));
        }

        public void Apply(MigrationSpecification migration)
        {
            var tasks = new List<Task>();
            RunSql(tasks, migration.Apply);
            if (IsTestDatabase)
                RunSql(tasks, migration.InsertTestData);
            Task.WaitAll(tasks.ToArray());
        }

        public void Unapply(MigrationSpecification migration)
        {
            var tasks = new List<Task>();
            if (IsTestDatabase)
                RunSql(tasks, migration.DeleteTestData);
            RunSql(tasks, migration.Unapply);
            Task.WaitAll(tasks.ToArray());
        }

        private void RunSql(ICollection<Task> tasks, string sql)
        {
            if (!string.IsNullOrEmpty(sql))
                tasks.Add(this.Tranection.ExecuteNonQuery(sql));
        }
    }
}