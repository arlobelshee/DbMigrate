using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DbMigrate.Model.Support.Database
{
	public class DatabaseRemote : IDatabase
	{
		// format strings in sql == really bad idea. However, the values aren't coming from a user and aren't
		// strings, so not as bad. And I don't want to take the time right now to support parameters in
		// my fully-encapsulated commands.
		private const string UpdateVersionSqlFormat = "update __database_info set {0}_version_number = {1};";

		private readonly DbEngine _dbEngine;

		public DatabaseRemote(DbEngine dbEngine, string connectionString)
			: this(new DbTranection(dbEngine, connectionString), dbEngine)
		{
		}

		public DatabaseRemote(ITranection tranection, DbEngine dbEngine)
		{
			Tranection = tranection;
			_dbEngine = dbEngine;
		}

		public ITranection Tranection { get; }

		public string ConnectionString => Tranection.ConnectionString;

		public bool IsTestDatabase { get; set; }

        public async Task<DatabaseVersion> GetVersion()
        {
            foreach (var attempt in _dbEngine.RequestVersionSql)
            {
                var result = await Tranection.ExecuteStructure(attempt, values => new DatabaseVersion((long)values[0], (long)values[1]));
				if (result.IsKnown) return result;
            }
			return new DatabaseVersion(DatabaseVersion.NOT_YET_KNOWN, DatabaseVersion.NOT_YET_KNOWN);
        }

        public void Commit()
		{
			Tranection.Commit();
		}

		public void Dispose()
		{
			Tranection.Dispose();
			GC.SuppressFinalize(this);
		}

		public Task SetMaxVersionTo(long targetVersion)
		{
			return Tranection.ExecuteNonQuery(UpdateVersionSqlFormat.Format("max", targetVersion));
		}

		public void BeginUpgrade(MigrationSpecification migration)
		{
			var tasks = new List<Task>();
			RunSql(tasks, migration.BeginUp);
			if (IsTestDatabase)
				RunSql(tasks, migration.InsertTestData);
			Task.WaitAll(tasks.ToArray());
		}

		public void BeginDowngrade(MigrationSpecification migration)
		{
			var tasks = new List<Task>();
			if (IsTestDatabase)
				RunSql(tasks, migration.DeleteTestData);
			RunSql(tasks, migration.BeginDown);
			Task.WaitAll(tasks.ToArray());
		}

		private void RunSql(ICollection<Task> tasks, string sql)
		{
			if (!string.IsNullOrEmpty(sql))
				tasks.Add(Tranection.ExecuteNonQuery(sql));
		}
	}
}