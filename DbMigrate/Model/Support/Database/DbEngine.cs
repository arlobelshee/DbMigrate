using System;
using System.Data.SqlClient;
using System.Data.SQLite;

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
end", typeof(SqlCommand));

		public static DbEngine SqlLite = new DbEngine("System.Data.SQLite", @"
select case (select count(*) from sqlite_master where type='table' and name='__database_info' collate nocase)
when 1 then (select top 1 version_number from __database_info)
else -1
end", typeof(SQLiteCommand));

		public static DbEngine None = new DbEngine("", null, null);

		private readonly Type _arbitraryTypeReferenceToEnsureLibraryIsReferencedAndCopied;
		public readonly string ProviderFactoryName;
		public readonly string RequestVersionSql;

		private DbEngine(string providerFactoryName, string requestVersionSql,
			Type arbitraryTypeReferenceToEnsureLibraryIsReferencedAndCopied)
		{
			ProviderFactoryName = providerFactoryName;
			RequestVersionSql = requestVersionSql;
			_arbitraryTypeReferenceToEnsureLibraryIsReferencedAndCopied =
				arbitraryTypeReferenceToEnsureLibraryIsReferencedAndCopied;
		}
	}
}