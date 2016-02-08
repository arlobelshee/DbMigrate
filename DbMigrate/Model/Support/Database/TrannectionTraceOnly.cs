using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DbMigrate.Util;

namespace DbMigrate.Model.Support.Database
{
    public class TrannectionTraceOnly : ITranection
    {
        private bool _capturing;

        public TrannectionTraceOnly()
        {
            this.IsDisposed = false;
            this.IsCommitted = false;
            this.SqlExecuted = new List<string>();
        }

        public Func<string, object> ExecuteScalarHandler { private get; set; }
        public Action<string> ExecuteNonQueryHandler { private get; set; }
        public bool IsDisposed { get; private set; }
        public bool IsCommitted { get; private set; }
        public List<string> SqlExecuted { get; private set; }

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
            this.IsDisposed = true;
        }

        public Task<T> ExecuteScalar<T>(string sql)
        {
            this.IsCommitted = false;
            if (this._capturing)
            {
                this.SqlExecuted.Add(sql);
                return default(T).ToTask();
            }
            return HelperMethods.AsTask(() => (T) this.ExecuteScalarHandler(sql));
        }

        public Task<int> ExecuteNonQuery(string sql)
        {
            this.IsCommitted = false;
            if (this._capturing)
            {
                this.SqlExecuted.Add(sql);
                return 999.ToTask();
            }
            return HelperMethods.AsTask(
                () =>
                    {
                        this.ExecuteNonQueryHandler(sql);
                        return 999;
                    }
                );
        }

        public void Commit()
        {
            this.IsCommitted = true;
        }

        public TrannectionTraceOnly BeginCapturing()
        {
            this._capturing = true;
            return this;
        }
    }
}