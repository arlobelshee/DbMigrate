using System;
using System.Threading.Tasks;

namespace DbMigrate.Model
{
	public interface IDatabase : IDisposable
	{
        Task<long> GetMaxVersion();

        bool IsTestDatabase { get; set; }
		void Commit();
		Task SetMaxVersionTo(long targetVersion);
		void BeginUpgrade(MigrationSpecification migration);
		void BeginDowngrade(MigrationSpecification migration);
	}
}