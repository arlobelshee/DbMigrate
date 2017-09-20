using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DbMigrate.Model.Support.Database
{
	internal static class CommandExtensions
	{
		public static Task<int> ExecuteNonQueryAnync(this DbCommand command)
		{
			var sqlCommand = (SqlCommand) command;
			return Task<int>.Factory.FromAsync(sqlCommand.BeginExecuteNonQuery(),
				sqlCommand.EndExecuteNonQuery);
		}

		public static Task<SqlDataReader> ExecuteReaderAsync<T>(this DbCommand command)
		{
			var sqlCommand = (SqlCommand) command;
			return Task<SqlDataReader>.Factory.FromAsync(
				sqlCommand.BeginExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow),
				sqlCommand.EndExecuteReader);
		}
	}
}