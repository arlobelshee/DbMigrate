using System.Collections.Generic;

namespace DbMigrate.Model.Support.Filesystem
{
    public class DirectoryInMemory : IDirectory
    {
        public DirectoryInMemory()
        {
            this.Files = new List<IFile>();
        }

        public List<IFile> Files { get; private set; }

        public bool Equals(IDirectory other)
        {
            return this.Equals((object) other);
        }

        public void AddFile(string fileName, string contents)
        {
            this.Files.Add(new FileInMemory(fileName, contents));
        }
    }
}