namespace DbMigrate.Model
{
	public class Migration
	{
		public enum Go
		{
			Up,
			Down
		}

		public Migration(Go direction, MigrationSpecification spec)
		{
			Direction = direction;
			Spec = spec;
		}

		public Go Direction { get; }
		public MigrationSpecification Spec { get; }
	}
}