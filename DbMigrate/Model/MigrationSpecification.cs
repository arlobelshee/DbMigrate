using DbMigrate.Model.Support.FileFormat;
using DbMigrate.UI;

namespace DbMigrate.Model
{
	public class MigrationSpecification
	{
		public MigrationSpecification(int version, string name, string beginUp, string beginDown)
			: this(version, name, beginUp, beginDown, string.Empty, string.Empty)
		{
		}

		public MigrationSpecification(int version, string name, string beginUp, string beginDown, string insertTestData,
			string deleteTestData)
		{
			Version = version;
			Name = name;
			BeginUp = beginUp;
			BeginDown = beginDown;
			InsertTestData = insertTestData;
			DeleteTestData = deleteTestData;
			Validate();
		}

		public MigrationSpecification(MigrationFile migrationFile)
			: this(
				migrationFile.Version, migrationFile.Name, migrationFile.BeginUp, migrationFile.BeginDown, migrationFile.InsertTestData,
				migrationFile.DeleteTestData)
		{
		}

		public int Version { get; }
		public string Name { get; }
		public string BeginUp { get; }
		public string BeginDown { get; }
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