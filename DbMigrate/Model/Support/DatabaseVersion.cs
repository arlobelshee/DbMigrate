using System;
using System.Diagnostics.CodeAnalysis;

namespace DbMigrate.Model.Support
{
    public class DatabaseVersion : IEquatable<DatabaseVersion>
    {
        public const long NOT_YET_KNOWN = -2;
        public DatabaseVersion(long min, long max)
        {
            Min = min;
            Max = max;
        }

        public long Min { get; }
        public long Max { get; }
        public bool IsKnown { get { return Max != NOT_YET_KNOWN; } }

        public override bool Equals(object obj)
        {
            return Equals(obj as DatabaseVersion);
        }

        public bool Equals([AllowNull] DatabaseVersion other)
        {
            return other != null && Min == other.Min && Max == other.Max;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Min, Max);
        }

        public DatabaseVersion WithMax(long targetVersion)
        {
            return new DatabaseVersion(Min, targetVersion);
        }

        public DatabaseVersion WithMin(long targetVersion)
        {
            return new DatabaseVersion(targetVersion, Max);
        }
    }
}