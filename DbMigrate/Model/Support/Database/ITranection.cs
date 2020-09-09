using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DbMigrate.Model.Support.Database
{
	public interface ITranection : IDisposable
	{
		bool IsOpen { get; }
		string ConnectionString { get; }
		Task<T> ExecuteScalar<T>(string sql);
		Task<int> ExecuteNonQuery(string sql);
		Task<T> ExecuteStructure<T>([NotNull] string sql, [NotNull] Func<object[], T> deserialize);
		void Commit();
	}
}