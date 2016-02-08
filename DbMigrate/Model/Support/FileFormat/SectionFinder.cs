using System;
using System.Text;

namespace DbMigrate.Model.Support.FileFormat
{
    internal class SectionFinder
    {
        private readonly StringBuilder _buffer = new StringBuilder();
        private readonly string _name;
        private readonly int _sectionOrder;
        private readonly Action<string> _whereToStoreResults;
        private bool _hasBeenEncountered;

        public SectionFinder(int sectionOrder, string name, Action<string> whereToStoreResults)
        {
            this._sectionOrder = sectionOrder;
            this._name = name;
            this._whereToStoreResults = whereToStoreResults;
        }

        public string Name
        {
            get { return this._name; }
        }

        public int Order
        {
            get { return this._sectionOrder; }
        }

        public bool IsEmpty
        {
            get { return !this._hasBeenEncountered; }
        }

        public void OnBeginSection()
        {
            this._hasBeenEncountered = true;
        }

        public void Store()
        {
            this._whereToStoreResults(this._buffer.ToString().Trim());
        }

        public bool IsFor(string sectionName)
        {
            return this._name == sectionName;
        }

        public void AppendLine(string line)
        {
            this._buffer.AppendLine(line);
        }
    }
}