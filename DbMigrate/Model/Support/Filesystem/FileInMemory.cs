using System.IO;

namespace DbMigrate.Model.Support.Filesystem
{
    public class FileInMemory : IFile
    {
        private readonly string _contents;

        public FileInMemory(string fileName, string contents)
        {
            this._contents = contents;
            this.Name = fileName;
        }

        public string Name { get; private set; }

        public TextReader OpenText()
        {
            return new StringReader(this._contents);
        }
    }
}