using System.Collections.Generic;
using System.Threading.Tasks;
using DbMigrate.Util;

namespace DbMigrate.Model.Support.Database
{
    public class DatabaseLocalMemory : IDatabase
    {
        public DatabaseLocalMemory()
        {
            this.IsDisposed = false;
            this.CurrentVersion = (-1).ToTask();
            this.AppliedMigrations = new List<MigrationSpecification>();
            this.UnappliedMigrations = new List<MigrationSpecification>();
        }

        public bool IsDisposed { get; private set; }
        public List<MigrationSpecification> AppliedMigrations { get; private set; }
        public List<MigrationSpecification> UnappliedMigrations { get; private set; }
        public bool CommittedTheChanges { get; private set; }
        public Task<int> CurrentVersion { get; private set; }
        public bool IsTestDatabase { get; set; }

        public void Dispose()
        {
            this.IsDisposed = true;
        }

        public void Commit()
        {
            if (this.AppliedMigrations.Count + this.UnappliedMigrations.Count > 0) this.CommittedTheChanges = true;
        }

        public Task SetCurrentVersionTo(int targetVersion)
        {
            this.CurrentVersion = targetVersion.ToTask();
            return HelperMethods.NoOpAction();
        }

        public void Apply(MigrationSpecification migration)
        {
            this.AppliedMigrations.Add(migration);
        }

        public void Unapply(MigrationSpecification migration)
        {
            this.UnappliedMigrations.Add(migration);
        }
    }
}