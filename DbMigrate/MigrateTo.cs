using System;
using System.ComponentModel;
using DbMigrate.Model;
using DbMigrate.UI;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DbMigrate
{
    public class MigrateTo : Task
    {
        private readonly MigrationParameters _args = new MigrationParameters();

        [Description(
            "The target version number (int). If not set, will go to latest defined version. Negative numbers count back from the end. E.g., -1 means the version right before the latest defined version."
            )]
        public int? TargetVersion
        {
            get { return this._args.TargetVersion; }
            set { this._args.TargetVersion = value; }
        }

        [Required]
        [Description("The database to migrate.")]
        public string ConnectionString
        {
            get { return this._args.ConnectionString; }
            set { this._args.ConnectionString = value; }
        }

        [Required]
        [Description("The folder that contains the migration definition files.")]
        public string MigrationFolderName
        {
            get { return this._args.Migrations; }
            set { this._args.Migrations = value; }
        }

        [Description("Indicates that this is a test database. Therefore, test data will be applied to it.")]
        public bool IsTestDatabase
        {
            get { return this._args.IsTestDatabase; }
            set { this._args.IsTestDatabase = value; }
        }

        public override bool Execute()
        {
            try
            {
                User.OnNotify += message => this.Log.LogMessage(message);
                using (var db = new Target(this.ConnectionString, this.IsTestDatabase))
                {
                    db.MigrateTo(this.TargetVersion)
                        .UsingMigrationsFrom(this.MigrationFolderName)
                        .ExecuteAll();
                }
            }
            catch (TerminateProgramWithMessageException ex)
            {
                this.Log.LogError(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex, true);
                return false;
            }
            return true;
        }
    }
}