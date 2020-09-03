using DbMigrate.Model.Support.FileFormat;
using DbMigrate.UI;

namespace DbMigrate.Model
{
	public class MigrationSpecification
	{
		public MigrationSpecification(int version, string name,
			string beginUp, string beginDown)
			: this(version, name,
				  beginUp, string.Empty,
				  beginDown, string.Empty,
				  string.Empty, string.Empty)
		{
		}

		public MigrationSpecification(int version, string name,
			string beginUp, string finishUp,
			string beginDown, string finishDown,
			string insertTestData, string deleteTestData)
		{
			Version = version;
			Name = name;
			BeginUp = beginUp;
			FinishUp = finishUp;
			BeginDown = beginDown;
			FinishDown = finishDown;
			InsertTestData = insertTestData;
			DeleteTestData = deleteTestData;
			Validate();
		}

		public MigrationSpecification(MigrationFile migrationFile)
			: this(
				migrationFile.Version, migrationFile.Name,
				migrationFile.StartUp, migrationFile.FinishUp,
				migrationFile.StartDown, migrationFile.FinishDown,
				migrationFile.InsertTestData, migrationFile.DeleteTestData)
		{
		}

		public int Version { get; }
		public string Name { get; }
		public string BeginUp { get; }
		public string FinishUp { get; }
		public string BeginDown { get; }
		public string FinishDown { get; }
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