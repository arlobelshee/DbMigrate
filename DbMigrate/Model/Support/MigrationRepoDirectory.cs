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
			_backingRepository = backingRepository;
		}

		private Dictionary<int, IFile> MigrationFiles
		{
			get
			{
				if (_migrationFiles == null)
					_migrationFiles =
						_backingRepository.Files
							.Where(f => f.Name.EndsWith(".migration.sql"))
							.ToDictionary(f => MigrationFile.FileNameVersion(f.Name));
				return _migrationFiles;
			}
		}

		public bool Equals(MigrationRepoDirectory other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other._backingRepository, _backingRepository);
		}

		public int MaxMigrationVersionFound
		{
			get
			{
				if (MigrationFiles.Count == 0) return -1;
				return MigrationFiles.Keys.Max();
			}
		}

		public MigrationSpecification LoadMigrationIfPresent(int version)
		{
			if (!MigrationFiles.ContainsKey(version)) return null;
			return FromFileContents(MigrationFiles[version]);
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
			return Equals(obj as MigrationRepoDirectory);
		}

		public override int GetHashCode()
		{
			return _backingRepository != null ? _backingRepository.GetHashCode() : 0;
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
			return $"Migrations in folder {_backingRepository}.";
		}
	}
}