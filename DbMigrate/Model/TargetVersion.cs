using DbMigrate.Model.Support;
using System;

namespace DbMigrate.Model
{
    public class TargetVersion : IEquatable<TargetVersion>
    {
        public TargetVersion(End endToMove, long destination)
        {
            EndToMove = endToMove;
            Destination = destination;
        }

        public End EndToMove { get; }
        public long Destination { get; }

        public bool Equals(TargetVersion other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.EndToMove == EndToMove && other.Destination == Destination;
        }

        public override bool Equals(object other)
        {
            return Equals(other as TargetVersion);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EndToMove, Destination);
        }

        public static bool operator ==(TargetVersion left, TargetVersion right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TargetVersion left, TargetVersion right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"Change {EndToMove} to {Destination}";
        }

        public enum End { Min, Max };

        public bool IsAtLeast(DatabaseVersion currentVersion)
        {
            return Destination >= EndToMove.Pick(currentVersion);
        }
    }

    public static class EndExtensions
    {
        public static long Pick(this TargetVersion.End self, DatabaseVersion version)
        {
            return self == TargetVersion.End.Min ? version.Min : version.Max;
        }

        public static long Pick(this TargetVersion.End self, long minVersion, long maxVersion)
        {
            return self == TargetVersion.End.Min ? minVersion : maxVersion;
        }

        public static long? Pick(this TargetVersion.End self, long? minVersion, long? maxVersion)
        {
            return self == TargetVersion.End.Min ? minVersion : maxVersion;
        }
    }
}