using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DbMigrate.Model.Support.Database
{
	public class SqlServerTranection : ITranection
	{
		private DbConnection _connection;
		private DbTransaction _transaction;

		public SqlServerTranection(string connectionString)
		{
			ConnectionString = connectionString + "Asynchronous Processing=True;MultipleActiveResultSets=true;";
		}

		public void Dispose()
		{
			_transaction?.Rollback();
			_transaction = null;
			_connection?.Close();
			_connection = null;
		}

		public bool IsOpen => _connection != null;

		public string ConnectionString { get; }

		public Task<T> ExecuteScalar<T>(string sql)
		{
			var command = GetCommand(sql);
			return command.ExecuteReaderAsync()
					.ContinueWith(t =>
					{
						command.Dispose(); // before attempting to check the result, in case there was an exception.
						return ReadScalar<T>(t.Result);
					});
		}

		public Task<int> ExecuteNonQuery(string sql)
		{
			var command = GetCommand(sql);
			return command.ExecuteNonQueryAnync()
				.ContinueWith(numRows =>
				{
					command.Dispose();
					return numRows.Result;
				});
		}

		public void Commit()
		{
			if (!IsOpen) return;
			_transaction.Commit();
			_transaction = _connection.BeginTransaction(IsolationLevel.Serializable);
		}

		private void EnsureIsOpen()
		{
			if (IsOpen) return;
			_connection = new SqlConnection(ConnectionString);
			_connection.Open();
			_transaction = _connection.BeginTransaction(IsolationLevel.Serializable);
		}

		private DbCommand GetCommand(string sql)
		{
			EnsureIsOpen();
			var command = _connection.CreateCommand();
			command.CommandText = sql;
			command.CommandType = CommandType.Text;
			command.Transaction = _transaction;
			return command;
		}

		private static T ReadScalar<T>(IDataReader reader)
		{
			reader.Read();
			var result = (T) reader.GetValue(0);
			reader.Close();
			return result;
		}
	}
}