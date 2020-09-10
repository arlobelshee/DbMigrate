using DbMigrate.Model.Support;
using System;
using System.Threading.Tasks;

namespace DbMigrate.Model
{
	public interface IDatabase : IDisposable
	{
        Task<DatabaseVersion> GetVersion();

        bool IsTestDatabase { get; set; }
		void Commit();
		Task SetMinVersionTo(long targetVersion);
		Task SetMaxVersionTo(long targetVersion);
		void BeginUpgrade(MigrationSpecification migration);
		void BeginDowngrade(MigrationSpecification migration);
	}
}