using System.IO;
using DbMigrate.Model;
using DbMigrate.Model.Support.FileFormat;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.MigrateADatabase
{
	[TestFixture]
	public class _3B2_ExtractMigrationSpecFromFile
	{
		private const string ValidFileName = "3345_some_migration_name.migration.sql";
		private static readonly string TrivialMigration = MigrationContentsForVersion(3345);

		public static string MigrationContentsForVersion(int version)
		{
			return
				string.Format(
					@"
-- Migration version: {0}
-- Full-line comments outside of sections should be ignored

-- Migration apply --

-- Full-line comments in sections should be ignored too.
create table Foo;

-- Migration insert test data --

insert into Foo;

-- Migration delete test data --

delete from Foo;

-- Migration unapply --

drop table Foo;
",
					version);
		}

		[Test]
		public void LoadingFileShouldFindDowngradeScript()
		{
			var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
			testSubject.BeginDown.Should().Be("drop table Foo;");
		}

		[Test]
		public void LoadingFileShouldFindMigrationVersionNumber()
		{
			var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
			testSubject.Version.Should().Be(3345);
		}

		[Test]
		public void LoadingFileShouldFindSectionToAddTestData()
		{
			var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
			testSubject.InsertTestData.Should().Be("insert into Foo;");
		}

		[Test]
		public void LoadingFileShouldFindSectionToDeleteTestData()
		{
			var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
			testSubject.DeleteTestData.Should().Be("delete from Foo;");
		}

		[Test]
		public void LoadingFileShouldFindUpgradeScript()
		{
			var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
			testSubject.BeginUp.Should().Be("create table Foo;");
		}

		[Test]
		public void ShouldKnowHowToGetVersionNumberFromFileName()
		{
			MigrationFile.FileNameVersion("3_asfasdf.migration.sql").Should().Be(3);
		}
	}
}