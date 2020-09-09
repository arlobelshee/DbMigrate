using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using DbMigrate.UI;

namespace DbMigrate.Model.Support.Database
{
    public class DbEngine
    {
        public static DbEngine SqlServer = new DbEngine("System.Data.SqlClient", SqlClientFactory.Instance, @"
if exists(select * from sys.objects where name = '__database_info' and type in ('U'))
begin
	select top 1 min_version_number, max_version_number from __database_info;
end
else begin
	select -1, -1;
end");

        public static DbEngine SqlLite = new DbEngine("System.Data.SQLite", SQLiteFactory.Instance,
            @"select
    case version_table.known when 0 then -1 else " + DatabaseVersion.NOT_YET_KNOWN + @" end,
    case version_table.known when 0 then -1 else " + DatabaseVersion.NOT_YET_KNOWN + @" end
    from (select count(*) as known from sqlite_master where type='table' and name='__database_info' collate nocase) as version_table",
            @"select min_version_number, max_version_number from __database_info limit 1");

        public static DbEngine None = new DbEngine("", null);
        private static readonly Dictionary<string, DbEngine> KnownEngines;

        private readonly DbProviderFactory _factory;
        public readonly string ProviderFactoryName;
        public readonly List<string> RequestVersionSql;

        static DbEngine()
        {
            KnownEngines = new Dictionary<string, DbEngine> { { "sqlite", SqlLite }, { "sqlserver", SqlServer } };
        }

        private DbEngine(string providerFactoryName, DbProviderFactory factory,
            params string[] requestVersionSql)
        {
            ProviderFactoryName = providerFactoryName;
            RequestVersionSql = requestVersionSql.ToList();
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