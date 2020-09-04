using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using DbMigrate.UI;

namespace DbMigrate.Model.Support.Database
{
    public class DbEngine
    {
        public static DbEngine SqlServer = new DbEngine("System.Data.SqlClient", @"
if exists(select * from sys.objects where name = '__database_info' and type in ('U'))
begin
	select top 1 max_version_number from __database_info;
end
else
begin
	select -1;
end", SqlClientFactory.Instance);

        public static DbEngine SqlLite = new DbEngine("System.Data.SQLite", @"
select case (select count(*) from sqlite_master where type='table' and name='__database_info' collate nocase)
when 1 then (select top 1 max_version_number from __database_info)
else -1
end", SQLiteFactory.Instance);

        public static DbEngine None = new DbEngine("", null, null);
        private static readonly Dictionary<string, DbEngine> KnownEngines;

        private readonly DbProviderFactory _factory;
        public readonly string ProviderFactoryName;
        public readonly string RequestVersionSql;

        static DbEngine()
        {
            KnownEngines = new Dictionary<string, DbEngine> { { "sqlite", SqlLite }, { "sqlserver", SqlServer } };
        }

        private DbEngine(string providerFactoryName, string requestVersionSql,
            DbProviderFactory factory)
        {
            ProviderFactoryName = providerFactoryName;
            RequestVersionSql = requestVersionSql;
            _factory = factory;
        }

        public static string KnownEngineNames =>
            string.Join(", ", KnownEngines.Keys);

        public static DbEngine LookUpByName(string name)
        {
            if (!KnownEngines.ContainsKey(name.ToLower()))
                throw new TerminateProgramWithMessageException(
                    $"I don't know the database engine '{name}'. I only understand how to communicate with {KnownEngineNames}. Please extend me if you want to use that engine.",
                    3);
            return KnownEngines[name];
        }

        public DbConnection CreateConnection()
        {
            return _factory.CreateConnection();
        }
    }
}