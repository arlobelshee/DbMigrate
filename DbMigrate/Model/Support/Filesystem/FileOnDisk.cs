using System.IO;

namespace DbMigrate.Model.Support.Filesystem
{
    public class FileOnDisk : IFile
    {
        private readonly FileInfo _fileInfo;

        public FileOnDisk(FileInfo fileInfo)
        {
            this._fileInfo = fileInfo;
        }

        public string Name
        {
            get { return this._fileInfo.Name; }
        }

        public TextReader OpenText()
        {
            return this._fileInfo.OpenText();
        }
    }
}