using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DbMigrate.Model.Support.Database
{
    public class SqlServerTranection : ITranection
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public SqlServerTranection(string connectionString)
        {
            this.ConnectionString = connectionString + "Asynchronous Processing=True;MultipleActiveResultSets=true;";
        }

        public void Dispose()
        {
            if (!this.IsOpen) return;
            this._transaction.Rollback();
            this._connection.Close();
            this._transaction = null;
            this._connection = null;
        }

        public bool IsOpen
        {
            get { return this._connection != null; }
        }

        public string ConnectionString { get; private set; }

        public Task<T> ExecuteScalar<T>(string sql)
        {
            var command = this.GetCommand(sql);
            return
                Task<SqlDataReader>.Factory.FromAsync(
                    command.BeginExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow), command.EndExecuteReader)
                    .ContinueWith(t =>
                                      {
                                          command.Dispose(); // before attempting to check the result, in case there was an exception.
                                          return ReadScalar<T>(t.Result);
                                      });
        }

        public Task<int> ExecuteNonQuery(string sql)
        {
            var command = this.GetCommand(sql);
            return Task<int>.Factory.FromAsync(command.BeginExecuteNonQuery(), command.EndExecuteNonQuery)
                .ContinueWith(numRows =>
                                  {
                                      command.Dispose();
                                      return numRows.Result;
                                  });
        }

        public void Commit()
        {
            if (!this.IsOpen) return;
            this._transaction.Commit();
            this._transaction = this._connection.BeginTransaction(IsolationLevel.Serializable);
        }

        private void EnsureIsOpen()
        {
            if (this.IsOpen) return;
            this._connection = new SqlConnection(this.ConnectionString);
            this._connection.Open();
            this._transaction = this._connection.BeginTransaction(IsolationLevel.Serializable);
        }

        private SqlCommand GetCommand(string sql)
        {
            this.EnsureIsOpen();
            var command = this._connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.Transaction = this._transaction;
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