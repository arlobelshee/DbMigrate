using System.Collections.Generic;
using System.IO;
using System.Linq;
using DbMigrate.UI;

namespace DbMigrate.Model.Support.Filesystem
{
	public class DirectoryOnDisk : IDirectory
	{
		private readonly DirectoryInfo _path;

		public DirectoryOnDisk(string path)
		{
			Require.Not(string.IsNullOrEmpty(path), 1, "You must supply a valid path to your migrations folder.");
			_path = new DirectoryInfo(path);
		}

		public List<IFile> Files
		{
			get { return _path.EnumerateFiles().Select(f => new FileOnDisk(f)).Cast<IFile>().ToList(); }
		}

		public bool Equals(IDirectory obj)
		{
			return Equals(obj as DirectoryOnDisk);
		}

		public bool Equals(DirectoryOnDisk other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other._path.FullName, _path.FullName);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as DirectoryOnDisk);
		}

		public override int GetHashCode()
		{
			return _path.FullName.GetHashCode();
		}

		public static bool operator ==(DirectoryOnDisk left, DirectoryOnDisk right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(DirectoryOnDisk left, DirectoryOnDisk right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return _path.FullName;
		}
	}
}