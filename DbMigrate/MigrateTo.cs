using System;
using System.ComponentModel;
using Args;
using DbMigrate.Model;
using DbMigrate.Model.Support.Database;
using DbMigrate.UI;
using Microsoft.Build.Framework;

namespace DbMigrate
{
	public class MigrateTo : Microsoft.Build.Utilities.Task
	{
		private readonly MigrationParameters _args = new MigrationParameters();

		[Description(
			"The target version number (int). If neither target version is set, will go to latest defined version. Negative numbers count back from the end. E.g., -1 means the version right before the latest defined version."
		)]
		public int? TargetMax
		{
			get => _args.TargetMax;
			set => _args.TargetMax = value;
		}

		[Description(
			"The target version number (int). Negative numbers count back from the end. E.g., -1 means the version right before the latest defined version."
		)]
		public int? TargetMin
		{
			get => _args.TargetMin;
			set => _args.TargetMin = value;
		}

		[Required]
		[Description("The database engine to work with.")]
		public string Engine
		{
			get => _args.Engine;
			set => _args.Engine = value;
		}

		[Required]
		[Description("The database to migrate.")]
		public string ConnectionString
		{
			get => _args.ConnectionString;
			set => _args.ConnectionString = value;
		}

		[Required]
		[Description("The folder that contains the migration definition files.")]
		public string MigrationFolderName
		{
			get => _args.Migrations;
			set => _args.Migrations = value;
		}

		[Description("Indicates that this is a test database. Therefore, test data will be applied to it.")]
		public bool IsTestDatabase
		{
			get => _args.IsTestDatabase;
			set => _args.IsTestDatabase = value;
		}

		public override bool Execute()
		{
			try
			{
				User.OnNotify += message => Log.LogMessage(message);
				Validate();
				using (var db = new Target(_args.ResolvedEngine, ConnectionString, IsTestDatabase))
				{
					db.MigrateTo(TargetMin, TargetMax)
						.UsingMigrationsFrom(MigrationFolderName)
						.ExecuteAll();
				}
			}
			catch (TerminateProgramWithMessageException ex)
			{
				Log.LogError(ex.Message);
				return false;
			}
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex, true);
				return false;
			}
			return true;
		}

		public void Validate()
		{
			_args.Validate(Configuration.Configure<MigrationParameters>());
		}
	}
}