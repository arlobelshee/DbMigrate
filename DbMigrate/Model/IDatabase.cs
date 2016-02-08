using System;
using System.Threading.Tasks;

namespace DbMigrate.Model
{
    public interface IDatabase : IDisposable
    {
        Task<int> CurrentVersion { get; }
        bool IsTestDatabase { get; set; }
        void Commit();
        Task SetCurrentVersionTo(int targetVersion);
        void Apply(MigrationSpecification migration);
        void Unapply(MigrationSpecification migration);
    }
}