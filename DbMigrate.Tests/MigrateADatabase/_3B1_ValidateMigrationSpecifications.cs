﻿using System;
using System.IO;
using DbMigrate.Model;
using DbMigrate.Model.Support.FileFormat;
using DbMigrate.UI;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.MigrateADatabase
{
	[TestFixture]
	public class _3B1_ValidateMigrationSpecifications
	{
		private const string MigrationWithDuplicateSection =
			@"
-- Migration version: 3345
-- Migration start upgrade --
-- Migration insert test data --
-- Migration start upgrade --
";

		private const string MigrationInWrongOrder =
			@"
-- Migration version: 3345
-- Migration insert test data --
-- Migration start upgrade --
";

		private const string MigrationWithUnparsableVersionLine =
			@"
-- Migration version: 33 --
-- Migration start upgrade --
-- Migration start downgrade --
";

		private const string MigrationWithoutVersionNumber =
			@"
-- Migration start upgrade --
-- Migration start downgrade --
";

		private const string MigrationWithBadSectionName =
			@"
-- Migration version: 3345
-- Migration unknown section or typo --

contents to make sure that the error is reported at the line number for the
section header, not at the end of the section.
";

		private const string ValidFileName = "3345_some_migration_name.migration.sql";

		private static Action Parsing(string trivialMigration, string fileName)
		{
			return () => new MigrationSpecification(new MigrationFile(new StringReader(trivialMigration), fileName));
		}

		[Test]
		public void BadMigrationFileNameShouldThrowFriendlyMessage()
		{
			Action testSubject = () => MigrationFile.FileNameVersion("3asfasdf.migration.sql");
			testSubject.Should().Throw<TerminateProgramWithMessageException>()
				.WithMessage(
					@"Invalid migration file name found.

I don't know how to extract the version number from the file name
'3asfasdf.migration.sql'.

Migration files are required to be named like 'X_name.migration.sql', where
X is the version number and name can be anything you want.")
				.And.ErrorLevel.Should().Be(1);
		}

		[Test]
		public void BadSectionNameShouldTerminateWithFriendlyMessage()
		{
			Parsing(MigrationWithBadSectionName, ValidFileName)
				.Should().Throw<TerminateProgramWithMessageException>()
				.WithMessage(
					@"Unable to parse migration file.

It appears that you are attempting to start a new migration section on line 3.
However, I do not recognize the section 'unknown section or typo'.

This tool only understands the following sections, and they must be defined in
this order.
  start upgrade
  insert test data
  finish upgrade
  start downgrade
  delete test data
  finish downgrade

The only required sections are start upgrade and start downgrade.")
				.And.ErrorLevel.Should().Be(1);
		}

		[Test]
		public void DuplicateSectionShouldTerminateWithFriendlyMessage()
		{
			Parsing(MigrationWithDuplicateSection, ValidFileName)
				.Should().Throw<TerminateProgramWithMessageException>()
				.WithMessage(
					@"Unable to parse migration file.

I encountered a duplicate start upgrade section. Each section may only be used once.
The second start upgrade section begins on line 5.

This tool only understands the following sections, and they must be defined in
this order.
  start upgrade
  insert test data
  finish upgrade
  start downgrade
  delete test data
  finish downgrade

The only required sections are start upgrade and start downgrade.")
				.And.ErrorLevel.Should().Be(1);
		}

		[Test]
		public void UnparsableVersionNumberShouldThrowFriendlyErrorMessage()
		{
			Parsing(MigrationWithUnparsableVersionLine, ValidFileName)
				.Should().Throw<TerminateProgramWithMessageException>()
				.WithMessage(
					@"Unable to parse migration file.

I could not parse the version number property in the file. Each migration is
required to start with:

-- Migration version: X

For X, I saw '33 --', which I cannot parse as an integer.

where X is the version number of that migration. This number must match the
version number that is part of the file name. The redundant version number is
to prevent errors due to accidental file rename or forgetting to update file
contents before running a migration during development.")
				.And.ErrorLevel.Should().Be(1);
		}

		[Test]
		public void VersionNumberMismatchShouldTerminateWithFriendlyMessage()
		{
			var contentsFor3345 = _3B2_ExtractMigrationSpecFromFile.MigrationContentsForVersion(3345);
			const string fileNameFor217 = "217_something_awesome.migration.sql";
			Parsing(contentsFor3345, fileNameFor217)
				.Should().Throw<TerminateProgramWithMessageException>()
				.WithMessage(
					@"Unable to parse migration file.

The version number in the file does not match the version number in the
file name.

Name: 217_something_awesome.migration.sql
Contents: -- Migration version: 3345

The formats for these values are:

Name: X_user_friendly_name.migration.sql
Contents: -- Migration version: X

where X is the version number. user_friendly_name can be anything you want.
It must be separated from the version number by an underscore.

The redundant version number is to prevent errors due to accidental file
rename or forgetting to update file contents before running a migration
during development.")
				.And.ErrorLevel.Should().Be(1);
		}

		[Test]
		public void VersionShouldBeRequired()
		{
			Parsing(MigrationWithoutVersionNumber, ValidFileName)
				.Should().Throw<TerminateProgramWithMessageException>()
				.WithMessage(
					@"Unable to parse migration file.

Missing migration version number property in the file. Each migration is
required to start with:

-- Migration version: X

where X is the version number of that migration. This number must match the
version number that is part of the file name. The redundant version number is
to prevent errors due to accidental file rename or forgetting to update file
contents before running a migration during development.")
				.And.ErrorLevel.Should().Be(1);
		}

		[Test]
		public void WrongSectionOrderShouldTerminateWithFriendlyMessage()
		{
			Parsing(MigrationInWrongOrder, ValidFileName)
				.Should().Throw<TerminateProgramWithMessageException>()
				.WithMessage(
					@"Unable to parse migration file.

The migration sections are not in the correct order.
Problem detected on line 4, when I encountered the start upgrade section.

This tool only understands the following sections, and they must be defined in
this order.
  start upgrade
  insert test data
  finish upgrade
  start downgrade
  delete test data
  finish downgrade

The only required sections are start upgrade and start downgrade.")
				.And.ErrorLevel.Should().Be(1);
		}
	}
}