using System;

namespace DbMigrate.Model
{
    public class ChangeGoal : IEquatable<ChangeGoal>
    {
        public ChangeGoal(int currentVersion, int? targetVersion)
        {
            this.CurrentVersion = currentVersion;
            this.TargetVersion = targetVersion;
        }

        public int CurrentVersion { get; private set; }
        public int? TargetVersion { get; private set; }

        public bool Equals(ChangeGoal other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.CurrentVersion == this.CurrentVersion && other.TargetVersion == this.TargetVersion;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ChangeGoal);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = this.CurrentVersion;
                result = (result*397) ^ (this.TargetVersion ?? -1);
                return result;
            }
        }

        public static bool operator ==(ChangeGoal left, ChangeGoal right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ChangeGoal left, ChangeGoal right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("Request to go from version {0} to version {1}.", this.CurrentVersion,
                ((object) this.TargetVersion) ?? "latest");
        }
    }
}