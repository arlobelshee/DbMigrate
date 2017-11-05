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
			Version = -1; // So that, if it isn't set by file contents, we'll make a detectably bad migration.
			_expectedSections = new List<SectionFinder>
			{
				new SectionFinder(0, "apply", s => Apply = s),
				new SectionFinder(1, "insert test data", s => InsertTestData = s),
				new SectionFinder(2, "delete test data", s => DeleteTestData = s),
				new SectionFinder(3, "unapply", s => Unapply = s)
			};
			_fileName = fileName;
			Name = fileName.UpToFirst(".");
			fileContents.Lines().Select(l => l.Trim()).Each(ParseLine);
			FinishCurrentSection();
		}

		public int Version { get; private set; }
		public string Name { get; }
		public string Apply { get; private set; }
		public string Unapply { get; private set; }
		public string InsertTestData { get; private set; }
		public string DeleteTestData { get; private set; }

		private void ParseLine(string line)
		{
			++_currentLine;
			if (line.StartsWith(MigrationVersionHeader))
			{
				StoreVersion(line);
				return;
			}
			if (line.StartsWith(MigrationSectionHeaderPrefix) && line.EndsWith(MigrationSectionHeaderSuffix))
			{
				ChangeCurrentSection(line);
				return;
			}
			StoreLine(line);
		}

		private void ChangeCurrentSection(string line)
		{
			FinishCurrentSection();
			line = line.RemovePrefix(MigrationSectionHeaderPrefix).RemoveSuffix(MigrationSectionHeaderSuffix);
			BeginStoringTo(line);
		}

		private void StoreVersion(string line)
		{
			Version = ParseVersionNumber(line);
			var fileNameVersion = FileNameVersion(_fileName);
			Verify(Version == fileNameVersion,
				UserMessage.ErrorMigrationFileParsePrefix + UserMessage.ErrorVersionNumberMismatch);
		}

		private static int ParseVersionNumber(string line)
		{
			int version;
			var versionNumberAsString = line.Substring(MigrationVersionHeader.Length).Trim();
			Require.That(int.TryParse(versionNumberAsString, out version), 1,
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
			if (line.StartsWith("--") || string.IsNullOrEmpty(line)) return;
			_currentSection.AppendLine(line);
		}

		private void FinishCurrentSection()
		{
			_currentSection?.Store();
		}

		private void BeginStoringTo(string sectionName)
		{
			_currentSection = _expectedSections.Find(s => s.IsFor(sectionName));
			ValidateSectionTransition(sectionName);
			_highestSectionSoFar = _currentSection.Order;
			_currentSection.OnBeginSection();
		}

		private void ValidateSectionTransition(string sectionName)
		{
			Verify(_currentSection != null, SectionErrorFormat(UserMessage.ErrorUnknownSection), sectionName);
			Verify(_currentSection.IsEmpty, SectionErrorFormat(UserMessage.ErrorDuplicateSection), sectionName);
			Verify(_currentSection.Order > _highestSectionSoFar,
				SectionErrorFormat(UserMessage.ErrorSectionOrdering), sectionName);
		}

		private void Verify(bool condition, string errorFormat, string sectionNamefound = null)
		{
			Require.That(condition, 1, errorFormat,
				Name,
				_currentLine,
				_fileName,
				Version,
				_expectedSections.Format(s => $"  {s.Name}", "\r\n"),
				sectionNamefound);
		}

		private static string SectionErrorFormat(string specificErrorFound)
		{
			return UserMessage.ErrorMigrationFileParsePrefix + specificErrorFound + UserMessage.ErrorSectionProblem;
		}
	}
}