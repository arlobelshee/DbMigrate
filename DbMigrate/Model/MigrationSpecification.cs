using DbMigrate.Model.Support.FileFormat;
using DbMigrate.UI;

namespace DbMigrate.Model
{
	public class MigrationSpecification
	{
		public MigrationSpecification(int version, string name, string apply, string unapply)
			: this(version, name, apply, unapply, string.Empty, string.Empty)
		{
		}

		public MigrationSpecification(int version, string name, string apply, string unapply, string insertTestData,
			string deleteTestData)
		{
			Version = version;
			Name = name;
			Apply = apply;
			Unapply = unapply;
			InsertTestData = insertTestData;
			DeleteTestData = deleteTestData;
			Validate();
		}

		public MigrationSpecification(MigrationFile migrationFile)
			: this(
				migrationFile.Version, migrationFile.Name, migrationFile.Apply, migrationFile.Unapply, migrationFile.InsertTestData,
				migrationFile.DeleteTestData)
		{
		}

		public int Version { get; }
		public string Name { get; }
		public string Apply { get; }
		public string Unapply { get; }
		public string InsertTestData { get; }
		public string DeleteTestData { get; }

		private void Validate()
		{
			Require.That(Version > -1, 1,
				UserMessage.ErrorMigrationFileParsePrefix + UserMessage.ErrorMissingInFileMigrationNumber, Name);
			// And other requirements for other sections.
		}

		public override string ToString()
		{
			return string.Format("Migration version {0}", Version);
		}
	}
}