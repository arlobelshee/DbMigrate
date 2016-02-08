using System;
using System.Collections.Generic;
using System.Linq;
using DbMigrate.Model.Support.FileFormat;
using DbMigrate.Model.Support.Filesystem;

namespace DbMigrate.Model.Support
{
    public class MigrationRepoDirectory : IMigrationLoader, IEquatable<MigrationRepoDirectory>
    {
        private readonly IDirectory _backingRepository;
        private Dictionary<int, IFile> _migrationFiles;

        public MigrationRepoDirectory(IDirectory backingRepository)
        {
            this._backingRepository = backingRepository;
        }

        private Dictionary<int, IFile> MigrationFiles
        {
            get
            {
                if (this._migrationFiles == null)
                {
                    this._migrationFiles =
                        this._backingRepository.Files
                            .Where(f => f.Name.EndsWith(".migration.sql"))
                            .ToDictionary(f => MigrationFile.FileNameVersion(f.Name));
                }
                return this._migrationFiles;
            }
        }

        public bool Equals(MigrationRepoDirectory other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._backingRepository, this._backingRepository);
        }

        public int MaxMigrationVersionFound
        {
            get
            {
                if (this.MigrationFiles.Count == 0) return -1;
                return this.MigrationFiles.Keys.Max();
            }
        }

        public MigrationSpecification LoadMigrationIfPresent(int version)
        {
            if (!this.MigrationFiles.ContainsKey(version)) return null;
            return FromFileContents(this.MigrationFiles[version]);
        }

        public static MigrationSpecification FromFileContents(IFile file)
        {
            using (var contents = file.OpenText())
            {
                return new MigrationSpecification(new MigrationFile(contents, file.Name));
            }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as MigrationRepoDirectory);
        }

        public override int GetHashCode()
        {
            return (this._backingRepository != null ? this._backingRepository.GetHashCode() : 0);
        }

        public static bool operator ==(MigrationRepoDirectory left, MigrationRepoDirectory right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MigrationRepoDirectory left, MigrationRepoDirectory right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("Migrations in folder {0}.", this._backingRepository);
        }
    }
}