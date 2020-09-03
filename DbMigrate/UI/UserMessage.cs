namespace DbMigrate.UI
{
	internal static class UserMessage
	{
		internal const string ErrorMigrationFileParsePrefix = "Unable to parse migration file.\r\n\r\n";

		internal const string ErrorMissingMigration =
			@"Missing migration {1}

I needed to {0} migration {1}, but could not find a definition for it.
Please make sure there is a file in your migration directory named

'{1}_some_name.migration.sql'.";

		internal const string ErrorUnparsableInFileMigrationNumber =
			@"Unable to parse migration file.

I could not parse the version number property in the file. Each migration is
required to start with:

-- Migration version: X

For X, I saw '{0}', which I cannot parse as an integer.

where X is the version number of that migration. This number must match the
version number that is part of the file name. The redundant version number is
to prevent errors due to accidental file rename or forgetting to update file
contents before running a migration during development.";

		internal const string ErrorMissingInFileMigrationNumber =
			@"Missing migration version number property in the file. Each migration is
required to start with:

-- Migration version: X

where X is the version number of that migration. This number must match the
version number that is part of the file name. The redundant version number is
to prevent errors due to accidental file rename or forgetting to update file
contents before running a migration during development.";

		internal const string ErrorInvalidFileNameFormat =
			@"Invalid migration file name found.

I don't know how to extract the version number from the file name
'{0}'.

Migration files are required to be named like 'X_name.migration.sql', where
X is the version number and name can be anything you want.";

		internal const string ErrorMissingMigrationDirectory =
			@"Could not find migration directory.

You said migrations were in '{0}'.
However, I could not find that directory.";

		internal const string ErrorVersionNumberMismatch =
			@"The version number in the file does not match the version number in the
file name.

Name: {2}
Contents: -- Migration version: {3}

The formats for these values are:

Name: X_user_friendly_name.migration.sql
Contents: -- Migration version: X

where X is the version number. user_friendly_name can be anything you want.
It must be separated from the version number by an underscore.

The redundant version number is to prevent errors due to accidental file
rename or forgetting to update file contents before running a migration
during development.";

		internal const string ErrorUnknownSection =
			@"It appears that you are attempting to start a new migration section on line {1}.
However, I do not recognize the section '{5}'.";

		internal const string ErrorDuplicateSection =
			@"I encountered a duplicate {5} section. Each section may only be used once.
The second apply section begins on line {1}.";

		internal const string ErrorSectionOrdering =
			@"The migration sections are not in the correct order.
Problem detected on line {1}, when I encountered the {5} section.";

		internal const string ErrorSectionProblem =
			@"

This tool only understands the following sections, and they must be defined in
this order.
{4}

The only required sections are start upgrade and start downgrade.";
	}
}