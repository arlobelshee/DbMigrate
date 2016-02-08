using System;
using System.Threading.Tasks;

namespace DbMigrate.Model.Support.Database
{
    public interface ITranection : IDisposable
    {
        bool IsOpen { get; }
        string ConnectionString { get; }
        Task<T> ExecuteScalar<T>(string sql);
        Task<int> ExecuteNonQuery(string sql);
        void Commit();
    }
}