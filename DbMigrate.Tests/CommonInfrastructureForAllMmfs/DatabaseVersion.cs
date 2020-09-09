using System;
using System.Diagnostics.CodeAnalysis;

namespace DbMigrate.Tests.CommonInfrastructureForAllMmfs
{
    internal class DatabaseVersion : IEquatable<DatabaseVersion>
    {
        public DatabaseVersion(long min, long max)
        {
            Min = min;
            Max = max;
        }

        public long Min { get; }
        public long Max { get; }

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
    }
}