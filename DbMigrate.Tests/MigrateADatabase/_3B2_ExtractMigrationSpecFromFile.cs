using System;
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
		private static readonly string ErrorMigration = @"-- Migration version: 3345
-- Migration apply --
create table Foo;
";

		public static string MigrationContentsForVersion(int version)
		{
			return
$@"
-- Migration version: {version}
-- Full-line comments outside of sections should be ignored

-- Migration start upgrade --

-- Full-line comments in sections should be ignored too.
create table Foo;

-- Migration insert test data --

insert into Foo;

-- Migration finish upgrade --

alter table Foo add index;

-- Migration start downgrade --

alter table Foo drop index;

-- Migration delete test data --

delete from Foo;

-- Migration finish downgrade --

drop table Foo;
";
		}

		[Test]
		public void LoadingFileShouldFindDowngradeStartScript()
		{
			var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
			testSubject.BeginDown.Should().Be("alter table Foo drop index;");
		}

		[Test]
		public void LoadingFileShouldFindDowngradeFinishScript()
		{
			var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
			testSubject.FinishDown.Should().Be("drop table Foo;");
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
		public void LoadingErroneousFileShouldProvideErrorInfoToUser()
		{
			Action testSubject = () => new MigrationFile(new StringReader(ErrorMigration), ValidFileName);
			testSubject.Should()
				.Throw<UI.TerminateProgramWithMessageException>()
				.WithMessage(@"Unable to parse migration file.

It appears that you are attempting to start a new migration section on line 2.
However, I do not recognize the section 'apply'.

This tool only understands the following sections, and they must be defined in
this order.
  start upgrade
  insert test data
  finish upgrade
  start downgrade
  delete test data
  finish downgrade

The only required sections are start upgrade and start downgrade.");
		}

		[Test]
		public void LoadingFileShouldFindUpgradeStartScript()
		{
			var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
			testSubject.BeginUp.Should().Be("create table Foo;");
		}

		[Test]
		public void LoadingFileShouldFindUpgradeFinishScript()
		{
			var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
			testSubject.FinishUp.Should().Be("alter table Foo add index;");
		}

		[Test]
		public void ShouldKnowHowToGetVersionNumberFromFileName()
		{
			MigrationFile.FileNameVersion("3_asfasdf.migration.sql").Should().Be(3);
		}
	}
}