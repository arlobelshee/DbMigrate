using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DbMigrate.UI;
using DbMigrate.Util;

namespace DbMigrate.Model.Support.FileFormat
{
    public class MigrationFile
    {
        private const string MigrationVersionHeader = "-- Migration version:";
        private const string MigrationSectionHeaderPrefix = "-- Migration ";
        private const string MigrationSectionHeaderSuffix = " --";

        private readonly List<SectionFinder> _expectedSections;
        private readonly string _fileName;
        private int _currentLine;
        private SectionFinder _currentSection;
        private int _highestSectionSoFar = -1;

        public MigrationFile(TextReader fileContents, string fileName)
        {
            this.Version = -1; // So that, if it isn't set by file contents, we'll make a detectably bad migration.
            this._expectedSections = new List<SectionFinder>
                {
                    new SectionFinder(0, "apply", s => this.Apply = s),
                    new SectionFinder(1, "insert test data", s => this.InsertTestData = s),
                    new SectionFinder(2, "delete test data", s => this.DeleteTestData = s),
                    new SectionFinder(3, "unapply", s => this.Unapply = s),
                };
            this._fileName = fileName;
            this.Name = fileName.UpToFirst(".");
            fileContents.Lines().Select(l => l.Trim()).Each(this.ParseLine);
            this.FinishCurrentSection();
        }

        public int Version { get; private set; }
        public string Name { get; private set; }
        public string Apply { get; private set; }
        public string Unapply { get; private set; }
        public string InsertTestData { get; private set; }
        public string DeleteTestData { get; private set; }

        private void ParseLine(string line)
        {
            ++this._currentLine;
            if (line.StartsWith(MigrationVersionHeader))
            {
                this.StoreVersion(line);
                return;
            }
            if (line.StartsWith(MigrationSectionHeaderPrefix) && line.EndsWith(MigrationSectionHeaderSuffix))
            {
                this.ChangeCurrentSection(line);
                return;
            }
            this.StoreLine(line);
        }

        private void ChangeCurrentSection(string line)
        {
            this.FinishCurrentSection();
            line = line.RemovePrefix(MigrationSectionHeaderPrefix).RemoveSuffix(MigrationSectionHeaderSuffix);
            this.BeginStoringTo(line);
        }

        private void StoreVersion(string line)
        {
            this.Version = ParseVersionNumber(line);
            var fileNameVersion = FileNameVersion(this._fileName);
            this.Verify(this.Version == fileNameVersion,
                UserMessage.ErrorMigrationFileParsePrefix + UserMessage.ErrorVersionNumberMismatch);
        }

        private static int ParseVersionNumber(string line)
        {
            int version;
            var versionNumberAsString = line.Substring(MigrationVersionHeader.Length).Trim();
            Require.That(Int32.TryParse(versionNumberAsString, out version), 1,
                UserMessage.ErrorUnparsableInFileMigrationNumber, versionNumberAsString);
            return version;
        }

        public static int FileNameVersion(string fileName)
        {
            int result;
            Require.That(int.TryParse(fileName.UpToFirst("_"), out result), 1,
                UserMessage.ErrorInvalidFileNameFormat, fileName);
            return result;
        }

        private void StoreLine(string line)
        {
            if (line.StartsWith("--") || String.IsNullOrEmpty(line)) return;
            this._currentSection.AppendLine(line);
        }

        private void FinishCurrentSection()
        {
            if (this._currentSection == null) return;
            this._currentSection.Store();
        }

        private void BeginStoringTo(string sectionName)
        {
            this._currentSection = this._expectedSections.Find(s => s.IsFor(sectionName));
            this.ValidateSectionTransition(sectionName);
            this._highestSectionSoFar = this._currentSection.Order;
            this._currentSection.OnBeginSection();
        }

        private void ValidateSectionTransition(string sectionName)
        {
            this.Verify(this._currentSection != null, SectionErrorFormat(UserMessage.ErrorUnknownSection), sectionName);
            this.Verify(this._currentSection.IsEmpty, SectionErrorFormat(UserMessage.ErrorDuplicateSection), sectionName);
            this.Verify(this._currentSection.Order > this._highestSectionSoFar,
                SectionErrorFormat(UserMessage.ErrorSectionOrdering), sectionName);
        }

        private void Verify(bool condition, string errorFormat, string sectionNamefound = null)
        {
            Require.That(condition, 1, errorFormat,
                this.Name,
                this._currentLine,
                this._fileName,
                this.Version,
                this._expectedSections.Format(s => String.Format("  {0}", s.Name), "\r\n"),
                sectionNamefound);
        }

        private static string SectionErrorFormat(string specificErrorFound)
        {
            return UserMessage.ErrorMigrationFileParsePrefix + specificErrorFound +
                UserMessage.ErrorSectionProblem;
        }
    }
}