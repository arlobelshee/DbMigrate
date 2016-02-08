using System;
using DbMigrate.Model.Support.Database;

namespace DbMigrate.Model
{
    public class Target : IDisposable
    {
        public Target(string connectionString, bool isTestDatabase)
            : this(new DatabaseRemote(connectionString) {IsTestDatabase = isTestDatabase})
        {
        }

        public Target(IDatabase database)
        {
            this.Database = database;
        }

        public IDatabase Database { get; private set; }

        public void Dispose()
        {
            this.Database.Dispose();
        }

        public ChangePlanner MigrateTo(int? targetVersion)
        {
            return new ChangePlanner(this.Database, FigureOutTheGoal(this.Database, targetVersion));
        }

        public static ChangeGoal FigureOutTheGoal(IDatabase database, int? targetVersion)
        {
            return new ChangeGoal(database.CurrentVersion.Result, targetVersion);
        }
    }
}