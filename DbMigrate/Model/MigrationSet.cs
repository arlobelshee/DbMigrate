namespace DbMigrate.Model
{
	public class MigrationSet
	{
		private readonly IDatabase _database;

		public MigrationSet(ChangePlan plan, IDatabase database, params IMigrationLoader[] loaders)
		{
			Plan = plan;
			Loaders = loaders;
			_database = database;
		}

		public IMigrationLoader[] Loaders { get; }
		public ChangePlan Plan { get; }

		public void ExecuteAll()
		{
			Plan.ApplyTo(_database, Loaders);
		}
	}
}