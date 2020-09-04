using System;
using DbMigrate.Model.Support.Database;

namespace DbMigrate.Model
{
	public class Target : IDisposable
	{
		public Target(DbEngine dbEngine, string connectionString, bool isTestDatabase)
			: this(new DatabaseRemote(dbEngine, connectionString) {IsTestDatabase = isTestDatabase})
		{
		}

		public Target(IDatabase database)
		{
			Database = database;
		}

		public IDatabase Database { get; }

        public void Dispose()
        {
            Database.Dispose();
            GC.SuppressFinalize(this);
        }

        public ChangePlanner MigrateTo(int? targetVersion)
		{
			return new ChangePlanner(Database, FigureOutTheGoal(Database, targetVersion));
		}

		public static ChangeGoal FigureOutTheGoal(IDatabase database, int? targetVersion)
		{
			return new ChangeGoal(database.MaxVersion.Result, targetVersion);
		}
	}
}