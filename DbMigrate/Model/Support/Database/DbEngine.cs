namespace DbMigrate.Model.Support.Database
{
	public class DbEngine
	{
		public static DbEngine SqlServer = new DbEngine(
			@"if exists(select * from sys.objects where name = '__database_info' and type in ('U'))
begin
	select top 1 version_number from __database_info;
end
else
begin
	select -1;
end");

		private DbEngine(string requestVersionSql)
		{
			RequestVersionSql = requestVersionSql;
		}

		public string RequestVersionSql { get; }
	}
}