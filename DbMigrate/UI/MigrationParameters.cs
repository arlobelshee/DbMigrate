using System.ComponentModel;
using Args;

namespace DbMigrate.UI
{
    [ArgsModel(SwitchDelimiter = "--")]
    [Description("Migrates the target database from its current version to a target version.")]
    public class MigrationParameters
    {
        [Description("The database to migrate. (required)")]
        public string ConnectionString { get; set; }

        [Description(
            "The target version number (int). If not set, will go to latest defined version. Negative numbers count back from the end. E.g., -1 means the version right before the latest defined version."
            )]
        public int? TargetVersion { get; set; }

        [Description("The folder that contains the migration definition files. (required)")]
        public string Migrations { get; set; }

        [Description("Display this help message.")]
        public bool Help { get; set; }

        [Description("Indicates that this is a test database. Therefore, test data will be applied to it.")]
        public bool IsTestDatabase { get; set; }
    }
}