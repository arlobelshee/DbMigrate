using System;
using DbMigrate.Model.Support.FileFormat;
using DbMigrate.UI;

namespace DbMigrate.Model
{
    public class MigrationSpecification
    {
        public MigrationSpecification(int version, string name, string apply, string unapply)
            : this(version, name, apply, unapply, String.Empty, String.Empty)
        {
        }

        public MigrationSpecification(int version, string name, string apply, string unapply, string insertTestData,
            string deleteTestData)
        {
            this.Version = version;
            this.Name = name;
            this.Apply = apply;
            this.Unapply = unapply;
            this.InsertTestData = insertTestData;
            this.DeleteTestData = deleteTestData;
            this.Validate();
        }

        public MigrationSpecification(MigrationFile migrationFile)
            : this(
                migrationFile.Version, migrationFile.Name, migrationFile.Apply, migrationFile.Unapply, migrationFile.InsertTestData,
                migrationFile.DeleteTestData)
        {
        }

        public int Version { get; private set; }
        public string Name { get; private set; }
        public string Apply { get; private set; }
        public string Unapply { get; private set; }
        public string InsertTestData { get; private set; }
        public string DeleteTestData { get; private set; }

        private void Validate()
        {
            Require.That(this.Version > -1, 1,
                UserMessage.ErrorMigrationFileParsePrefix + UserMessage.ErrorMissingInFileMigrationNumber, this.Name);
            // And other requirements for other sections.
        }

        public override string ToString()
        {
            return string.Format("Migration version {0}", this.Version);
        }
    }
}