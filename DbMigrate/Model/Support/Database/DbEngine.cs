namespace DbMigrate.Model.Support.Database
{
	public class DbEngine
	{
		public static DbEngine SqlServer = new DbEngine("System.Data.SqlClient", @"
if exists(select * from sys.objects where name = '__database_info' and type in ('U'))
begin
	select top 1 version_number from __database_info;
end
else
begin
	select -1;
end");

		public static DbEngine SqlLite = new DbEngine("System.Data.SQLite", @"
select case (select count(*) from sqlite_master where type='table' and name='__database_info' collate nocase)
when 1 then (select top 1 version_number from __database_info)
else -1
end");

		private DbEngine(string providerFactoryName, string requestVersionSql)
		{
			ProviderFactoryName = providerFactoryName;
			RequestVersionSql = requestVersionSql;
		}

		public readonly string ProviderFactoryName;
		public readonly string RequestVersionSql;
	}
}