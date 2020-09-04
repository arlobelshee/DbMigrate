using System;
using System.Threading.Tasks;

namespace DbMigrate.Model
{
	public interface IDatabase : IDisposable
	{
		Task<int> MaxVersion { get; }
		bool IsTestDatabase { get; set; }
		void Commit();
		Task SetMaxVersionTo(int targetVersion);
		void BeginUpgrade(MigrationSpecification migration);
		void BeginDowngrade(MigrationSpecification migration);
	}
}