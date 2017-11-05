using System.Collections.Generic;

namespace DbMigrate.Model.Support.Filesystem
{
	public class DirectoryInMemory : IDirectory
	{
		public DirectoryInMemory()
		{
			Files = new List<IFile>();
		}

		public List<IFile> Files { get; }

		public bool Equals(IDirectory other)
		{
			return Equals((object) other);
		}

		public void AddFile(string fileName, string contents)
		{
			Files.Add(new FileInMemory(fileName, contents));
		}
	}
}