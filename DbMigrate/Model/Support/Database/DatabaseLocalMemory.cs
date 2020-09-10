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
			version = new DatabaseVersion(-1L, -1L);
			AppliedMigrations = new List<MigrationSpecification>();
			UnappliedMigrations = new List<MigrationSpecification>();
		}

		public bool IsDisposed { get; private set; }
		public List<MigrationSpecification> AppliedMigrations { get; }
		public List<MigrationSpecification> UnappliedMigrations { get; }
		public bool CommittedTheChanges { get; private set; }

        private DatabaseVersion version;

        public Task<DatabaseVersion> GetVersion()
        {
            return version.ToTask();
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

		public Task SetMinVersionTo(long targetVersion)
		{
			version = version.WithMin(targetVersion);
			return HelperMethods.NoOpAction();
		}

		public Task SetMaxVersionTo(long targetVersion)
		{
			version = version.WithMax(targetVersion);
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