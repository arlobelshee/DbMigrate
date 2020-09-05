using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DbMigrate.Util;

namespace DbMigrate.Model.Support.Database
{
	public class TrannectionTraceOnly : ITranection
	{
		private bool _capturing;

		public TrannectionTraceOnly()
		{
			IsDisposed = false;
			IsCommitted = false;
			SqlExecuted = new List<string>();
		}

		[NotNull]
		public Func<string, object> ExecuteScalarHandler { private get; set; } = s => { return null; };
		[NotNull]
		public Action<string> ExecuteNonQueryHandler { private get; set; } = s => { };
		public bool IsDisposed { get; private set; }
		public bool IsCommitted { get; private set; }
		public List<string> SqlExecuted { get; }

		public bool IsOpen
		{
			get { throw new NotImplementedException(); }
		}

		public string ConnectionString
		{
			get { throw new NotImplementedException(); }
		}

		public void Dispose()
		{
			IsDisposed = true;
			GC.SuppressFinalize(this);
		}

		public Task<T> ExecuteScalar<T>(string sql)
		{
			IsCommitted = false;
			if (_capturing)
			{
				SqlExecuted.Add(sql);
				return default(T).ToTask();
			}
			return HelperMethods.AsTask(() => (T) ExecuteScalarHandler(sql));
		}

		public Task<int> ExecuteNonQuery(string sql)
		{
			IsCommitted = false;
			if (_capturing)
			{
				SqlExecuted.Add(sql);
				return 999.ToTask();
			}
			return HelperMethods.AsTask(
				() =>
				{
					ExecuteNonQueryHandler(sql);
					return 999;
				}
			);
		}

		public void Commit()
		{
			IsCommitted = true;
		}

		public TrannectionTraceOnly BeginCapturing()
		{
			_capturing = true;
			return this;
		}
	}
}