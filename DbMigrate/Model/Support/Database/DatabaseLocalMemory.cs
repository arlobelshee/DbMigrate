using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DbMigrate.Util;

namespace DbMigrate.Model.Support.Database
{
	public class DatabaseLocalMemory : IDatabase
	{
		public DatabaseLocalMemory()
		{
			IsDisposed = false;
            SetMaxVersion((-1L).ToTask());
			AppliedMigrations = new List<MigrationSpecification>();
			UnappliedMigrations = new List<MigrationSpecification>();
		}

		public bool IsDisposed { get; private set; }
		public List<MigrationSpecification> AppliedMigrations { get; }
		public List<MigrationSpecification> UnappliedMigrations { get; }
		public bool CommittedTheChanges { get; private set; }

        private Task<long> maxVersion;

        public Task<long> GetMaxVersion()
        {
            return maxVersion;
        }

        private void SetMaxVersion(Task<long> value)
        {
            maxVersion = value;
        }

        public bool IsTestDatabase { get; set; }

		public void Dispose()
		{
			IsDisposed = true;
			GC.SuppressFinalize(this);
		}

		public void Commit()
		{
			if (AppliedMigrations.Count + UnappliedMigrations.Count > 0) CommittedTheChanges = true;
		}

		public Task SetMaxVersionTo(long targetVersion)
		{
            SetMaxVersion(targetVersion.ToTask());
			return HelperMethods.NoOpAction();
		}

		public void BeginUpgrade(MigrationSpecification migration)
		{
			AppliedMigrations.Add(migration);
		}

		public void BeginDowngrade(MigrationSpecification migration)
		{
			UnappliedMigrations.Add(migration);
		}
	}
}